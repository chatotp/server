using System.Text;

namespace CipherLibs.CipherFiles;

internal class Utils
{
    internal static long GetFileSize(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Invalid File Path");
            Environment.Exit(1);
        }

        return new FileInfo(filePath).Length;
    }

    internal static byte[] GetKeyFromFile(string keyPath)
    {
        if (!File.Exists(keyPath))
        {
            Console.WriteLine("Invalid Key File Path");
            Environment.Exit(1);
        }

        using (var reader = new StreamReader(keyPath))
        {
            reader.ReadLine();
            return Convert.FromBase64String(reader.ReadLine()!);
        }
    }
    
    internal static string GetPadding(string originalFilePath, string keyPath)
    {
        // get hashcode of file name
        string fileName = Path.GetFileName(originalFilePath);
        int fileHashCode = fileName.GetHashCode();
        string hashcode = Convert.ToString(fileHashCode);

        // split hashcode into 2 halves
        var (half1, half2) = (hashcode.Substring(0, hashcode.Length / 2),
            hashcode.Substring(hashcode.Length / 2));
        var (base64Half1, base64Half2) = (Convert.ToBase64String(Encoding.UTF8.GetBytes(half1)),
            Convert.ToBase64String(Encoding.UTF8.GetBytes(half2)));

        // get file size of key
        string keyFileSize = Convert.ToString(new FileInfo(keyPath).Length);

        // alternate each character of base64half1 with base64half2
        var sb = new StringBuilder();
        int length = Math.Max(base64Half1.Length, Math.Max(keyFileSize.Length, base64Half2.Length));
        for (int i = 0; i < length; i++)
        {
            if (i < base64Half1.Length)
            {
                sb.Append(base64Half1[i]);
            }

            if (i < keyFileSize.Length)
            {
                sb.Append(keyFileSize[i]);
            }

            if (i < base64Half2.Length)
            {
                sb.Append(base64Half2[i]);
            }
        }

        return sb.ToString();
    }
}