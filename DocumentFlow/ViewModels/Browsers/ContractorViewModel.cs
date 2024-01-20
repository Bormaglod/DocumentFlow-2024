//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public class ContractorViewModel : DirectoryViewModel<Contractor>, ISelfTransientLifetime
{
    public ContractorViewModel() { }

    public ContractorViewModel(IDatabase database) : base(database) { }

    public override Type? GetEditorViewType() => typeof(Views.Editors.ContractorView);

    protected override void ConfigureColumn(IColumnInfo columnInfo)
    {
        if (columnInfo.MappingName == nameof(Directory.Code))
        {
            columnInfo.AlwaysVisible = true;
        }
    }

    protected override IReadOnlyList<Contractor> GetData(IDbConnection connection, Guid? id)
    {
        return GetData<Okopf>(connection, (contractor, okopf) => { contractor.Okopf = okopf; return contractor; }, id: id);
    }
}
