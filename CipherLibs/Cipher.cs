namespace CipherLibs;

public abstract class Cipher : ICipher
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
        _key = CreateKey(size);
    }

    protected abstract byte[] CreateKey(long size);

    public abstract List<int> Encrypt(int[] plainText);
    public abstract List<int> Decrypt(int[] cipherText);
}