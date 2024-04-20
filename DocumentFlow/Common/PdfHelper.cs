//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common;
using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;

using FastReport;
using FastReport.Export.Image;

using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;

using System.IO;
using System.Windows.Media.Imaging;

namespace DocumentFlow.Tools;

public class PdfHelper
{
    public static string CreateDocument(Report report, int reportResolution, PdfNamingStrategy namingStrategy)
    {
        var pdfFile = FileHelper.GetTempFileName("Reports", namingStrategy, FileExtension.Pdf);
        var path = Path.GetDirectoryName(pdfFile) ?? string.Empty;

        ImageExport exp = new()
        {
            ImageFormat = ImageExportFormat.Png,
            Resolution = reportResolution
        };

        exp.Export(report, Path.Combine(path, Guid.NewGuid() + ".png"));

        PdfDocument pdf = new(new PdfWriter(pdfFile));
        Document document = new(pdf);
        document.SetMargins(0, 0, 0, 0);

        foreach (string fileName in exp.GeneratedFiles)
        {
            var data = ImageDataFactory.Create(fileName);
            var image = new iText.Layout.Element.Image(data);

            var size = new iText.Kernel.Geom.PageSize(
                image.GetImageWidth().DpiToPoint(exp.ResolutionX),
                image.GetImageHeight().DpiToPoint(exp.ResolutionY));
            pdf.AddNewPage(size);

            document.Add(image);
        }

        document.Close();

        return pdfFile;
    }

    public static string CreateDocument(IList<BitmapSource> images, PdfNamingStrategy namingStrategy)
    {
        var pdfFile = FileHelper.GetTempFileName("Reports", namingStrategy, FileExtension.Pdf);

        PdfDocument pdf = new(new PdfWriter(pdfFile));
        Document document = new(pdf);
        document.SetMargins(0, 0, 0, 0);

        foreach (var image in images)
        {
            var data = ImageDataFactory.Create(ImageToByteArray(image));
            var pdfImage = new iText.Layout.Element.Image(data);

            var size = new iText.Kernel.Geom.PageSize(
                pdfImage.GetImageWidth().DpiToPoint(Convert.ToSingle(image.DpiX)),
                pdfImage.GetImageHeight().DpiToPoint(Convert.ToSingle(image.DpiY)));
            pdf.AddNewPage(size);

            document.Add(pdfImage);
        }

        document.Close();

        return pdfFile;
    }

    private static byte[] ImageToByteArray(BitmapSource imageIn)
    {
        using var ms = new MemoryStream();

        JpegBitmapEncoder encoder = new()
        {
            QualityLevel = 100
        };

        encoder.Frames.Add(BitmapFrame.Create(imageIn));
        encoder.Save(ms);

        return ms.ToArray();
    }
}
