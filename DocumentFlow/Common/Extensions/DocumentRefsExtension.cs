//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Ghostscript;
using DocumentFlow.Models.Entities;
using DocumentFlow.Tools;

using System.IO;
using System.Windows.Media.Imaging;

namespace DocumentFlow.Common.Extensions;

public static class DocumentRefsExtension
{
    public static void CreateThumbnailImage(this DocumentRefs document, string fileName, int imageSize)
    {
        string ext = Path.GetExtension(fileName);

        string[] images = { ".jpg", ".jpeg", ".png", ".bmp" };
        if (images.Contains(ext))
        {
            InternalCreateThumbnailImage(document, fileName, imageSize);
        }
        else if (ext == ".pdf")
        {
            var path = FileHelper.PrepareTempPath("Thumbnails");
            var name = Path.Combine(path, Path.GetFileName(fileName));

            GhostscriptWrapper.GeneratePageThumb(fileName, name, 1, 200, 200);
            InternalCreateThumbnailImage(document, name, imageSize);
        }
        else
        {
            document.Thumbnail = null;
        }
    }

    private static void InternalCreateThumbnailImage(DocumentRefs document, string fileName, int imageSize)
    {
        BitmapImage image = new();

        image.BeginInit();
        image.UriSource = new Uri(fileName);
        image.DecodePixelWidth = imageSize;
        image.EndInit();

        document.Thumbnail = ImageHelper.ImageToBase64(image);
    }
}
