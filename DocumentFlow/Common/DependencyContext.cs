//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/license/mit-0/
// Source: https://github.com/sefacan/Scrutor.AspNetCore
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace DocumentFlow.Common;

public class DependencyContext : IDependencyContext
{
    public IServiceProvider ServiceProvider { get; private set; }

    public DependencyContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public T GetService<T>() where T : notnull
    {
        return ServiceProvider.GetRequiredService<T>();
    }

    public IEnumerable<T> GetServices<T>()
    {
        return ServiceProvider.GetServices<T>();
    }

    public object GetService(Type type)
    {
        return ServiceProvider.GetRequiredService(type);
    }

    public IEnumerable<object?> GetServices(Type type)
    {
        return ServiceProvider.GetServices(type);
    }

    public T GetOrCreateService<T>()
    {
        return ActivatorUtilities.GetServiceOrCreateInstance<T>(ServiceProvider);
    }

    public object GetOrCreateService(Type type)
    {
        return ActivatorUtilities.GetServiceOrCreateInstance(ServiceProvider, type);
    }
}
