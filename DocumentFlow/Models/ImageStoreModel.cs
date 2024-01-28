//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Enums;

using System.Windows.Media.Imaging;

namespace DocumentFlow.Models;

public class ImageStoreModel
{
    private readonly FileExtension imageStore;

    public ImageStoreModel(FileExtension imageStore, string imageName)
    {
        this.imageStore = imageStore;

        Name = imageStore.ToString().ToUpper();
        Image = new BitmapImage(new Uri($"pack://application:,,,/DocumentFlow;component/Images/icons8-{imageName}-16.png"));
    }

    public FileExtension ImageStore => imageStore;
    public string Name { get; }
    public BitmapSource Image { get; }
}
