//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DocumentFlow.Tools;

public static class ImageHelper
{
    public static ImageSource Base64ToImage(string base64String)
    {
        ArgumentException.ThrowIfNullOrEmpty(base64String);

        byte[] imageBytes = Convert.FromBase64String(base64String);
        using var stream = new MemoryStream(imageBytes, 0, imageBytes.Length);

        return BitmapFrame.Create(stream, BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.OnLoad);
    }
}
