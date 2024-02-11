//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DocumentFlow.Models.Entities;

public abstract class DocumentInfo : Entity
{
    private Dictionary<PropertyInfo, object?>? storedValues;

    public Guid UserCreatedId { get; protected set; }
    public DateTime DateCreated { get; protected set; }
    public string? UserCreated { get; protected set; }
    public Guid UserUpdatedId { get; protected set; }
    public DateTime DateUpdated { get; protected set; }
    public string? UserUpdated { get; protected set; }
    public bool Deleted { get; protected set; }

    /// <summary>
    /// Возвращает true, если документ имеет прикрепленные файлы.
    /// </summary>
    public bool HasDocuments { get; protected set; } = false;

    /// <summary>
    /// Возвращает true, если документ имеет сохранённые эскизы изображений.
    /// </summary>
    public bool HasThumbnails { get; protected set; }

    public ImageSource RowStatusImage
    {
        get
        {
            return new BitmapImage(new Uri($"pack://application:,,,/DocumentFlow;component/Images/{GetRowStatusImageName()}.png"));
        }
    }

    public void BeginEdit()
    {
        storedValues = BackUp();
    }

    public void CancelEdit()
    {
        if (storedValues == null)
        {
            return;
        }

        foreach (var item in storedValues)
        {
            item.Key.SetValue(this, item.Value);
        }
    }

    public bool IsEqualProperties(string propertyName)
    {
        if (storedValues == null)
        {
            return false;
        }

        var value = storedValues.First(x => x.Key.Name == propertyName);

        var current = value.Key.GetValue(this);

        if (current == null && value.Value == null)
        {
            return true;
        }

        if (current == null || value.Value == null)
        {
            return false;
        }

        return current.Equals(value.Value);
    }

    public void EndEdit()
    {
        if (storedValues != null)
        {
            storedValues.Clear();
            storedValues = null;
        }
    }

    protected virtual string GetRowStatusImageName()
    {
        if (Deleted)
        {
            return "icons8-document-delete-16";
        }

        if (HasThumbnails)
        {
            if (HasDocuments)
            {
                return "icons8-document-attached-images-16";
            }
            else
            {
                return "icons8-document-images-16";
            }
        }
        else
        {
            if (HasDocuments)
            {
                return "icons8-document-attached-16";
            }
        }

        return "icons8-document-16";
    }

    protected Dictionary<PropertyInfo, object?> BackUp()
    {
        var dict = new Dictionary<PropertyInfo, object?>();
        var itemProperties = GetType().GetTypeInfo().DeclaredProperties;

        foreach (var pDescriptor in itemProperties)
        {

            if (pDescriptor.CanWrite)
            {
                dict.Add(pDescriptor, pDescriptor.GetValue(this));
            }
        }

        return dict;
    }
}
