//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Controls;
using DocumentFlow.Interfaces;

namespace DocumentFlow.Views.Browsers;

/// <summary>
/// Логика взаимодействия для OperationsPerformedView.xaml
/// </summary>
public partial class OperationsPerformedView : BaseViewerControl, ISelfTransientLifetime
{
    public OperationsPerformedView()
    {
        InitializeComponent();
        columnOrder.AllowGrouping = true;
        columnLot.AllowGrouping = true;
        columnGoodsCode.AllowGrouping = true;
        columnGoodsItemName.AllowGrouping = true;
    }

    protected override void OnOwnerChanged(object newValue)
    {
        columnOrder.AllowGrouping = newValue == null;
        columnLot.AllowGrouping = newValue == null;
        columnGoodsCode.AllowGrouping = newValue == null;
        columnGoodsItemName.AllowGrouping = newValue == null;
    }
}
