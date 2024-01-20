//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DocumentFlow.Models;

public class NavigatorModel
{
    private readonly ObservableCollection<NavigatorModel> models;

    public NavigatorModel()
    {
        models = new ObservableCollection<NavigatorModel>();
    }

    public ObservableCollection<NavigatorModel> Models => models;

    public required string Header { get; init; }

    public Type? ViewType { get; init; }

    public required string ImageName { get; init; }

    public ImageSource Image
    {
        get
        {
            return new BitmapImage(new Uri($"pack://application:,,,/DocumentFlow;component/Images/{ImageName}.png"));
        }
    }
}
