using CipherLibs;
using Grpc.Core;
using CipherLibs.Ciphers;

namespace OTPChat.Services;

public class EncryptService : OTPChat.EncryptService.EncryptServiceBase
{
    private readonly ILogger<EncryptService> _logger;

    public EncryptService(ILogger<EncryptService> logger)
    {
        _logger = logger;
    }

    public override Task<CipherText> Encrypt(PlainText request, ServerCallContext context)
    {
        // Use the CipherLibs library to encrypt the text
        // check if algorithm given by client is valid
        Cipher encryptionAlg;
        bool hasKey = request.HasKey;
        switch (request.Algorithm)
        {
            case "Xor":
                encryptionAlg = hasKey
                    ? new XorCipher(Convert.FromBase64String(request.Key))
                    : new XorCipher(request.Text.Length);
                break;
            case "Rc4":
                encryptionAlg = hasKey
                    ? new Rc4Cipher(Convert.FromBase64String(request.Key))
                    : new Rc4Cipher(request.Text.Length);
                break;
            default:
                // invalid
                return Task.FromResult(new CipherText());
        }

        // encrypt the text
        var cipherText = encryptionAlg.Encrypt(request.Text);
        var key = Convert.ToBase64String(encryptionAlg.Key);

        // return the encrypted text and the key
        return Task.FromResult(new CipherText
        {
            Text = cipherText,
            Key = key
        });
    }
}