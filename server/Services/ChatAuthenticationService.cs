using CipherLibs;
using CipherLibs.Ciphers;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json;
using server.Models;
using System.Security.Cryptography;

namespace server.Services
{
    public class ChatAuthenticationService
    {
        private DbControllerService _controllerService;

        public ChatAuthenticationService(IConfiguration configuration)
        {
            _controllerService = new DbControllerService(configuration);
        }

        public async Task<bool> AuthenticateAsync(string connId, string user, int algorithm)
        {
            // Send key to client
            await _controllerService.CreateUser(connId, user, algorithm);
            return true;
        }

        // TODO: This is probably broken. Fix this.
        public async Task<bool> IsUserAuthenticatedAsync(string connId)
        {
            return await _controllerService.CheckIfUserExists(connId);
        }

        public async Task DisconnectAsync(string connId)
        {
            await _controllerService.RemoveUser(connId);
        }

        public ChatUser? GetUserInfo(string connId)
        {
            return _controllerService.GetUserInfo(connId);
        }

        public List<int> EncryptMsg(string connId, byte[] key, int algorithm, int[] msg)
        {
            Cipher cipher;

            // TODO: Get this info from Redis Cache
            if (algorithm == 3)
            {
                cipher = new XorCipher(key);
            }
            else if (algorithm == 4)
            {
                cipher = new Rc4Cipher(key);
            }
            else
            {
                throw new InvalidDataException("Invalid Algorithm Format");
            }

            return cipher.Encrypt(msg);
        }

        public List<int> DecryptMsg(string connId, int[] msg)
        {
            var userInfo = GetUserInfo(connId);
            var (key, algorithm) = (userInfo.Key, userInfo.Algorithm);
            Cipher cipher;

            // TODO: Get this info from Redis Cache
            if (algorithm == 3)
            {
                cipher = new XorCipher(key);
            }
            else if (algorithm == 4)
            {
                cipher = new Rc4Cipher(key);
            }
            else
            {
                throw new InvalidDataException("Invalid Algorithm Format");
            }

            return cipher.Decrypt(msg);
        }

        public List<ChatUser> GetCurrentUsers()
        {
            return _controllerService.GetCurrentUsers();
        }

        public List<ChatUser> GetCurrentUsersExceptCaller(string connId)
        {
            return _controllerService.GetCurrentUsersExceptCaller(connId);
        }
    }
}
