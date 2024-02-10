//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Common;
using DocumentFlow.Data;
using DocumentFlow.Interfaces;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Windows;

namespace DocumentFlow.Dialogs;

/// <summary>
/// Логика взаимодействия для CodeGeneratorWindow.xaml
/// </summary>
public partial class CodeGeneratorWindow : Window, INotifyPropertyChanged
{
    [GeneratedRegex("^(\\d{1})\\.(\\d{3,4})\\.(\\d{7}).(\\d{2})$")]
    private static partial Regex CodeRegex();

    private IEnumerable<CodeGenerator> types;
    private IEnumerable<CodeGenerator> brands;
    private IEnumerable<CodeGenerator>? models;
    private IEnumerable<CodeGenerator> engines;
    private CodeGenerator productType;
    private CodeGenerator? brand;
    private CodeGenerator? model;
    private CodeGenerator? engineFrom;
    private CodeGenerator? engineTo;
    private short number = 0;
    private short mod = 0;
    private string codeText = "0.000.3724000.00";

    public event PropertyChangedEventHandler? PropertyChanged;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
    public CodeGeneratorWindow()
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
    {
        InitializeComponent();

        using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

        Types = conn.Query<CodeGenerator>("select * from code_generator where code_info_value = 'type'::code_info");
        Brands = conn.Query<CodeGenerator>("select * from code_generator where code_info_value = 'brand'::code_info");
        Engines = conn.Query<CodeGenerator>("select * from code_generator where code_info_value = 'engine'::code_info");

        ProductType = Types.First(x => x.CodeId == 1);
    }

    public IEnumerable<CodeGenerator> Types
    {
        get => types;
        set
        {
            if (types != value)
            {
                types = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Types)));
            }
        }
    }

    public IEnumerable<CodeGenerator> Brands
    {
        get => brands;
        set
        {
            if (brands != value)
            {
                brands = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Brands)));
            }
        }
    }

    public IEnumerable<CodeGenerator>? Models
    {
        get => models;
        set
        {
            if (models != value)
            {
                models = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Models)));
            }
        }
    }

    public IEnumerable<CodeGenerator> Engines
    {
        get => engines;
        set
        {
            if (engines != value)
            {
                engines = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Engines)));
            }
        }
    }

    public CodeGenerator ProductType
    {
        get => productType;
        set
        {
            if (productType != value)
            {
                productType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProductType)));

                UpdatePanelsVisible();
                UpdateCodeText();
            }
        }
    }

    public CodeGenerator? Brand
    {
        get => brand;
        set
        {
            if (brand != value)
            {
                brand = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Brand)));

                using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();
                Models = conn.Query<CodeGenerator>("select * from code_generator where code_info_value = 'model'::code_info and parent_id = :Id", value);

                UpdateCodeText();
            }
        }
    }

    public CodeGenerator? Model
    {
        get => model;
        set
        {
            if (model != value)
            {
                model = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Model)));

                UpdateCodeText();
            }
        }
    }

    public CodeGenerator? EngineFrom
    {
        get => engineFrom;
        set
        {
            if (engineFrom != value)
            {
                engineFrom = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EngineFrom)));

                UpdateCodeText();
            }
        }
    }

    public CodeGenerator? EngineTo
    {
        get => engineTo;
        set
        {
            if (engineTo != value)
            {
                engineTo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EngineTo)));

                UpdateCodeText();
            }
        }
    }

    public short Number
    {
        get => number;
        set
        {
            if (number != value)
            {
                number = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Number)));

                UpdateCodeText();
            }
        }
    }

    public short Mod
    {
        get => mod;
        set
        {
            if (mod != value)
            {
                mod = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Mod)));

                UpdateCodeText();
            }
        }
    }

    public string CodeText
    {
        get => codeText;
        set
        {
            if (codeText != value)
            {
                codeText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CodeText)));
            }
        }
    }

    public bool Get(string source, [MaybeNullWhen(false)] out string code)
    {
        var match = CodeRegex().Match(source);

        if (!match.Success)
        {
            throw new Exception("Код должен имет вид 0.000.0000000.00");
        }

        ProductType = Types.First(x => x.CodeId == short.Parse(match.Groups[1].Value));

        if (ProductType.CodeId == 4)
        {
            EngineFrom = Engines.FirstOrDefault(x => x.CodeId == short.Parse(match.Groups[2].Value.AsSpan(0, 2)));
            EngineTo = Engines.FirstOrDefault(x => x.CodeId == short.Parse(match.Groups[2].Value.AsSpan(2, 2)));
        }
        else
        {
            Brand = Brands.FirstOrDefault(x => x.CodeId == short.Parse(match.Groups[2].Value.AsSpan(0, 1)));
            Model = Models?.FirstOrDefault(x => x.CodeId == short.Parse(match.Groups[2].Value.AsSpan(1, 2)));
        }

        Number = short.Parse(match.Groups[3].Value.AsSpan(4, 3));
        Mod = short.Parse(match.Groups[4].Value);

        if (ShowDialog() == true)
        {
            code = CodeText;
            return true;
        }

        code = default;
        return false;
    }

    private void UpdatePanelsVisible()
    {
        switch (ProductType.CodeId)
        {
            case 1:
                rowBrand.Height = new(0);
                rowModel.Height = new(0);
                rowEngineFrom.Height = new(0);
                rowEngineTo.Height = new(0);
                break;
            case 2:
            case 3:
                rowBrand.Height = GridLength.Auto;
                rowModel.Height = GridLength.Auto;
                rowEngineFrom.Height = new(0);
                rowEngineTo.Height = new(0);
                break;
            case 4:
                rowBrand.Height = new(0);
                rowModel.Height = new(0);
                rowEngineFrom.Height = GridLength.Auto;
                rowEngineTo.Height = GridLength.Auto;
                break;
        }
    }

    private void UpdateCodeText()
    {
        CodeText = ProductType.CodeId switch
        {
            1 => $"1.000.3724{Number:000}.{Mod:00}",
            2 or 3 => $"{ProductType.CodeId}.{Brand?.CodeId ?? 0}{(Model?.CodeId ?? 0):00}.3724{Number:000}.{Mod:00}",
            4 => $"4.{(EngineFrom?.CodeId ?? 0):00}{(EngineTo?.CodeId ?? 0):00}.3724{Number:000}.{Mod:00}",
            _ => "0.000.0000000.00",
        };
    }

    private void AcceptClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}
