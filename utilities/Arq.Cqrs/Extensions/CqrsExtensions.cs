using Arq.Core;
using Arq.Cqrs.Interfaces;
using Arq.Cqrs.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Arq.Cqrs.Extensions;

public static class CqrsExtensions
{
    public static IServiceCollection AddCQRS(this IServiceCollection services, Action<DbContextProviderBuilder> configure)
    {
        var builder = new DbContextProviderBuilder();
        configure(builder);
        var factories = builder.Build();

        services.AddSingleton<IDbContextProvider>(provider =>
            new DbContextProvider(provider, factories));

        services.AddScoped(typeof(IEFCommandRepository<>), typeof(EFCommandRepository<>));
        services.AddScoped(typeof(IEFQueryRepository<>), typeof(EFQueryRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
