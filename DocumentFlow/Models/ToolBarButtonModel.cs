//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using Syncfusion.Windows.Tools.Controls;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DocumentFlow.Models;

public class ToolBarButtonModel : ToolBarItemModel
{
    private string? largeIconName;
    private string? smallIconName;
    private ImageSource? largeIcon;
    private ImageSource? smallIcon;

    public ToolBarButtonModel(string label)
    {
        Label = label;
    }

    public ToolBarButtonModel(string label, string iconName)
    {
        Label = label;
        LargeIconName = $"icons8-{iconName}-30";
        SmallIconName = $"icons8-{iconName}-16";
    }

    public string Label { get; }

    public string? LargeIconName 
    {
        get => largeIconName;
        init
        {
            largeIconName = value;
            largeIcon = new BitmapImage(new Uri($"pack://application:,,,/DocumentFlow;component/Images/{largeIconName}.png"));
        }
    }

    public string? SmallIconName
    {
        get => smallIconName;
        init 
        {
            smallIconName = value;
            smallIcon = new BitmapImage(new Uri($"pack://application:,,,/DocumentFlow;component/Images/{smallIconName}.png"));
        }
    }

    public ImageSource? LargeIcon => largeIcon;

    public ImageSource? SmallIcon => smallIcon;

    public SizeMode SizeMode { get; init; }

    public ICommand? Command { get; set; }
}
