using System.Linq;

namespace Eca.Commons.DomainLayer
{
    public interface ILinqRepository<TEntity, TId> : IFind<TEntity, TId>, IQueryable<TEntity> where TEntity : class
    {
        int Count();
    }
}