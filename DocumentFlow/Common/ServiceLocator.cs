//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/license/mit-0/
// Source: https://github.com/sefacan/Scrutor.AspNetCore
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;

namespace DocumentFlow.Common;

public class ServiceLocator
{
    private static IDependencyContext? _dependencyContext;

    public static IDependencyContext Context => _dependencyContext ?? throw new Exception("You should initialize the context before using it.");

    public static void Initialize(IDependencyContext dependencyContext)
    {
        _dependencyContext = dependencyContext;
    }
}