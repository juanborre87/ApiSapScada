using Application.Interfaces;
using Arq.Core;
using Arq.Cqrs;
using Arq.Cqrs.Extensions;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(
            options => options.UseSqlServer(configuration.GetConnectionString("SapScada")),
            ServiceLifetime.Scoped);
        services.AddHttpClient<SapService>();
        services.AddTransient<ISapService, SapService>();

        //services.AddTransient<ICommandSqlDB<SolicitudPagoEntity>, CommandSqlDB<SolicitudPagoEntity>>();
        //services.AddTransient<IQuerySqlDB<SolicitudPagoEntity>, QuerySqlDB<SolicitudPagoEntity>>();
        services.AddCQRS(builder =>
        {
            builder.AddContext<ApplicationDbContext>("SapScada");
        });
        services.AddSingleton<IFileLogger, FileLogger>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IEFCommandRepository<>), typeof(EFCommandRepository<>));
        services.AddScoped(typeof(IEFQueryRepository<>), typeof(EFQueryRepository<>));

        return services;
    }

}