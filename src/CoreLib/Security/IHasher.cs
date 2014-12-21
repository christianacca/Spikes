namespace Eca.Commons.Security
{
    public interface IHasher
    {
        string GenerateHash(string plainText);
        bool VerifyHash(string plainText, string hashValue);
    }
}