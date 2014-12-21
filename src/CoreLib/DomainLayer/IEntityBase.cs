namespace Eca.Commons.DomainLayer
{
    public interface IEntityBase : IExposesId, INew, INullable, ICanStopValidating {}



    public interface IEntity<T> : IExposesId<T>, INew, INullable, ICanStopValidating {}
}