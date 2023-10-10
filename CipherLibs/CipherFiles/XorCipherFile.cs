using System.Security.Cryptography;
using System.Text;
using CipherLibs.Ciphers;

namespace CipherLibs.CipherFiles;

public class XorCipherFile : CipherFile, ICipherFile
{
    private readonly string _filePath;
    private readonly string _keyPath;
    private readonly string _encryptedFilePath;

    public XorCipherFile(string filePath) : base(Utils.GetFileSize(filePath))
    {
        string filePathWithoutExtension = string.Join("", filePath.Split('.')[..^1]);
        string keyPath = filePathWithoutExtension + ".otpkey";
        string encryptedFilePath = filePathWithoutExtension + ".otpenc";

        this._filePath = filePath;
        this._keyPath = keyPath;
        this._encryptedFilePath = encryptedFilePath;
    }

    public XorCipherFile(string encryptedFilePath, string keyPath) : base(Utils.GetKeyFromFile(keyPath))
    {
        if (!File.Exists(encryptedFilePath))
        {
            Console.WriteLine("Invalid Encrypted File Path");
            Environment.Exit(1);
        }

        using (var reader = new StreamReader(keyPath))
        {
            string fileNameWithExtension = reader.ReadLine()!;
            string directoryPath = Path.GetDirectoryName(keyPath)!;

            this._filePath = Path.Join(directoryPath, fileNameWithExtension);
        }

        this._encryptedFilePath = encryptedFilePath;
        this._keyPath = keyPath;
    }

    protected override byte[] GetKey(long size)
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

    public override void Encrypt()
    {
        /* Read a file and write to other file simultaneously
         using StreamReader and StreamWriter
         */

        // Output File: Input file name + .otpenc
        // Output Keyfile: Input file name + .otpkey
        int keyLength = Key.Length;

        // save key to .otpkey file
        using (var writer = new StreamWriter(_keyPath))
        {
            writer.WriteLine(Path.GetFileName(_filePath));
            writer.Write(Convert.ToBase64String(Key));
        }

        try
        {
            using (var reader = new StreamReader(_filePath))
            {
                using (var writer = new StreamWriter(_encryptedFilePath))
                {
                    // padding and add to the output file
                    writer.Write(Utils.GetPadding(_filePath, _keyPath));

                    // read the file character by character
                    // encrypt it, and write to the output file
                    int i = 0;
                    while (!reader.EndOfStream)
                    {
                        int charCode = reader.Read();
                        charCode ^= Key[i % keyLength];
                        writer.Write((char)charCode);
                        i++;
                    }
                }
            }
        }
        catch (IOException e)
        {
            Console.WriteLine(e.Message);
            Environment.Exit(1);
        }
    }

    public override void Decrypt()
    {
        try
        {
            using (var reader = new StreamReader(_encryptedFilePath))
            {
                using (var writer = new StreamWriter(_filePath))
                {
                    int paddingLength = Utils.GetPadding(_filePath, _keyPath).Length;
                    int keyLength = Key.Length;
                    // skip padding
                    reader.BaseStream.Seek(paddingLength, SeekOrigin.Begin);

                    // read the file character by character
                    // decrypt it, and write to the output file
                    int i = 0;
                    while (!reader.EndOfStream)
                    {
                        int charCode = reader.Read();
                        charCode ^= Key[i % keyLength];
                        writer.Write((char)charCode);
                        i++;
                    }
                }
            }
        }
        catch (IOException e)
        {
            Console.WriteLine(e.Message);
            Environment.Exit(1);
        }
    }
}