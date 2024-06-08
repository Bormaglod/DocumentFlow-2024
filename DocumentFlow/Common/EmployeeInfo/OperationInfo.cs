//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Models.Entities;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace DocumentFlow.Common;

public partial class OperationInfo : BaseEmpInfo
{
    private readonly ObservableCollection<OperationsPerformed> source = [];
    private readonly ObservableCollection<EmpInfo> employees = [];

    /// <summary>
    /// Возвращает или устанавливает количество изделий в партии.
    /// </summary>
    [ObservableProperty]
    private decimal lotQuantity;

    /// <summary>
    /// Возвращает или устанавливает количество операций <see cref="Operation"/> с учётом повторов на партию.
    /// </summary>
    [ObservableProperty]
    private long operationsByLot;

    [ObservableProperty]
    private bool isComplete;

    public OperationInfo()
    {
        source.CollectionChanged += Operations_CollectionChanged;
    }

    public required CalculationOperation Operation { get; init; }

    public ObservableCollection<OperationsPerformed> Source => source;

    public ObservableCollection<EmpInfo> Employees => employees;

    private void RecalcSummary()
    {
        Quantity = source.Sum(x => x.Quantity);
        Salary = source.Sum(x => x.Salary);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(Quantity) || e.PropertyName == nameof(OperationsByLot))
        {
            IsComplete = OperationsByLot == Quantity;
        }
    }

    private void RecalcEmployeeSummary(EmpInfo emp)
    {
        long quantity = 0;
        decimal salary = 0;
        foreach (var item in source.Where(x => x.Employee != null && x.Employee.Id == emp.Employee.Id))
        {
            quantity += item.Quantity;
            salary += item.Salary;
        }

        emp.Quantity = quantity;
        emp.Salary = salary;
    }

    private void AddOperation(OperationsPerformed operationsPerformed)
    {
        if (operationsPerformed.Employee == null)
        {
            return;
        }

        var emp = employees.FirstOrDefault(x => x.Employee.Id == operationsPerformed.Employee.Id);
        if (emp == null) 
        {
            employees.Add(new EmpInfo(operationsPerformed.Employee) { Quantity = operationsPerformed.Quantity, Salary = operationsPerformed.Salary });
        }
        else
        {
            RecalcEmployeeSummary(emp);
        }
    }

    private void RemoveOperation(OperationsPerformed operationsPerformed)
    {
        if (operationsPerformed.Employee == null)
        {
            return;
        }

        var emp = employees.FirstOrDefault(x => x.Employee.Id == operationsPerformed.Employee.Id);
        if (emp != null)
        {
            RecalcEmployeeSummary(emp);
        }
    }

    private void CheckOperationsPerformed(OperationsPerformed operation)
    {
        if (operation.Operation == null)
        {
            throw new NullReferenceException(nameof(Operation));
        }

        if (operation.Operation.Id != Operation.Id)
        {
            throw new Exception("Неверное значение производственной операции.");
        }

        if (operation.Employee == null)
        {
            throw new Exception("В операции не указан сотрудник.");
        }
    }

    private void Operations_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action) 
        { 
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems != null && e.NewItems.Count > 0 && e.NewItems[0] is OperationsPerformed added)
                {
                    CheckOperationsPerformed(added);
                    AddOperation(added);
                }

                break;
            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems != null && e.OldItems.Count > 0 && e.OldItems[0] is OperationsPerformed removed)
                {
                    CheckOperationsPerformed(removed);
                    RemoveOperation(removed);
                }

                break;
        }

        RecalcSummary();
    }

    partial void OnLotQuantityChanged(decimal value)
    {
        OperationsByLot = Convert.ToInt64(value) * Operation.Repeats;
    }
}

