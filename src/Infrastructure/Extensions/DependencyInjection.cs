using Application.Interfaces;
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
            ServiceLifetime.Transient);
        services.AddHttpClient<SapService>();
        services.AddTransient<ISapService, SapService>();

        //services.AddTransient<ICommandSqlDB<SolicitudPagoEntity>, CommandSqlDB<SolicitudPagoEntity>>();
        //services.AddTransient<IQuerySqlDB<SolicitudPagoEntity>, QuerySqlDB<SolicitudPagoEntity>>();
        services.AddTransient(typeof(ICommandSqlDB<>), typeof(CommandSqlDB<>));
        services.AddTransient(typeof(IQuerySqlDB<>), typeof(QuerySqlDB<>));

        return services;
    }

}