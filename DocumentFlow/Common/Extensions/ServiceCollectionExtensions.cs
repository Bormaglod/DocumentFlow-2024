//-----------------------------------------------------------------------
// Copyright © 2010-2024 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/license/mit-0/
// Source: https://github.com/sefacan/Scrutor.AspNetCore
//-----------------------------------------------------------------------

using DocumentFlow.Interfaces;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Scrutor;

namespace DocumentFlow.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services,
                                                 Action<IDatabase> configureDatabase,
                                                 ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configureDatabase == null) throw new ArgumentNullException(nameof(configureDatabase));

        IDatabase database = new Database();

        configureDatabase(database);

        database.Build();

        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                services.TryAddSingleton(_ => database);
                break;
            case ServiceLifetime.Scoped:
                services.TryAddScoped(_ => database);
                break;
            case ServiceLifetime.Transient:
                services.TryAddTransient(_ => database);
                break;
        }

        return services;
    }

    public static IServiceCollection AddAdvancedDependencyInjection(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromDependencyContext(Microsoft.Extensions.DependencyModel.DependencyContext.Default)
            .AddClassesFromInterfaces());

        return services.AddCommonServices();
    }

    private static IImplementationTypeSelector AddClassesFromInterfaces(this IImplementationTypeSelector selector)
    {
        //singleton
        selector
            .AddClasses(classes => classes.AssignableTo<ISingletonLifetime>(), true)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsMatchingInterface()
            .WithSingletonLifetime()

            .AddClasses(classes => classes.AssignableTo<ISelfSingletonLifetime>(), true)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsSelf()
            .WithSingletonLifetime()

            //transient
            .AddClasses(classes => classes.AssignableTo<ITransientLifetime>(), true)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsMatchingInterface()
            .WithTransientLifetime()

            .AddClasses(classes => classes.AssignableTo<ISelfTransientLifetime>(), true)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsSelf()
            .WithTransientLifetime()

            //scoped
            .AddClasses(classes => classes.AssignableTo<IScopedLifetime>(), true)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsMatchingInterface()
            .WithScopedLifetime()

            .AddClasses(classes => classes.AssignableTo<ISelfScopedLifetime>(), true)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsSelf()
            .WithScopedLifetime();

        return selector;
    }

    private static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        services.TryAddSingleton<IDependencyContext, DependencyContext>();
        services.TryAddSingleton(services);

        return services;
    }
}
