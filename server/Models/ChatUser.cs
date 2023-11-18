using Microsoft.AspNetCore.Identity;

namespace server.Models
{
    public class ChatUser : IdentityUser
    {
        [PersonalData]
        public string Id { get; set; }
        [PersonalData]
        public string Name { get; set; }
        [ProtectedPersonalData]
        public int Algorithm { get; set; }
        [ProtectedPersonalData]
        public byte[] Key { get; set; }

        public ChatUser(string id, string name, int algorithm, byte[] key)
        {
            Id = id;
            Name = name;
            Algorithm = algorithm;
            Key = key;
        }
    }
}
