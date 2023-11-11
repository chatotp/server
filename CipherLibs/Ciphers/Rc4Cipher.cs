namespace CipherLibs.Ciphers;

public class Rc4Cipher : XorCipher, ICipher
{
    public Rc4Cipher(byte[] key) : base(key)
    {
    }

    public Rc4Cipher(long size) : base(size)
    {
    }

    // <summary>
    // This function returns permutation
    // using KSA (Key Scheduling Algorithm)
    // <param name="key">Key</param>
    // </summary>
    protected override byte[] CreateKey(long size)
    {
        var (i, j) = (0L, 0L);
        byte[] permut = GetPermutation(size);
        byte[] key = new byte[size];

        for (int k = 0; k < size; k++)
        {
            i = (i + 1) % size;
            j = (j + permut[i]) % size;
            (permut[i], permut[j]) = (permut[j], permut[i]);
            key[k] = permut[(permut[i] + permut[j]) % size];
        }

        return key;
    }

    private byte[] GetPermutation(long size)
    {
        var random = new Random();
        // initialize identity permutation
        byte[] permut = new byte[size];
        byte[] key = new byte[size];
        for (int i = 0; i < size; i++)
        {
            permut[i] = (byte)random.NextInt64(0, size);
            key[i] = (byte)random.NextInt64(0, size);
        }

        long j = 0;
        for (int i = 0; i < size; i++)
        {
            j = (j + permut[i] + key[i % key.Length]) % size;
            (permut[i], permut[j]) = (permut[j], permut[i]);
        }

        return permut;
    }
}