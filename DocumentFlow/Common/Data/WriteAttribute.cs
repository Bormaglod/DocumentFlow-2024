//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

namespace DocumentFlow.Common.Data;

/// <summary>
/// Specifies whether a field is writable in the database.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class WriteAttribute : Attribute
{
    /// <summary>
    /// Specifies whether a field is writable in the database.
    /// </summary>
    /// <param name="write">Whether a field is writable in the database.</param>
    public WriteAttribute(bool write)
    {
        Write = write;
    }

    /// <summary>
    /// Whether a field is writable in the database.
    /// </summary>
    public bool Write { get; }
}
