//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

namespace DocumentFlow.ViewModels.Browsers;

public class OkopfViewModel : DirectoryViewModel<Okopf>, ISelfTransientLifetime
{
    public OkopfViewModel() { }

    public OkopfViewModel(IDatabase database) : base(database) { }

    public override Type? GetEditorViewType() => typeof(Views.Editors.OkopfView);

    protected override void ConfigureColumn(IColumnInfo columnInfo)
    {
        if (columnInfo.MappingName == nameof(Directory.ItemName))
        {
            columnInfo.AlwaysVisible = true;
        }
    }
}
