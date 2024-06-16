//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Dapper;

using DocumentFlow.Common;
using DocumentFlow.Common.Data;
using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Dialogs;
using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Messages;
using DocumentFlow.Models.Entities;

using SqlKata;

using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace DocumentFlow.ViewModels.Editors;

public partial class ProductionLotViewModel(
    IProductionOrderRepository orderRepository,
    IGoodsRepository goodsRepository,
    ICalculationRepository calculationRepository,
    IOperationsPerformedRepository operationsPerformedRepository,
    IOrganizationRepository organizationRepository) : DocumentEditorViewModel<ProductionLot>(organizationRepository), IRecipient<DocumentActionMessage<OperationsPerformed>>, ISelfTransientLifetime
{
    [ObservableProperty]
    private ProductionOrder? order;

    [ObservableProperty]
    private Goods? product;

    [ObservableProperty]
    private Calculation? calculation;

    [ObservableProperty]
    private decimal quantity;

    [ObservableProperty]
    private IEnumerable<ProductionOrder>? orders;

    [ObservableProperty]
    private IEnumerable<Goods>? products;

    [ObservableProperty]
    private IEnumerable<Calculation>? calculations;

    [ObservableProperty]
    private IEnumerable<CalculationOperation>? operations;

    [ObservableProperty]
    private ObservableCollection<OperationsPerformed>? operationsPerformed;

    #region Commands

    [RelayCommand]
    private void AddOperation(OperationsPerformedContext context)
    {
        if (Calculation == null || Entity == null)
        {
            return;
        }

        OperationsPerformedWindow window = new();
        if (window.Create(Entity, Calculation, context, out var operationsPerformed))
        {
            try
            {
                using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();
                using var transaction = conn.BeginTransaction();

                try
                {
                    conn.Insert(operationsPerformed, transaction);
                    
                    var sql = $"call execute_system_operation(:Id, 'accept'::system_operation, true, 'operations_performed')";
                    conn.Execute(sql, new { operationsPerformed.Id }, transaction);

                    operationsPerformed.Salary = conn.QuerySingle<decimal>("select salary from operations_performed where id = :Id", new { operationsPerformed.Id });

                    transaction.Commit();

                    OperationsPerformed?.Add(operationsPerformed);

                    var table = EntityProperties.GetTableName(typeof(OperationsPerformed));
                    WeakReferenceMessenger.Default.Send(new EntityActionMessage(table, operationsPerformed.Id, MessageAction.Add));
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(ExceptionHelper.Message(e), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    #endregion

    #region Receives

    public void Receive(DocumentActionMessage<OperationsPerformed> message)
    {
        if (OperationsPerformed == null || (Entity != null && message.Document.OwnerId != Entity.Id))
        {
            return;
        }

        if (message.Document.CarriedOut)
        {
            var doc = OperationsPerformed.FirstOrDefault(x => x.Id == message.Document.Id);
            if (doc == null)
            {
                OperationsPerformed.Add(message.Document);
            }
            else
            {
                OperationsPerformed[OperationsPerformed.IndexOf(doc)] = message.Document;
            }
        }
        else
        {
            var doc = OperationsPerformed.FirstOrDefault(x => x.Id == message.Document.Id);
            if (doc != null)
            {
                OperationsPerformed.Remove(doc);
            }
        }
    }

    #endregion

    protected override string GetStandardHeader() => "Партия";

    protected override void DoAfterLoadDocument(ProductionLot entity)
    {
        DocumentNumber = entity.DocumentNumber;
        DocumentDate = entity.DocumentDate;
        Organization = entity.Organization;
        Order = entity.Order;
        Calculation = entity.Calculation;
        Quantity = entity.Quantity;
        Product = entity.Calculation?.Goods;
    }

    protected override void UpdateEntity(ProductionLot entity)
    {
        entity.DocumentNumber = DocumentNumber;
        entity.DocumentDate = DocumentDate;
        entity.Organization = Organization;
        entity.Order = Order;
        entity.Calculation = Calculation;
        entity.Quantity = Quantity;
    }

    protected override IEnumerable<short> DisabledStates() => [State.Canceled, State.Completed];

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .MappingQuery<ProductionLot>(x => x.State)
            .MappingQuery<ProductionLot>(x => x.Organization)
            .MappingQuery<ProductionLot>(x => x.Order)
            .MappingQuery<ProductionLot>(x => x.Calculation)
            .MappingQuery<Calculation>(x => x.Goods)
            .MappingQuery<Goods>(x => x.Measurement);
    }

    protected override void Load(IDbConnection connection)
    {
        Load<State, Organization, ProductionOrder, Calculation, Goods, Measurement>(connection, (lot, state, org, order, calc, goods, measurement) =>
        {
            goods.Measurement = measurement;

            calc.Goods = goods;

            lot.State = state;
            lot.Organization = org;
            lot.Order = order;
            lot.Calculation = calc;

            return lot;
        });
    }

    protected override void InitializeEntityCollections(IDbConnection connection, ProductionLot? entity = null)
    {
        base.InitializeEntityCollections(connection, entity);

        Orders = orderRepository.GetActive(entity?.Order);
        if (entity != null)
        {
            if (entity.Order != null)
            {
                Products = orderRepository.GetProducts(entity.Order);
            }

            OperationsPerformed = new ObservableCollection<OperationsPerformed>(operationsPerformedRepository.GetOperations(entity));
        }
    }

    partial void OnOrderChanged(ProductionOrder? value)
    {
        if (value != null)
        {
            Products = orderRepository.GetProducts(value);
        }

        Owner = value;
    }

    partial void OnProductChanged(Goods? value)
    {
        if (value != null)
        {
            Calculations = goodsRepository.GetCalculations(value, Calculation);
        }
        else
        {
            Calculations = null;
        }
    }

    partial void OnCalculationChanged(Calculation? value)
    {
        if (value != null)
        {
            Operations = calculationRepository.GetOperations(value, true);
        }
        else
        {
            Operations = null;
        }
    }
}
