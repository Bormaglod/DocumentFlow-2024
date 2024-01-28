//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Enums;

using System.IO;
using System.Windows.Media.Imaging;

namespace DocumentFlow.Common.Extensions;

public static class ImageExtension
{
    public static void SaveToFile(this BitmapSource bitmap, string fileName)
    {
        var ext = Path.GetExtension(fileName).ToUpper();
        FileExtension scannerImageStore = ext switch
        {
            ".PNG" => FileExtension.Png,
            ".JPG" => FileExtension.Jpg,
            ".BMP" => FileExtension.Bmp,
            ".TIF" => FileExtension.Tif,
            _ => throw new NotSupportedException()
        };

        SaveToFile(bitmap, fileName, scannerImageStore);
    }

    public static void SaveToFile(this BitmapSource bitmap, string fileName, FileExtension scannerImageStore) 
    {
        BitmapEncoder encoder = scannerImageStore switch
        {
            FileExtension.Png => new PngBitmapEncoder(),
            FileExtension.Jpg => new JpegBitmapEncoder(),
            FileExtension.Bmp => new BmpBitmapEncoder(),
            FileExtension.Tif => new TiffBitmapEncoder(),
            _ => throw new NotSupportedException()
        };

        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
        encoder.Save(stream);
    }
}