//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Exceptions;
using DocumentFlow.Interfaces;
using DocumentFlow.Models.Entities;
using Dapper;
using Microsoft.Xaml.Behaviors;
using Humanizer;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;

using System.Transactions;
using System.Windows;

namespace DocumentFlow.Common.Triggers;

public class DocumentStateEndEditTrigger : TargetedTriggerAction<SfDataGrid>
{
    protected override void Invoke(object parameter)
    {
        if (parameter is CurrentCellEndEditEventArgs args)
        {
            if (Target.CurrentColumn.MappingName != nameof(BaseDocument.State))
            {
                return;
            }

            var datarow = Target.RowGenerator.Items.FirstOrDefault(dr => dr.RowIndex == args.RowColumnIndex.RowIndex);
            if (datarow == null || datarow.RowData is not BaseDocument document)
            {
                return;
            }

            if (!document.IsEqualProperties(nameof(BaseDocument.State)))
            {
                using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();
                using var transaction = conn.BeginTransaction();

                try
                {
                    conn.Execute($"update {document.GetType().Name.Underscore()} set state_id = :State where id = :Id", new { State = document.State?.Id, document.Id }, transaction);
                    transaction?.Commit();
                        
                    datarow.Element.DataContext = null;
                    Target.UpdateDataRow(args.RowColumnIndex.RowIndex);
                }
                catch (Exception e)
                {
                    transaction?.Rollback();
                    document.CancelEdit();
                    MessageBox.Show(ExceptionHelper.Message(e), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            document.EndEdit();
        }
    }
}
