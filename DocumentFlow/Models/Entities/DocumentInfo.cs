//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DocumentFlow.Models.Entities;

public abstract class DocumentInfo : Entity
{
    public Guid UserCreatedId { get; protected set; }
    public DateTime DateCreated { get; protected set; }
    public string? UserCreated { get; protected set; }
    public Guid UserUpdatedId { get; protected set; }
    public DateTime DateUpdated { get; protected set; }
    public string? UserUpdated { get; protected set; }
    public bool Deleted { get; protected set; }
    public bool? HasDocuments { get; protected set; } = false;

    public ImageSource RowStatusImage
    {
        get
        {
            return new BitmapImage(new Uri($"pack://application:,,,/DocumentFlow;component/Images/{GetRowStatusImageName()}.png"));
        }
    }

    protected virtual string GetRowStatusImageName()
    {
        if (Deleted)
        {
            return "icons8-document-delete-16";
        }

        if (HasDocuments == true)
        {
            return "icons8-document-attached-16";
        }

        return "icons8-document-16";
    }
}
