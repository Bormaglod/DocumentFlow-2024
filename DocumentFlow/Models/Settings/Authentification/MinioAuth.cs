//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Models.Settings.Authentification;

public class MinioAuth
{
    public string Server { get; set; } = string.Empty;
    public int Port { get; set; }
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;

    public override string ToString() => $"{Server}:{Port}";
}