//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace DocumentFlow.Common.Converters;

public class PriceStatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var dest = parameter?.ToString()?.ToUpper();
        if (dest == null)
        {
            return null!;
        }

        if (value is int priceStatus)
        {
            if (priceStatus >= 0)
            {
                switch (dest)
                {
                    case "TEXT":
                        string[] texts = { "Действующая", "Ручной ввод", "Устаревшая" };
                        return texts[priceStatus];
                    case "IMAGE":
                        string[] images = { "perfect", "sufficient", "insufficient" };
                        return new BitmapImage(new Uri($"pack://application:,,,/DocumentFlow;component/Images/{images[priceStatus]}.png"));
                }
            }
        }

        return null!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
