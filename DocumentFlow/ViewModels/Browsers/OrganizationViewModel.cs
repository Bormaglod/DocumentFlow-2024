//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using System.Data;

namespace DocumentFlow.ViewModels.Browsers;

public class OrganizationViewModel : DirectoryViewModel<Organization>, ISelfTransientLifetime
{
    public OrganizationViewModel() { }

    public OrganizationViewModel(IDatabase database) : base(database) { }

    public override Type? GetEditorViewType() => typeof(Views.Editors.OrganizationView);

    protected override void ConfigureColumn(IColumnInfo columnInfo)
    {
        if (columnInfo.MappingName == nameof(Directory.Code))
        {
            columnInfo.AlwaysVisible = true;
        }
    }

    protected override IReadOnlyList<Organization> GetData(IDbConnection connection, Guid? id)
    {
        return GetData<Okopf>(connection, (org, okopf) => { org.Okopf = okopf; return org; }, id: id);
    }
}
