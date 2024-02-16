//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
//-----------------------------------------------------------------------

using System.Linq.Expressions;
using System.Reflection;

namespace DocumentFlow.Common.Reflection;

public static class ReflectionHelper
{
    public static MemberInfo? GetMember<TModel, TReturn>(Expression<Func<TModel, TReturn?>> expression)
    {
        return GetMember(expression.Body);
    }

    private static MemberInfo? GetMember(Expression expression)
    {
        var memberExpression = GetMemberExpression(expression);

        return memberExpression?.Member;
    }

    private static MemberExpression? GetMemberExpression(Expression expression)
    {
        return GetMemberExpression(expression, true);
    }

    private static MemberExpression? GetMemberExpression(Expression expression, bool enforceCheck)
    {
        MemberExpression? memberExpression = null;
        if (expression.NodeType == ExpressionType.Convert)
        {
            var body = (UnaryExpression)expression;
            memberExpression = body.Operand as MemberExpression;
        }
        else if (expression.NodeType == ExpressionType.MemberAccess)
        {
            memberExpression = expression as MemberExpression;
        }

        if (enforceCheck && memberExpression == null)
        {
            throw new ArgumentException("Not a member access", nameof(expression));
        }

        return memberExpression;
    }
}
