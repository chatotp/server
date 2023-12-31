using System.Security.Cryptography;
using System.Text;

namespace CipherLibs.Ciphers;

public class XorCipher : Cipher
{
    public XorCipher(byte[] key) : base(key)
    {
    }

    public XorCipher(long size) : base(size)
    {
    }

    protected override byte[] CreateKey(long size)
    {
        // use a random number between size and 3 * size
        // to set size for key
        long newSize = new Random().NextInt64(size, 3 * size);

        // use RandomNumberGenerator to generate a random key
        // since it is available on all platforms
        // unlike RNGCryptoServiceProvider (deprecated too)
        byte[] key = new byte[newSize];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(key);
        }

        return key;
    }

    public override List<int> Encrypt(int[] plainText)
    {
        List<int> cipherText = new();
        for (int i = 0; i < plainText.Length; i++)
        {
            int charCode = plainText[i];
            charCode ^= Key[i % Key.Length];
            cipherText.Add(charCode);
        }

        return cipherText;
    }

    public override List<int> Decrypt(int[] cipherText)
    {
        return this.Encrypt(cipherText);
    }
}