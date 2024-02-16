//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using DocumentFlow.Common.Reflection;

using System.Linq.Expressions;
using System.Reflection;

namespace DocumentFlow.Common.Extensions;

public static class ReflectionExtensions
{
    public static MemberInfo ToMember<TMapping, TReturn>(this Expression<Func<TMapping, TReturn?>> propertyExpression)
    {
        return ReflectionHelper.GetMember(propertyExpression) ?? throw new NullReferenceException("Параметр memberExpression должен содержать имя поля, но оно не найдено в классе."); ;
    }
}
