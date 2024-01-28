//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
// Date: 22.11.2023
//-----------------------------------------------------------------------

using DocumentFlow.Scanner.Enums;

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

using WIA;

namespace DocumentFlow.Scanner;

public class WIAScanner
{
    class WIA_DPS_DOCUMENT_HANDLING_SELECT
    {
        public const uint FEEDER = 0x00000001;
        public const uint FLATBED = 0x00000002;
    }

    class WIA_DPS_DOCUMENT_HANDLING_STATUS
    {
        public const uint FEED_READY = 0x00000001;
    }

    class WIA_PROPERTIES
    {
        public const uint WIA_RESERVED_FOR_NEW_PROPS = 1024;
        public const uint WIA_DIP_FIRST = 2;
        public const uint WIA_DPA_FIRST = WIA_DIP_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
        public const uint WIA_DPC_FIRST = WIA_DPA_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
        //
        // Scanner only device properties (DPS)
        //
        public const uint WIA_DPS_FIRST = WIA_DPC_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
        public const uint WIA_DPS_DOCUMENT_HANDLING_STATUS = WIA_DPS_FIRST + 13;
        public const uint WIA_DPS_DOCUMENT_HANDLING_SELECT = WIA_DPS_FIRST + 14;
    }

    // https://learn.microsoft.com/en-us/previous-versions/windows/desktop/wiaaut/-wiaaut-consts-formatid
    const string wiaFormatBMP = "{B96B3CAB-0728-11D3-9D7B-0000F81EF32E}";
    const string wiaFormatPNG = "{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}";

    // https://learn.microsoft.com/ru-ru/windows/win32/wia/-wia-wia-property-constant-definitions
    const string WIA_DEVICE_PROPERTY_PAGES_ID = "3096";
    const string WIA_SCAN_BRIGHTNESS_PERCENTS = "6154";
    const string WIA_SCAN_CONTRAST_PERCENTS = "6155";
    const string WIA_SCAN_COLOR_MODE = "6146";
    const string WIA_HORIZONTAL_SCAN_RESOLUTION_DPI = "6147";
    const string WIA_VERTICAL_SCAN_RESOLUTION_DPI = "6148";
    const string WIA_HORIZONTAL_SCAN_START_PIXEL = "6149";
    const string WIA_VERTICAL_SCAN_START_PIXEL = "6150";
    const string WIA_HORIZONTAL_SCAN_SIZE_PIXELS = "6151";
    const string WIA_VERTICAL_SCAN_SIZE_PIXELS = "6152";

    const int WIA_S_NO_DEVICE_AVAILABLE = -2145320939; // 0x80210015

    private WIADeviceInfo? currentDevice;
    private Items? items;
    private readonly WiaDeviceType deviceType;
    private readonly DeviceManager manager;

    public WIAScanner()
    {
        deviceType = WiaDeviceType.ScannerDeviceType;
        manager = new DeviceManager();
    }

    /// <summary>
    /// Gets the list of available WIA devices.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<WIADeviceInfo> GetDevices()
    {
        foreach (DeviceInfo info in manager.DeviceInfos)
        {
            if (deviceType == WiaDeviceType.UnspecifiedDeviceType || info.Type == deviceType)
            {
                // https://msdn.microsoft.com/en-us/library/windows/desktop/ms630313(v=vs.85).aspx
                yield return new WIADeviceInfo(info.DeviceID, info.Properties[WIADeviceInfoProp.Manufacturer].get_Value());
            }
        }
    }

    public void SetDevice(WIADeviceInfo device)
    {
        if (currentDevice == null || currentDevice.Value.DeviceID != device.DeviceID)
        {
            currentDevice = device;
            items = null;
        }
    }

    public void ShowDeviceProperties()
    {
        PerformDialogAction((device, dialog) => dialog.ShowDeviceProperties(device));
    }

    public void ShowDeviceItemProperties()
    {
        PerformDeviceAction(device => ScannerPropertiesWindow.ShowPropertiesDialog(device.Properties));
    }

    public void ShowSelectItems()
    {
        PerformDialogAction((device, dialog) => items = dialog.ShowSelectItems(device, SingleSelect: false));
    }

    public void ShowItemProperties()
    {
        var items = GetItems();
        if (items.Count > 0)
        {
            PerformDialogAction((_, dialog) => dialog.ShowItemProperties(items[1]));
        }
        else
        {
            throw new Exception("No items have been selected.");
        }
    }

    public void ShowItemListProperties()
    {
        var items = GetItems();
        if (items.Count > 0)
        {
            ScannerPropertiesWindow.ShowPropertiesDialog(items[1].Properties);
        }
        else
        {
            throw new Exception("No items have been selected.");
        }
    }

    public bool ShowSelectDevice([MaybeNullWhen(false)] out string deviceId)
    {
        var dialog = new CommonDialog();

        Device? selection;
        try
        {
            selection = dialog.ShowSelectDevice(AlwaysSelectDevice: true);
        }
        catch (COMException ex) when (ex.ErrorCode == WIA_S_NO_DEVICE_AVAILABLE)
        {
            throw new Exception("No devices are available. Please plug in a device such as a Scanner or Camera and try again.", ex);
        }

        if (selection != null)
        {
            deviceId = selection.DeviceID;
            return true;
        }

        deviceId = null;
        return false;
    }

    /// <summary>
    /// Use scanner to scan an image (scanner is selected by its unique id).
    /// </summary>
    /// <returns>Scanned images.</returns>
    public IList<BitmapSource> Scan(int pages, ScanSettings settings)
    {
        List<BitmapSource> images = new();
        bool hasMorePages = true;
        int numbrPages = pages;

        while (hasMorePages)
        {
            // select the correct scanner using the provided scannerId parameter
            var device = GetDevice() ?? throw new Exception("No device selected.");
            SetWIAProperty(device.Properties, WIA_DEVICE_PROPERTY_PAGES_ID, 1);
            
            Item item = GetItems()[1];

            if (items == null)
            {
                int width_pixels;
                int height_pixels;

                switch (settings.PageSize)
                {
                    case WIAPageSize.A4:
                        width_pixels = (int)(8.3f * settings.Resolution);
                        height_pixels = (int)(11.7f * settings.Resolution);
                        break;
                    case WIAPageSize.Letter:
                        width_pixels = (int)(8.5f * settings.Resolution);
                        height_pixels = (int)(11f * settings.Resolution);
                        break;
                    case WIAPageSize.Legal:
                        width_pixels = (int)(8.5f * settings.Resolution);
                        height_pixels = (int)(14f * settings.Resolution);
                        break;
                    default:
                        throw new Exception("Unknown WIAPageSize: " + settings.PageSize.ToString());
                }

                AdjustScannerSettings(item, settings.Resolution, 0, 0, width_pixels, height_pixels, settings.Brightness, settings.Contrast, (int)settings.ColorMode);
            }

            try
            {
                // scan image
                ICommonDialog wiaCommonDialog = new CommonDialog();
                ImageFile scanImage = (ImageFile)wiaCommonDialog.ShowTransfer(item, wiaFormatPNG, false);

                if (scanImage != null)
                {
                    byte[] imageBytes = (byte[])scanImage.FileData.get_BinaryData();
                    MemoryStream stream = new(imageBytes);

                    var bitmap = new BitmapImage();

                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    scanImage = null!;

                    // add file to output list
                    images.Add(bitmap);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                item = null!;

                //determine if there are any more pages waiting
                Property documentHandlingSelect = null!;
                Property documentHandlingStatus = null!;
                foreach (Property prop in device.Properties)
                {
                    if (prop.PropertyID == WIA_PROPERTIES.WIA_DPS_DOCUMENT_HANDLING_SELECT)
                        documentHandlingSelect = prop;
                    if (prop.PropertyID == WIA_PROPERTIES.WIA_DPS_DOCUMENT_HANDLING_STATUS)
                        documentHandlingStatus = prop;
                }

                // assume there are no more pages
                hasMorePages = false;
                // may not exist on flatbed scanner but required for feeder
                if (documentHandlingSelect != null)
                {
                    // check for document feeder
                    if ((Convert.ToUInt32(documentHandlingSelect.get_Value()) & WIA_DPS_DOCUMENT_HANDLING_SELECT.FEEDER) != 0)
                    {
                        hasMorePages = ((Convert.ToUInt32(documentHandlingStatus.get_Value()) & WIA_DPS_DOCUMENT_HANDLING_STATUS.FEED_READY) != 0);
                    }
                }
            }

            numbrPages -= 1;
            hasMorePages = numbrPages > 0;
        }

        return images;
    }

    private Device? GetDevice()
    {
        if (currentDevice != null)
        {
            var deviceInfo = manager.DeviceInfos.OfType<DeviceInfo>().First(x => x.DeviceID == currentDevice.Value.DeviceID);
            return deviceInfo.Connect();
        }

        return null;
    }

    private Items GetItems()
    {
        if (items == null)
        {
            var device = GetDevice();
            if (device != null)
            {
                return device.Items;
            }
            else
            {
                throw new Exception("No device selected.");
            }
        }
        else
        {
            return items;
        }
    }

    private void PerformDeviceAction(Action<Device> action)
    {
        var device = GetDevice();
        if (device != null) 
        {
            action(device);
        }
        else
        {
            throw new Exception("No device selected.");
        }
    }

    private void PerformDialogAction(Action<Device, WIA.CommonDialog> action)
    {
        var device = GetDevice();
        if (device != null)
        {
            var dialog = new WIA.CommonDialog();
            action(device, dialog);
        }
        else
        {
            throw new Exception("No device selected.");
        }
    }

    private static void SetWIAProperty(IProperties properties, object propName, object propValue)
    {
        try
        {
            Property prop = properties.get_Item(ref propName);
            prop.set_Value(ref propValue);
        }
        catch (Exception)
        {
        }
    }

    private static void AdjustScannerSettings(IItem scannnerItem,
                                              int scanResolutionDPI,
                                              int scanStartLeftPixel,
                                              int scanStartTopPixel,
                                              int scanWidthPixels,
                                              int scanHeightPixels,
                                              int brightnessPercents,
                                              int contrastPercents,
                                              int colorMode)
    {
        SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_RESOLUTION_DPI, scanResolutionDPI);
        SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_RESOLUTION_DPI, scanResolutionDPI);
        SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_START_PIXEL, scanStartLeftPixel);
        SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_START_PIXEL, scanStartTopPixel);
        SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_SIZE_PIXELS, scanWidthPixels);
        SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_SIZE_PIXELS, scanHeightPixels);
        SetWIAProperty(scannnerItem.Properties, WIA_SCAN_BRIGHTNESS_PERCENTS, brightnessPercents);
        SetWIAProperty(scannnerItem.Properties, WIA_SCAN_CONTRAST_PERCENTS, contrastPercents);
        SetWIAProperty(scannnerItem.Properties, WIA_SCAN_COLOR_MODE, colorMode);
    }
}
