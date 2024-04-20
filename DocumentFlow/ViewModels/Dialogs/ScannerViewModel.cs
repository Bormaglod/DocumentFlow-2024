//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Enums;
using DocumentFlow.Common.Extensions;
using DocumentFlow.Interfaces;
using DocumentFlow.Models;
using DocumentFlow.Models.Settings;
using DocumentFlow.Scanner;
using DocumentFlow.Scanner.Enums;

using Microsoft.Extensions.Options;

using Syncfusion.UI.Xaml.Utility;
using Syncfusion.Windows.Shared;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DocumentFlow.ViewModels.Dialogs;

public partial class ScannerViewModel : WindowViewModel, ISelfTransientLifetime
{
    private readonly WIAScanner scanner;
    private readonly LocalSettings? settings;

    [ObservableProperty]
    private WIADeviceInfo? selectedDevice;

    [ObservableProperty]
    private ImageStoreModel imageStore;

    [ObservableProperty]
    private int dpi;

    [ObservableProperty]
    private WIAColorMode colorMode;

    [ObservableProperty]
    private int brightness;

    [ObservableProperty]
    private int contrast;

    [ObservableProperty]
    private BitmapSource? currentImage;

    [ObservableProperty]
    private int pageIndex = -1;

    [ObservableProperty]
    private int currentPage = 0;

    [ObservableProperty]
    private int maxPage = 0;

    [ObservableProperty]
    private ObservableCollection<WIADeviceInfo> devices;

    [ObservableProperty]
    public ObservableCollection<ImageStoreModel> imageStoreList;

    [ObservableProperty]
    public ObservableCollection<int> resolutions;

    [ObservableProperty]
    public ObservableCollection<BitmapSource> images = new();

    public ScannerViewModel()
    {
        scanner = new WIAScanner();

        Devices = new(scanner.GetDevices());
        SelectedDevice = Devices.FirstOrDefault();

        ImageStoreList = new ObservableCollection<ImageStoreModel>()
        {
            new(FileExtension.Jpg, "image"),
            new(FileExtension.Png, "image"),
            new(FileExtension.Pdf, "pdf"),
            new(FileExtension.Tif, "image"),
            new(FileExtension.Bmp, "image"),
        };

        Resolutions = new ObservableCollection<int> { 75, 100, 200, 300, 600, 1200 };

        imageStore = ImageStoreList[0];
        dpi = 300;
        colorMode = WIAColorMode.Color;

        Images.CollectionChanged += Images_CollectionChanged;
    }

    public ScannerViewModel(IOptionsSnapshot<LocalSettings> options) : this()
    {
        settings = options.Value;

        RestoreSettings(settings.Scanner.Settings);

        imageStore = ImageStoreList.FirstOrDefault(x => x.ImageStore == settings.Scanner.ImageStore) ?? ImageStoreList[0];
        Dpi = settings.Scanner.Dpi;
        colorMode = settings.Scanner.ColorMode;
        brightness = ScannerSettings.Brightness;
        contrast = ScannerSettings.Contrast;
    }
    
    #region Commands

    #region Scan

    private ICommand? scan;

    public ICommand Scan
    {
        get
        {
            scan ??= new BaseCommand(OnScan);
            return scan;
        }
    }

    private void OnScan(object parameter)
    {
        var images = scanner.Scan(1, new ScanSettings()
        {
            ColorMode = ColorMode,
            Resolution = Dpi,
            Brightness = Brightness,
            Contrast = Contrast
        });

        foreach (var image in images)
        {
            Images.Add(image);
        }

        PageIndex = Images.Count - 1;
    }

    #endregion

    #region ResetScanParameter

    private ICommand? resetScanParameter;

    public ICommand ResetScanParameter
    {
        get
        {
            resetScanParameter ??= new BaseCommand(OnResetScanParameter);
            return resetScanParameter;
        }
    }

    private void OnResetScanParameter(object parameter)
    {
        switch (parameter?.ToString()?.ToUpper())
        {
            case "BRIGHTNESS":
                Brightness = 0;
                break;
            case "CONTRAST":
                Contrast = 0; 
                break;
        }
    }

    #endregion

    #region RotateImage

    private ICommand? rotateImage;

    public ICommand RotateImage
    {
        get
        {
            rotateImage ??= new BaseCommand(OnRotateImage);
            return rotateImage;
        }
    }

    private void OnRotateImage(object parameter)
    {
        if (CurrentImage != null && int.TryParse(parameter?.ToString(), out var degree))
        {
            TransformedBitmap bitmap = new(CurrentImage, new RotateTransform(degree));
            
            Images[PageIndex] = bitmap;
            CurrentImage = bitmap;
        }
    }

    #endregion

    #region RotateImage

    private ICommand? flipImage;

    public ICommand FlipImage
    {
        get
        {
            flipImage ??= new BaseCommand(OnFlipImage);
            return flipImage;
        }
    }

    private void OnFlipImage(object parameter)
    {
        if (CurrentImage != null)
        {
            var axes = parameter?.ToString()?.ToUpper();
            if (axes == null)
            {
                return;
            }

            double scaleX = axes == "X" ? -1 : 1;
            double scaleY = axes == "Y" ? -1 : 1;

            var transform = new ScaleTransform(scaleX, scaleY);

            TransformedBitmap bitmap = new(CurrentImage, transform);

            Images[PageIndex] = bitmap;
            CurrentImage = bitmap;
        }
    }

    #endregion

    #region OpenImage

    private ICommand? openImage;

    public ICommand OpenImage
    {
        get
        {
            openImage ??= new BaseCommand(OnOpenImage);
            return openImage;
        }
    }

    private void OnOpenImage(object parameter)
    {
        Microsoft.Win32.OpenFileDialog dlg = new()
        {
            Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
        };

        if (dlg.ShowDialog() == true)
        {
            var image = new BitmapImage(new Uri(dlg.FileName));

            Images.Add(image);
            PageIndex = Images.Count - 1;
        }
    }

    #endregion

    #region SaveImage

    private ICommand? saveImage;

    public ICommand SaveImage
    {
        get
        {
            saveImage ??= new BaseCommand(OnSaveImage);
            return saveImage;
        }
    }

    private void OnSaveImage(object parameter)
    {
        if (CurrentImage == null)
        {
            return;
        }

        Microsoft.Win32.SaveFileDialog dlg = new()
        {
            Filter = "PNG (*.png)|*.png|JPEG (*.jpg)|*.jpg|BMP (*.bmp)|*.bmp|TIFF (*.tif)|*.tif"
        };

        if (dlg.ShowDialog() == true)
        {
            CurrentImage.SaveToFile(dlg.FileName);
        }
    }

    #endregion

    #region WindowClosing

    private ICommand? windowClosing;

    public ICommand WindowClosing
    {
        get
        {
            windowClosing ??= new DelegateCommand<CancelEventArgs>(OnWindowClosing);
            return windowClosing;
        }
    }

    public void OnWindowClosing(CancelEventArgs e)
    {
        if (settings != null)
        {
            SaveSettings(settings.Scanner.Settings);

            settings.Scanner.ImageStore = ImageStore.ImageStore;
            settings.Scanner.Dpi = Dpi;
            settings.Scanner.ColorMode = ColorMode;

            ScannerSettings.Brightness = Brightness;
            ScannerSettings.Contrast = Contrast;

            settings.Save();
        }
    }

    #endregion

    #endregion

    private void Images_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        MaxPage = Images.Count;
    }

    partial void OnSelectedDeviceChanged(WIADeviceInfo? value)
    {
        if (value != null)
        {
            scanner.SetDevice(value.Value);
        }
    }

    partial void OnPageIndexChanged(int value)
    {
        if (Images == null)
        {
            return;
        }

        if (value < Images.Count)
        {
            CurrentImage = Images[value];
            CurrentPage = value + 1;
        }
    }
}
