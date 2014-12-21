namespace Eca.Commons.DomainLayer
{
    public interface IUser
    {
        string Username { get; }
        bool Authenticated { get; }
    }
}