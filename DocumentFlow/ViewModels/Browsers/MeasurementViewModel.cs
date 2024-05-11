//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using Microsoft.Extensions.Configuration;

namespace DocumentFlow.ViewModels.Browsers;

public sealed class MeasurementViewModel : DirectoryViewModel<Measurement>, ISelfTransientLifetime
{
    public MeasurementViewModel() { }

    public MeasurementViewModel(IDatabase database, IConfiguration configuration) : base(database, configuration) { }

    public override Type? GetEditorViewType() => typeof(Views.Editors.MeasurementView);

    protected override void ConfigureColumn(IColumnInfo columnInfo)
    {
        if (columnInfo.MappingName == nameof(Directory.ItemName))
        {
            columnInfo.State = ColumnVisibleState.AlwaysVisible;
        }
    }
}
