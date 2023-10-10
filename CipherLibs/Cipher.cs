namespace CipherLibs;

public abstract class Cipher
{
    private readonly byte[] _key;
    
    public byte[] Key
    {
        get => _key;
    }
    
    public Cipher(byte[] key)
    {
        _key = key;
    }

    public Cipher(long size)
    {
        _key = GetKey(size);
    }

    protected abstract byte[] GetKey(long size);
    public abstract string Encrypt(string plainText);
    public abstract string Decrypt(string cipherText);
}