//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Json;

using System.IO;
using System.Reflection;
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

    public static string GetLocalSettingsPath()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        var textVersion = version == null ? "0.0.0" : $"{version.Major}.{version.Minor}.{version.Revision}";

        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Автоком",
            textVersion,
            "settings");
    }

    public void Save()
    {
#if DEBUG
        var file = "appsettings.local.json";
#else
        var file = Path.Combine(
            GetLocalSettingsPath(),
            "appsettings.local.json"
        );
#endif

        string json = JsonHelper.GetJsonText(this);

        JsonObject? jsonObj = new()
        {
            ["LocalSettings"] = JsonNode.Parse(json)
        };

        File.WriteAllText(file, jsonObj.ToJsonString(JsonHelper.StandardOptions()));
    }
}
