//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DocumentFlow.Interfaces;
using DocumentFlow.Messages;
using DocumentFlow.Models;
using DocumentFlow.Models.Entities;
using DocumentFlow.Views.Editors;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public sealed partial class CustomerViewModel : EntityGridViewModel<Customer>, ISelfTransientLifetime
{
    public CustomerViewModel() { }

    public CustomerViewModel(IDatabase database, IConfiguration configuration, ILogger<CustomerViewModel> logger) : base(database, configuration, logger) { }

    #region Commands

    [RelayCommand]
    private void OpenContractor()
    {
        if (SelectedItem is Customer customer)
        {
            WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(typeof(ContractorView), customer));
        }
    }

    [RelayCommand]
    private void OpenContract()
    {
        if (SelectedItem is Customer customer && customer.ContractId.HasValue)
        {
            WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(typeof(ContractView), customer.ContractId.Value));
        }
    }

    [RelayCommand]
    private void OpenAppContract()
    {
        if (SelectedItem is Customer customer && customer.ApplicationId.HasValue)
        {
            WeakReferenceMessenger.Default.Send(new EntityEditorOpenMessage(typeof(ContractApplicationView), customer.ApplicationId.Value));
        }
    }

    #endregion

    protected override bool GetSupportAccepting() => false;

    protected override void ConfigureColumn(IColumnInfo columnInfo)
    {
        if (columnInfo.MappingName == nameof(Directory.Code))
        {
            columnInfo.State = ColumnVisibleState.AlwaysVisible;
        }
    }

    protected override IReadOnlyList<Customer> GetData(IDbConnection connection, Guid? id = null)
    {
        var customer = new Query("contractor")
            .Select("contractor.*")
            .SelectRaw("row_number() over(partition by contractor.id order by ca.date_start desc) as rn")
            .Select("ca.code as doc_number", "ca.item_name as doc_name", "ca.date_start as date_start", "pa.price")
            .Select("c.id as contract_id")
            .Select("ca.id as application_id")
            .Join("contract as c", "c.owner_id", "contractor.id")
            .Join("contract_application as ca", "ca.owner_id", "c.id")
            .Join("price_approval as pa", "pa.owner_id", "ca.id")
            .WhereRaw("coalesce(c.date_end, ca.date_end) is null")
            .Where("pa.product_id", Owner?.Id);

        var factory = new QueryFactory(connection, new PostgresCompiler());
        return factory.Query("customer")
            .With("customer", customer)
            .Where("rn", 1)
            .Get<Customer>()
            .ToList();
    }

    protected override void InitializeToolBar()
    {
        ToolBarItems.AddButtons(this,
            new ToolBarButtonModel("Контрагент", "contractor") { Command = OpenContractorCommand },
            new ToolBarButtonModel("Договор", "contract") { Command = OpenContractCommand },
            new ToolBarButtonModel("Приложение", "contract-app") { Command = OpenAppContractCommand });
    }
}
