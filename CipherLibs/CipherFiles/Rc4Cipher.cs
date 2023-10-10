using CipherLibs.Ciphers;

namespace CipherLibs.CipherFiles;

public class Rc4CipherFile : XorCipherFile
{
    public Rc4CipherFile(string filePath) : base(filePath)
    {
    }
    
    public Rc4CipherFile(string encryptedFilePath, string keyPath) : base(encryptedFilePath, keyPath)
    {
    }
    
    protected override byte[] GetKey(long size)
    {
        return new Rc4Cipher(size).Key;
    }
}