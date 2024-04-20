//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Json;

using System.IO;
using System.Text.Json.Nodes;

namespace DocumentFlow.Models.Settings;

public class LocalSettings
{
    public string ConnectionName { get; set; } = string.Empty;
    public MainWindowSettings MainWindow { get; set; } = new();
    public PreviewRowSettings PreviewRows { get; set; } = new();
    public ScannerSettings Scanner { get; set; } = new();
    public ReportSettings Report { get; set; } = new();
    public PreviewPdfSettings PreviewPdf { get; set; } = new();

    public void Save()
    {
        var file = Path.Combine(
#if !DEBUG
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Автоком",
            "settings",
#endif
            "appsettings.local.json"
        );

        string json = JsonHelper.GetJsonText(this);

        JsonObject? jsonObj = new()
        {
            ["LocalSettings"] = JsonNode.Parse(json)
        };

        File.WriteAllText(file, jsonObj.ToJsonString(JsonHelper.StandardOptions()));
    }
}
