//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.Input;

using Dapper;

using DocumentFlow.Common;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Syncfusion.UI.Xaml.Grid;

namespace DocumentFlow.ViewModels.Browsers;

public partial class ProductViewModel<T> : DirectoryViewModel<T>
    where T: Product
{
    public ProductViewModel() { }

    public ProductViewModel(IDatabase database, IConfiguration configuration, ILogger<ProductViewModel<T>> logger) : base(database, configuration, logger) { }

    #region Commands

    [RelayCommand]
    private void PopulateThumbnailsGroup(GridDetailsViewExpandingEventArgs e)
    {
        if (e.Record is Product product && product.HasThumbnails)
        {
            using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

            product.Thumbnails = conn.Query<DocumentRefs>("select * from document_refs where owner_id = :Id and thumbnail is not null", product).ToList();
        }
        else
        {
            e.Cancel = true;
        }
    }

    #endregion

    protected override void ConfigureColumn(IColumnInfo columnInfo)
    {
        if (columnInfo.MappingName == nameof(Directory.ItemName))
        {
            columnInfo.State = ColumnVisibleState.AlwaysVisible;
        }
    }
}
