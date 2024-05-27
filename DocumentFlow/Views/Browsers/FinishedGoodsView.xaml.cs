//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Controls;
using DocumentFlow.Interfaces;

namespace DocumentFlow.Views.Browsers;

/// <summary>
/// Логика взаимодействия для FinishedGoodsView.xaml
/// </summary>
public partial class FinishedGoodsView : BaseViewerControl, ISelfTransientLifetime
{
    public FinishedGoodsView()
    {
        InitializeComponent();
    }

    protected override void OnOwnerChanged(object newValue) 
    {
        columnLotDate.IsHidden = newValue != null;
        columnLotNumber.IsHidden = newValue != null;
        columnLot.AllowGrouping = newValue == null;
    }
}
