namespace CipherLibs;

public abstract class CipherFile
{
    private readonly byte[] _key;

    public byte[] Key
    {
        get => _key;
    }

    public CipherFile(byte[] key)
    {
        _key = key;
    }

    public CipherFile(long size)
    {
        _key = GetKey(size);
    }

    protected abstract byte[] GetKey(long size);
    public abstract void Encrypt();
    public abstract void Decrypt();
}