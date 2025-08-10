using Arq.Cqrs.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Arq.Cqrs.Providers
{
    public class DbContextProvider : IDbContextProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, Func<IServiceProvider, DbContext>> _factories;
        private readonly ConcurrentDictionary<string, IServiceScope> _scopes = new();

        public DbContextProvider(
            IServiceProvider serviceProvider,
            Dictionary<string, Func<IServiceProvider, DbContext>> factories)
        {
            _serviceProvider = serviceProvider;
            _factories = factories;
        }

        public DbContext GetDbContext(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre del DbContext no puede ser nulo o vacío.", nameof(name));

            if (!_factories.TryGetValue(name, out var factory))
                throw new ArgumentException($"No se encontró un DbContext registrado con el nombre: {name}");

            // Obtiene el contexto por nombre. Crea un scope nuevo si no existe (por operación).
            var scope = _scopes.GetOrAdd(name, _ => _serviceProvider.CreateScope());
            return factory(scope.ServiceProvider);
        }

        // Permite limpiar scopes (usar en UnitOfWork.Dispose o al terminar operación)
        public void DisposeScopeFor(string name)
        {
            if (_scopes.TryRemove(name, out var scope))
            {
                scope.Dispose();
            }
        }

        // Limpia todos los scopes
        public void DisposeAllScopes()
        {
            foreach (var kv in _scopes)
            {
                kv.Value.Dispose();
            }
            _scopes.Clear();
        }

    }
}
