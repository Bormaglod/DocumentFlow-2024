//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;
using DocumentFlow.Interfaces.Repository;
using DocumentFlow.Models.Entities;

using Syncfusion.UI.Xaml.Grid;

using System.Collections;
using System.Collections.ObjectModel;

namespace DocumentFlow.Common.Selectors;

public class DocumentStateItemsSelector : IItemsSourceSelector
{
    public IEnumerable GetItemsSource(object record, object dataContext)
    {
        if (record is BaseDocument document && document.State != null) 
        {
            using var conn = ServiceLocator.Context.GetService<IDatabase>().OpenConnection();

            var repo = ServiceLocator.Context.GetService<IStatesRepository>();
            return new ObservableCollection<State>(repo.GetStateTargets(document));
        }

        return null!;
    }
}
