//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common;

using Humanizer;

using System.IO;
using System.Text.Json.Nodes;

namespace DocumentFlow.Settings;

public class BrowserSettings
{
    /*[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<ColumnSettings>? Columns { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<GroupSettings>? Groups { get; set; }

    public ReportPageSettings Page { get; set; } = new();*/

    public void Save(string browserName, object? filter = null)
    {
        var path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Автоком",
            "settings",
            "browsers"
        );

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var file = Path.Combine(path, $"appsettings.{browserName.Underscore()}.json");

        var node = JsonHelper.GetJsonNode(this);
        if (node != null)
        {
            if (filter != null)
            {
                node["Filter"] = JsonHelper.GetJsonNode(filter);
            }

            JsonObject? jsonObj = new()
            {
                [browserName] = node
            };

            File.WriteAllText(file, jsonObj.ToJsonString(JsonHelper.StandardOptions()));
        }
    }
}
