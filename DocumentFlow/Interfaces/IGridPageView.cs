//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Syncfusion.UI.Xaml.Grid;

namespace DocumentFlow.Interfaces;

public interface IGridPageView : IPageView
{
    SfDataGrid? GetDataGrid();
}
