//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Common.Extensions;

public static class StringExtension
{
    public static string? NullIfEmpty(this string source) => string.IsNullOrEmpty(source) ? null : source;

    /// <summary>
    /// Функция проверяет, является ли строка json-выражением.
    /// Взято из <see href="https://stackoverflow.com/questions/7256142/way-to-quickly-check-if-string-is-xml-or-json-in-c-sharp/7256193#7256193">stackoverflow.com</see>
    /// </summary>
    /// <param name="input">Строка, котороая может быть json-выражением.</param>
    /// <returns>true, если строка представляет собой json-выражение, и false - в противном случае.</returns>
    public static bool IsJson(this string input)
    {
        input = input.Trim();
        return input.StartsWith("{") && input.EndsWith("}")
               || input.StartsWith("[") && input.EndsWith("]");
    }
}
