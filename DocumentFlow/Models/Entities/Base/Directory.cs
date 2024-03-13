//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using DocumentFlow.Common.Data;

namespace DocumentFlow.Models.Entities;

public abstract partial class Directory : DocumentInfo, IComparable, IComparable<Directory>
{
    [ObservableProperty]
    [property: DenyCopying]
    private string code = string.Empty;

    [ObservableProperty]
    private string? itemName;

    public Guid? ParentId { get; set; }

    public bool IsFolder { get; protected set; }

    public override string ToString() => string.IsNullOrEmpty(ItemName) ? Code : ItemName;

    public int CompareTo(object? obj)
    {
        if (obj is Directory other)
        {
            return CompareTo(other);
        }

        throw new Exception($"{obj} должен быть типа {GetType().Name}");
    }

    public int CompareTo(Directory? other)
    {
        if (other == null)
        {
            return 1;
        }

        if (IsFolder != other.IsFolder)
        {
            return IsFolder ? -1 : 1;
        }

        return ToString().CompareTo(other.ToString());
    }

    protected override string GetRowStatusImageName()
    {
        if (IsFolder) 
        { 
            if (Deleted)
            {
                return "icons8-folder-delete-16";
            }
            else
            {
                return "icons8-folder-16";
            }
        }

        return base.GetRowStatusImageName();
    }
}
