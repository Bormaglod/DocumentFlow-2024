//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using SqlKata;

namespace DocumentFlow.ViewModels.Browsers;

public class CuttingViewModel : DirectoryViewModel<Cutting>, ISelfTransientLifetime
{
    public CuttingViewModel() { }

    public CuttingViewModel(IDatabase database) : base(database) { }

    //public override Type? GetEditorViewType() => typeof(Views.Editors.OperationView);

    protected override void ConfigureColumn(IColumnInfo columnInfo)
    {
        if (columnInfo.MappingName == nameof(Directory.ItemName))
        {
            columnInfo.AlwaysVisible = true;
        }
    }

    protected override Query SelectQuery(Query query)
    {
        return base
            .SelectQuery(query)
            .SelectRaw("exists(select 1 from calculation_cutting cc join calculation c on c.id = cc.owner_id where cc.item_id = t0.id and c.state = 'approved'::calculation_state) as operation_using");
    }
}
