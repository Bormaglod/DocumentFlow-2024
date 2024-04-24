//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;

using Syncfusion.UI.Xaml.Grid;

using System.Windows.Controls;

namespace DocumentFlow.Views.Browsers;

/// <summary>
/// Логика взаимодействия для WaybillReceiptView.xaml
/// </summary>
public partial class WaybillReceiptView : UserControl, IGridPageView, ISelfTransientLifetime
{
    public WaybillReceiptView()
    {
        InitializeComponent();
    }

    public SfDataGrid DataGrid => gridContent;
}
