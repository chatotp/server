namespace CipherLibs;

internal interface ICipher
{
    byte[] Key { get; }
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}