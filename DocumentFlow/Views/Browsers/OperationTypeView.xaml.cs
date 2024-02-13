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
/// Логика взаимодействия для OperationTypeView.xaml
/// </summary>
public partial class OperationTypeView : UserControl, IGridPageView, ISelfTransientLifetime
{
    public OperationTypeView()
    {
        InitializeComponent();
    }

    public SfDataGrid DataGrid => gridContent;
}
