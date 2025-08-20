using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Arq.Cqrs.Providers;

public class DbContextProviderBuilder
{
    private readonly Dictionary<string, Func<IServiceProvider, DbContext>> _factories = [];

    public void AddContext<TContext>(string name) where TContext : DbContext
    {
        _factories[name] = sp => sp.GetRequiredService<TContext>();
    }

    internal Dictionary<string, Func<IServiceProvider, DbContext>> Build() => _factories;
}
