//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Dapper;

using DocumentFlow.Common;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;

using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Windows.Shared;

using System.Windows.Input;

namespace DocumentFlow.ViewModels.Browsers;

public class ProductViewModel<T> : DirectoryViewModel<T>
    where T: Product
{
    public ProductViewModel() { }

    public ProductViewModel(IDatabase database) : base(database) { }

    #region Commands

    #region PopulateThumbnails

    private ICommand? populateThumbnails;

    public ICommand PopulateThumbnails
    {
        get
        {
            populateThumbnails ??= new DelegateCommand<GridDetailsViewExpandingEventArgs>(OnPopulateThumbnailsGroup);
            return populateThumbnails;
        }
    }

    private void OnPopulateThumbnailsGroup(GridDetailsViewExpandingEventArgs e)
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

    #endregion

    protected override void ConfigureColumn(IColumnInfo columnInfo)
    {
        if (columnInfo.MappingName == nameof(Directory.ItemName))
        {
            columnInfo.AlwaysVisible = true;
        }
    }
}
