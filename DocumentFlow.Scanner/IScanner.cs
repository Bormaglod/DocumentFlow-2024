//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
// Date: 29.11.2023
//-----------------------------------------------------------------------

using DocumentFlow.Scanner.Enums;

using System.Diagnostics.CodeAnalysis;
using System.Windows.Media.Imaging;

namespace DocumentFlow.Scanner;

public interface IScanner
{
    /// <summary>
    /// Gets the list of available WIA devices.
    /// </summary>
    /// <returns></returns>
    IEnumerable<WIADeviceInfo> GetDevices();

    void SetDevice(WIADeviceInfo device);
    void ShowDeviceProperties();
    void ShowDeviceItemProperties();
    void ShowSelectItems();
    void ShowItemProperties();
    void ShowItemListProperties();
    bool ShowSelectDevice([MaybeNullWhen(false)] out string deviceId);
    
    /// <summary>
    /// Use scanner to scan an image (scanner is selected by its unique id).
    /// </summary>
    /// <returns>Scanned images.</returns>
    IList<BitmapSource> Scan(int pages, int resolution = 300, WIAPageSize pageSize = WIAPageSize.A4);
}
