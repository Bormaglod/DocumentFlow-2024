//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DocumentFlow.Common.Collections;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using Syncfusion.Windows.Tools.Controls;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Windows;

namespace DocumentFlow.ViewModels.Editors;

public partial class CalculationOperationViewModel : BaseCalculationOperationViewModel<CalculationOperation>, ISelfTransientLifetime
{
    private readonly IMaterialRepository repoMaterial = null!;
    private readonly ICalculationOperationRepository repoCalcOperation = null!;
    private readonly IOperationRepository repoOperation = null!;
    private readonly IPropertyRepository repoProperty = null!;

    [ObservableProperty]
    private string[]? previousOperation;

    [ObservableProperty]
    private DependentCollection<CalculationOperationProperty>? properties;

    [ObservableProperty]
    private CalculationOperationProperty? propertySelected;

    [ObservableProperty]
    private IEnumerable<CalculationOperation>? previousOperationList;

    [ObservableProperty]
    private ObservableCollection<object>? selectedPrevious;

    [ObservableProperty]
    private ObservableCollection<UIElement>? existingProperties;

    public CalculationOperationViewModel() { }

    public CalculationOperationViewModel(
        IEquipmentRepository repoEquipment, 
        IMaterialRepository repoMaterial, 
        IOperationRepository repoOperation,
        ICalculationOperationRepository repoCalcOperation,
        IPropertyRepository repoProperty) : base(repoEquipment)
    {
        this.repoMaterial = repoMaterial;
        this.repoCalcOperation = repoCalcOperation;
        this.repoOperation = repoOperation;
        this.repoProperty = repoProperty;
    }

    #region Commands

    [RelayCommand]
    private void AddProperty()
    {
    }

    [RelayCommand]
    private void EditProperty()
    {
    }

    [RelayCommand]
    private void DeleteProperty()
    {
        if (PropertySelected != null && Properties != null)
        {
            Properties.Remove(PropertySelected);
        }
    }

    [RelayCommand]
    private void OnAddSpecifiedProperty(object parameter)
    {
        if (Properties != null && parameter is Property prop)
        {
            if (Properties.FirstOrDefault(p => p.Property.Id == prop.Id) != null)
            {
                return;
            }

            CalculationOperationProperty calculationOperation = new()
            {
                Property = prop
            };

            Properties.Add(calculationOperation);
        }
    }

    #endregion

    protected override string GetStandardHeader() => "Произв. операция";

    protected override void DoAfterLoadDocument(CalculationOperation entity)
    {
        base.DoAfterLoadDocument(entity);

        PreviousOperation = entity.PreviousOperation;
        if (PreviousOperation != null && PreviousOperationList != null)
        {
            var list = PreviousOperationList.Where(x => PreviousOperation.Contains(x.Code));
            SelectedPrevious = new ObservableCollection<object>(list);
            SelectedPrevious.CollectionChanged += SelectedPrevious_CollectionChanged;
        }
    }

    private void SelectedPrevious_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action) 
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems != null)
                {
                    var newItems = e.NewItems.OfType<CalculationOperation>().Select(x => x.Code).ToArray();
                    if (PreviousOperation == null)
                    {
                        PreviousOperation = newItems;
                    }
                    else
                    {
                        PreviousOperation = [.. PreviousOperation, .. newItems];
                    }
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems != null)
                {
                    var oldItems = e.OldItems.OfType<CalculationOperation>().Select(x => x.Code).ToArray();
                    if (PreviousOperation != null)
                    {
                        PreviousOperation = PreviousOperation.Except(oldItems).ToArray();
                    }
                }
                break;
        }
    }

    protected override void UpdateEntity(CalculationOperation entity)
    {
        base.UpdateEntity(entity);

        entity.PreviousOperation = PreviousOperation;
    }

    protected override void InitializeEntityCollections(IDbConnection connection, CalculationOperation? entity = null)
    {
        base.InitializeEntityCollections(connection, entity);

        Materials = repoMaterial.GetMaterials(connection);
        Operations = repoOperation.GetOperations(connection);
        
        if (Owner is Calculation calculation)
        {
            PreviousOperationList = repoCalcOperation.GetPreviousOperations(connection, calculation, entity);
        }

        if (entity != null) 
        {
            Properties = new DependentCollection<CalculationOperationProperty>(entity, repoCalcOperation.GetProperties(connection, entity));
        }
        else
        {
            Properties ??= [];
        }    

        ExistingProperties = [];
        foreach (var item in repoProperty.GetSlim(connection))
        {
            ExistingProperties.Add(new DropDownMenuItem() { Header = item, Command = addSpecifiedPropertyCommand, CommandParameter = item });
        }
    }

    protected override void UpdateDependents(IDbConnection connection, CalculationOperation operation, IDbTransaction? transaction = null)
    {
        if (Properties != null)
        {
            Properties.Owner = operation;
            connection.UpdateDependents(Properties, transaction);
        }
    }
}
