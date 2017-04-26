namespace SharperArchitecture.Common.Specifications
{
    public interface ICryptography
    {
        string Encrypt(string value);

        bool IsDecryptSupported { get; }

        string Decrypt(string value);
    }
}
