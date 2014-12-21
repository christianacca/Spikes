namespace Eca.Commons.DomainLayer
{
    public interface ICanStopValidating
    {
        void DisableValidation();
        void EnableValidation();
        bool IsValidationEnabled { get; }
    }
}