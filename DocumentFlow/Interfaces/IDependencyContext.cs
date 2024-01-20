//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/license/mit-0/
// Source: https://github.com/sefacan/Scrutor.AspNetCore
//-----------------------------------------------------------------------

namespace DocumentFlow.Interfaces;

public interface IDependencyContext
{
    IServiceProvider ServiceProvider { get; }
    T GetService<T>() where T : notnull;
    IEnumerable<T> GetServices<T>();
    object GetService(Type type);
    IEnumerable<object?> GetServices(Type type);
    T GetOrCreateService<T>();
    object GetOrCreateService(Type type);
}