namespace Eca.Commons.DI
{
    /// <summary>
    /// Marker interface to be <b>inherited</b> by an <b>interface</b> that wants to publicize itself as being a
    /// Component that wants to avail itself of auto-registration with the dependency injection container
    /// </summary>
    /// <remarks>
    /// By convention components implementing this interface should be registered with a lifestyle of Per-Web-Request
    /// for web apps and Transient for desktop apps
    /// </remarks>
    public interface IAutoRegisteredComponent {}
}