using Microsoft.EntityFrameworkCore;

namespace Arq.Cqrs.Interfaces;

public interface IDbContextProvider
{
    /// <summary>
    /// Gets a DbContext for the specified name. The provider manages scopes internally
    /// Obtiene un DbContext para el nombre indicado. El provider gestiona scopes internamente
    /// </summary>
    DbContext GetDbContext(string name);

    /// <summary>
    /// Releases the scope associated with the name
    /// Libera el scope asociado al nombre
    /// </summary>
    void DisposeScopeFor(string name);

    /// <summary>
    /// Release all scopes
    /// Libera todos los scopes
    /// </summary>
    void DisposeAllScopes();

}
