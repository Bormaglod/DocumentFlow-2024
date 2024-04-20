//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Enums;

namespace DocumentFlow.Common.Data;

[AttributeUsage(AttributeTargets.Class)]
public class ProductContentAttribute : Attribute
{
    public ProductContentAttribute(ProductContent content) => Content = content;

    public ProductContent Content { get; }
}
