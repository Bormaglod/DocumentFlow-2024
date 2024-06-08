//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace DocumentFlow.Dialogs;

/// <summary>
/// Логика взаимодействия для OperationsPerformedWindow.xaml
/// </summary>
[INotifyPropertyChanged]
public partial class OperationsPerformedWindow : Window
{
    [ObservableProperty]
    private DateTime operationDate = DateTime.Now;

    [ObservableProperty]
    private CalculationOperation? operation;

    [ObservableProperty]
    private Material? usingMaterial;

    [ObservableProperty]
    private bool skipMaterial;

    [ObservableProperty]
    private Employee? employee;

    [ObservableProperty]
    private long quantity;

    [ObservableProperty]
    private bool? doubleRate;

    [ObservableProperty]
    private IEnumerable<CalculationOperation>? operations;

    [ObservableProperty]
    private IEnumerable<Material>? materials;

    [ObservableProperty]
    private IEnumerable<Employee>? employees;

    public OperationsPerformedWindow()
    {
        InitializeComponent();
    }

    public bool Create(ProductionLot lot, Calculation calculation, OperationsPerformedContext context, [MaybeNullWhen(false)] out OperationsPerformed operationsPerformed)
    {
        var calcRepo = ServiceLocator.Context.GetService<ICalculationRepository>();
        Operations = calcRepo.GetOperations(calculation, true);

        var matRepo = ServiceLocator.Context.GetService<IMaterialRepository>();
        Materials = matRepo.GetSlim();

        var empRepo = ServiceLocator.Context.GetService<IOurEmployeeRepository>();
        Employees = empRepo.GetSlim();

        Employee = context.Employee;
        Operation = context.Operation;

        if (ShowDialog() == true)
        {
            operationsPerformed = new()
            {
                OwnerId = lot.Id,
                DocumentDate = OperationDate,
                Employee = Employee,
                Operation = Operation,
                ReplacingMaterial = UsingMaterial,
                Quantity = Quantity,
                SkipMaterial = SkipMaterial,
                DoubleRate = DoubleRate
            };

            return true;
        }

        operationsPerformed = null;
        return false;
    }

    private void UpdateMaterialSelectStatus()
    {
        if (SkipMaterial || Operation == null)
        {
            selectMaterial.IsEnabled = false;
            return;
        }

        selectMaterial.IsEnabled = Operation.Material != null;
    }

    private void AcceptClick(object sender, RoutedEventArgs e)
    {
        if (Employee == null)
        {
            MessageBox.Show("Необходимо выбрать сотрудника.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (Operation == null)
        {
            MessageBox.Show("Необходимо выбрать выполненную операцию.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (Quantity <= 0)
        {
            MessageBox.Show("Необходимо указать количество операций.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        // Если установлен флаг "Материал не использовать", то проверку на количество пропустим...
        if (!SkipMaterial && Operation.Material != null)
        {
            decimal materialCount;
            var matRepo = ServiceLocator.Context.GetService<IMaterialRepository>();
            if (UsingMaterial != null)
            {
                materialCount = matRepo.GetRemainder(UsingMaterial, OperationDate);
            }
            else
            {
                materialCount = matRepo.GetRemainder(Operation.Material, OperationDate);
            }

            decimal expense = Quantity * Operation.MaterialAmount;
            if (expense > materialCount)
            {
                MessageBox.Show($"Количества материала ({materialCount}) недостаточно для выполнения этой операции.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        DialogResult = true;
    }

    private void OperationWindow_Loaded(object sender, RoutedEventArgs e)
    {
        textQuantity.Focus();
    }

    partial void OnOperationChanged(CalculationOperation? value)
    {
        UpdateMaterialSelectStatus();
    }

    partial void OnSkipMaterialChanged(bool value)
    {
        UpdateMaterialSelectStatus();
    }
}
