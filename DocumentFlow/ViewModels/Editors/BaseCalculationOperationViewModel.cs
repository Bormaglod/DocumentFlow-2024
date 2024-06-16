//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Data;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using SqlKata;

using System.ComponentModel;
using System.Data;

namespace DocumentFlow.ViewModels.Editors;

public abstract partial class BaseCalculationOperationViewModel<T> : DirectoryEditorViewModel<T>
    where T : CalculationOperation, new()
{
    private readonly IEquipmentRepository repoEquipment = null!;

    [ObservableProperty]
    private string code = string.Empty;

    [ObservableProperty]
    private string? itemName;

    [ObservableProperty]
    private Operation? operation;

    [ObservableProperty]
    private decimal price;

    [ObservableProperty]
    private Equipment? equipment;

    [ObservableProperty]
    private Equipment? tool;

    [ObservableProperty]
    private Material? material;

    [ObservableProperty]
    private decimal materialAmount;

    [ObservableProperty]
    private int repeats = 1;

    [ObservableProperty]
    private string? note;

    [ObservableProperty]
    private IEnumerable<Operation>? operations;

    [ObservableProperty]
    private IEnumerable<Equipment>? equipments;

    [ObservableProperty]
    private IEnumerable<Equipment>? tools;

    [ObservableProperty]
    private IEnumerable<Material>? materials;

    public BaseCalculationOperationViewModel() { }

    public BaseCalculationOperationViewModel(IEquipmentRepository repoEquipment) : base()
    {
        this.repoEquipment = repoEquipment;
    }

    protected override void DoAfterLoadDocument(T entity)
    {
        Code = entity.Code;
        ItemName = entity.ItemName;
        Price = entity.Price;
        MaterialAmount = entity.MaterialAmount;
        Repeats = entity.Repeats;
        Note = entity.Note;
        Operation = entity.Operation;
        Equipment = entity.Equipment;
        Tool = entity.Tool;
        Material = entity.Material;
    }

    protected override void UpdateEntity(T entity)
    {
        entity.Code = Code; 
        entity.ItemName = ItemName;
        entity.Price = Price;
        entity.MaterialAmount = MaterialAmount;
        entity.Repeats = Repeats;
        entity.Note = Note;
        entity.Operation = Operation;
        entity.Equipment = Equipment;
        entity.Tool = Tool;
        entity.Material = Material;
    }

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<T>(x => x.Operation)
            .MappingQuery<T>(x => x.Equipment)
            .MappingQuery<T>(x => x.Tool)
            .MappingQuery<T>(x => x.Material);
    }

    protected override void Load(IDbConnection connection)
    {
        Load<Operation, Equipment, Equipment, Material>(
            connection, 
            (op, operation, equipment, tool, material) =>
            {
                op.Operation = operation;
                op.Equipment = equipment;
                op.Tool = tool;
                op.Material = material;
                return op;
            },
            new QueryParameters() { FromOnly = true }
        );
    }

    protected override void InitializeEntityCollections(IDbConnection connection, T? entity = null)
    {
        base.InitializeEntityCollections(connection, entity);

        Equipments = repoEquipment.GetEquipments(connection);
        Tools = repoEquipment.GetTools(connection);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(Operation)) 
        {
            Price = Operation == null ? 0 : Operation.Salary;
        }
    }

    partial void OnCodeChanged(string value)
    {
        UpdateHeader(value);
    }
}
