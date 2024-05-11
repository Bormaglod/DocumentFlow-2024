//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Converters;

using Syncfusion.UI.Xaml.Grid;

using System.Windows.Data;

namespace DocumentFlow.Common;

public partial class CustomGroupColumn : ObservableObject
{
    [ObservableProperty]
    private int order;

    public CustomGroupColumn(GridColumnBase column, bool isGroup, int order)
    {
        MappingName = column.MappingName;
        Text = column.HeaderText;
        GroupName = isGroup ? column.HeaderText : "Все столбцы";
        Order = order;
    }

    public CustomGroupColumn(CustomGroupColumn parent, IValueConverter converter, int order)
    {
        MappingName = parent.MappingName;
        Text = $"{parent.Text}: {((CustomGroupConverter)converter).Text}";
        GroupName = parent.GroupName;
        Order = order;
        Converter = converter;
    }

    public string MappingName { get; }
    public string Text { get; }
    public string GroupName { get; }

    public IValueConverter? Converter { get; }

    public override string ToString() => $"{MappingName}, order = {Order}";
}
