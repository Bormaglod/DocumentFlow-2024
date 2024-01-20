//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;

using Humanizer;

using System.ComponentModel.DataAnnotations;

namespace DocumentFlow.Models.Entities;

public class DocumentRefs
{
    [Display(AutoGenerateField = false)]
    public long Id { get; set; }

    [Display(AutoGenerateField = false)]
    public Guid OwnerId { get; set; }

    [Display(Name = "Имя файла", Order = 2)]
    public string FileName { get; set; } = string.Empty;

    [Display(Name = "Описание", Order = 1)]
    public string? Note { get; set; }

    [Display(Name = "Размер", Order = 3)]
    public long FileLength { get; set; }

    [Display(AutoGenerateField = false)]
    public string? Thumbnail { get; set; }

    [Display(Name = "Галлерея", Order = 4)]
    public bool ThumbnailExist => !string.IsNullOrEmpty(Thumbnail);

    [Display(AutoGenerateField = false)]
    public string? S3object { get; set; }

    public static string GetBucketForEntity(object entity)
    {
        if (entity is IBucketInfo bucket)
        {
            return bucket.BucketName;
        }

        return entity.GetType().Name.Underscore().Replace('_', '-');
    }
}
