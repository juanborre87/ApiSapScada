using Microsoft.EntityFrameworkCore;

namespace Arq.Cqrs.Interfaces
{
    public interface IDbContextProvider
    {
        DbContext GetDbContext(string name);

    }
}
