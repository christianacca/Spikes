namespace Eca.Commons.DomainLayer
{
    public interface IFind<TEntity, TId>
    {
        TEntity GetById(TId id);
        bool Exists(TId id);
    }
}