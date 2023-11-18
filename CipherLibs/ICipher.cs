namespace CipherLibs;

internal interface ICipher
{
    byte[] Key { get; }
    List<int> Encrypt(int[] plainText);
    List<int> Decrypt(int[] cipherText);
}