//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
// Date: 25.11.2023
//
// An introduction to using Windows Image Acquisition (WIA) via C#
// https://www.cyotek.com/blog/an-introduction-to-using-windows-image-acquisition-wia-via-csharp
//-----------------------------------------------------------------------

using System.Text;

using WIA;

namespace DocumentFlow.Scanner;

public static class WiaExtensions
{
    public static string GetValueString(this Property property)
    {
        string value;

        if (property.IsVector)
        {
            value = ((Vector)property.get_Value()).ToSeparatedString();
        }
        else
        {
            value = (WiaPropertyType)property.Type switch
            {
                WiaPropertyType.ClassIDPropertyType or WiaPropertyType.StringPropertyType => (string)property.get_Value(),
                WiaPropertyType.LongPropertyType => ((int)property.get_Value()).ToString(),
                _ => (string)property.get_Value().ToString(),
            };
        }

        return value;
    }

    public static string ToSeparatedString(this Vector vector)
    {
        StringBuilder sb;

        sb = new StringBuilder();

        for (int i = 0; i < vector.Count; i++)
        {
            if (i > 0)
            {
                sb.Append(',').Append(' ');
            }

            sb.Append(vector.get_Item(i + 1).ToString());
        }

        return sb.ToString();
    }
}
