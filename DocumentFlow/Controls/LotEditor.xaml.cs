//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common;
using DocumentFlow.Common.Converters;
using DocumentFlow.Models.Entities;

using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace DocumentFlow.Controls;

/// <summary>
/// Логика взаимодействия для LotEditor.xaml
/// </summary>
public partial class LotEditor : UserControl
{
    [GeneratedRegex("^Employees\\[(\\d+)\\].(\\w+)$")]
    private static partial Regex EmpPropertiesRegex();

    private readonly GridRowSizingOptions gridRowResizingOptions = new();
    private readonly ObservableCollection<OperationInfo> operations = new();

    private bool isLoaded;

    public LotEditor()
    {
        InitializeComponent();
    }

    public ObservableCollection<OperationInfo> OperationInfos => operations;

    public decimal LotQuantity
    {
        get => (decimal)GetValue(LotQuantityProperty);
        set => SetValue(LotQuantityProperty, value);
    }

    public IEnumerable<CalculationOperation> Operations
    {
        get => (IEnumerable<CalculationOperation>)GetValue(OperationsProperty);
        set => SetValue(OperationsProperty, value);
    }

    public IEnumerable<OperationsPerformed> OperationsPerformed
    {
        get => (IEnumerable<OperationsPerformed>)GetValue(OperationsPerformedProperty);
        set => SetValue(OperationsPerformedProperty, value);
    }

    public IList<Employee> WorkedEmployes
    {
        get => (IList<Employee>)GetValue(WorkedEmployesProperty);
        set => SetValue(WorkedEmployesProperty, value);
    }

    public ICommand AddOperationCommand
    {
        get { return (ICommand)GetValue(AddOperationCommandProperty); }
        set { SetValue(AddOperationCommandProperty, value); }
    }

    public static readonly DependencyProperty LotQuantityProperty = DependencyProperty.Register(
        nameof(LotQuantity),
        typeof(decimal),
        typeof(LotEditor),
        new FrameworkPropertyMetadata(OnLotQuantityChanged));

    public static readonly DependencyProperty OperationsProperty = DependencyProperty.Register(
        nameof(Operations),
        typeof(IEnumerable<CalculationOperation>),
        typeof(LotEditor));

    public static readonly DependencyProperty OperationsPerformedProperty = DependencyProperty.Register(
        nameof(OperationsPerformed),
        typeof(IEnumerable<OperationsPerformed>),
        typeof(LotEditor),
        new FrameworkPropertyMetadata(OnOperationsPerformedChanged));

    public static readonly DependencyProperty WorkedEmployesProperty = DependencyProperty.Register(
        nameof(WorkedEmployes),
        typeof(IList<Employee>),
        typeof(LotEditor),
        new FrameworkPropertyMetadata(OnWorkedEmployesChanged));

    public static readonly DependencyProperty AddOperationCommandProperty = DependencyProperty.Register(
        nameof(AddOperationCommand),
        typeof(ICommand),
        typeof(LotEditor));

    private void AddOperationsPerformed(OperationsPerformed operation)
    {
        ArgumentNullException.ThrowIfNull(operation.Operation);
        ArgumentNullException.ThrowIfNull(operation.Employee);

        var info = OperationInfos.FirstOrDefault(x => x.Operation.Id == operation.Operation.Id);
        if (info == null)
        {
            info = new OperationInfo() { Operation = operation.Operation, LotQuantity = LotQuantity };

            for (int i = 0; i < WorkedEmployes.Count; i++)
            {
                info.Employees.Add(new EmpInfo(WorkedEmployes[i]));
            }

            OperationInfos.Add(info);
        }

        info.Source.Add(operation);

        gridContent.View.Refresh();
    }

    private void AddEmployee(Employee employee)
    {
        foreach (var item in OperationInfos)
        {
            item.Employees.Add(new EmpInfo(employee));
        }

        AddColumnEmployee(WorkedEmployes.Count - 1);
    }

    private Style CreateStyle(string column)
    {
        Binding binding = new(column)
        {
            Converter = new EmpPerformedColorConverter()
        };

        Setter setter = new(ForegroundProperty, binding);

        Style style = new(typeof(GridCell), (Style)FindResource("SyncfusionGridCellStyle"));
        style.Setters.Add(setter);

        return style;
    }

    private void GridContent_QueryRowHeight(object sender, QueryRowHeightEventArgs e)
    {
        if (gridContent.GetHeaderIndex() == e.RowIndex)
        {
            if (gridContent.GridColumnSizer.GetAutoRowHeight(e.RowIndex, gridRowResizingOptions, out var autoHeight))
            {
                if (autoHeight > 24)
                {
                    e.Height = autoHeight;
                    e.Handled = true;
                }
            }
        }
    }

    private void AddColumnEmployee(int empIndex)
    {
        var qColName = $"Employees[{empIndex}].Quantity";
        var sColName = $"Employees[{empIndex}].Salary";

        GridNumericColumn columnQuantity = new()
        {
            MappingName = qColName,
            HeaderText = "Кол-во",
            Width = 80,
            NumberDecimalDigits = 0,
            CellStyle = CreateStyle(qColName)
        };

        GridCurrencyColumn columnSalary = new()
        {
            MappingName = sColName,
            HeaderText = "Зарплата",
            Width = 90,
            CellStyle = CreateStyle(sColName)
        };

        stackedHeaderRow.StackedColumns.Add(new StackedColumn()
        {
            ChildColumns = $"{qColName},{sColName}",
            HeaderText = WorkedEmployes[empIndex].ToString()
        });

        gridContent.Columns.Insert((empIndex + 1) * 2, columnQuantity);
        gridContent.Columns.Insert((empIndex + 1) * 2 + 1, columnSalary);

        var empSummaryColumn = new GridSummaryColumn
        {
            SummaryType = SummaryType.Custom,
            Format = "{ComplexSum:c}",
            MappingName = sColName,
            Name = sColName,
            CustomAggregate = new ComplexSummaryAggregate()
        };

        summaryRow.SummaryColumns.Add(empSummaryColumn);
    }

    private void LotEditor_Loaded(object sender, RoutedEventArgs e)
    {
        if (isLoaded)
        {
            return;
        }

        // Список всех операций необходимых для изготовления изделия из партии
        operations.Clear();

        if (Operations != null)
        {
            foreach (var item in Operations.OrderBy(x => x.Code))
            {
                operations.Add(new OperationInfo() { Operation = item, LotQuantity = LotQuantity });
            }
        }

        if (WorkedEmployes != null)
        {
            // Список занятых в изготовлении партии
            gridContent.Columns.Suspend();
            for (int i = 0; i < WorkedEmployes.Count; i++)
            {
                foreach (var op in operations)
                {
                    op.Employees.Add(new EmpInfo(WorkedEmployes[i]));
                }

                AddColumnEmployee(i);
            }

            gridContent.Columns.Resume();
            gridContent.RefreshColumns();
        }

        if (OperationsPerformed != null)
        {
            // Список произведенных работ
            foreach (var item in OperationsPerformed)
            {
                if (item.Operation == null)
                {
                    continue;
                }

                var op = operations.FirstOrDefault(x => x.Operation.Id == item.Operation.Id);
                if (op == null)
                {
                    continue;
                }

                op.Source.Add(item);
            }

            gridContent.GridColumnSizer.ResetAutoCalculationforAllColumns();
            gridContent.GridColumnSizer.Refresh();
        }

        gridContent.View.Refresh();

        isLoaded = true;
    }

    private static void OnLotQuantityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LotEditor lotEditor)
        {
            foreach (var item in lotEditor.operations)
            {
                item.LotQuantity = (decimal)e.NewValue;
            }
        }
    }

    private static void OnWorkedEmployesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var action = new NotifyCollectionChangedEventHandler((o, args) =>
        {
            if (d is not LotEditor editor || args.NewItems == null || args.NewItems.Count == 0 || args.NewItems[0] is not Employee employee)
            {
                return;
            }

            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    editor.AddEmployee(employee);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        });

        if (e.OldValue != null)
        {
            var collection = (INotifyCollectionChanged)e.OldValue;
            // Unsubscribe from CollectionChanged on the old collection
            collection.CollectionChanged -= action;
        }

        if (e.NewValue != null)
        {
            var collection = (INotifyCollectionChanged)e.NewValue;
            // Subscribe to CollectionChanged on the new collection
            collection.CollectionChanged += action;
        }
    }

    private static void OnOperationsPerformedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var action = new NotifyCollectionChangedEventHandler((o, args) =>
        {
            if (d is not LotEditor editor || args.NewItems == null || args.NewItems.Count == 0 || args.NewItems[0] is not OperationsPerformed operation)
            {
                return;
            }

            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    editor.AddOperationsPerformed(operation);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        });

        if (e.OldValue != null)
        {
            var collection = (INotifyCollectionChanged)e.OldValue;
            // Unsubscribe from CollectionChanged on the old collection
            collection.CollectionChanged -= action;
        }

        if (e.NewValue != null)
        {
            var collection = (INotifyCollectionChanged)e.NewValue;
            // Subscribe to CollectionChanged on the new collection
            collection.CollectionChanged += action;
        }
    }

    private void GridContent_CellDoubleTapped(object sender, GridCellDoubleTappedEventArgs e)
    {
        if (e.Record is OperationInfo info)
        {
            Employee? employee = null;
            Match m = EmpPropertiesRegex().Match(e.Column.MappingName);
            if (m.Success && int.TryParse(m.Groups[1].Value, out int empIndex))
            {
                employee = info.Employees[empIndex].Employee;
            }

            AddOperationCommand?.Execute(new OperationsPerformedContext() { Operation = info.Operation, Employee = employee });
        }
    }
}
