using Microsoft.AspNetCore.SignalR;
using server.Services;
using System.IO;

namespace server.Hubs
{
    public class ChatHub : Hub<IChatClient>
    {
        private readonly ChatAuthenticationService _authenticationService;

        public ChatHub(ChatAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task Connect(string user, int algorithm)
        {
            /*if (await _authenticationService.IsUserAuthenticatedAsync(user))
            {
                // TODO: DISCONNECT THE USER
                // PROBABLY NEED BETTER HANDLING
            }*/

            /* Add to Db and store the key */
            await _authenticationService.AuthenticateAsync(Context.ConnectionId, user, algorithm);

            // Get Key
            var userInfo = _authenticationService.GetUserInfo(Context.ConnectionId);
            if (userInfo != null)
            {
                await Clients.Caller.StartKeyCall();
                foreach (int val in userInfo.Key.Select(x => (int)x).ToArray())
                {
                    await Clients.Caller.ReceiveChar(val);
                }
                await Clients.Caller.StopKeyCall();
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            /* TODO: Check if user in Db and remove the user */
            bool conn = await _authenticationService.IsUserAuthenticatedAsync(Context.ConnectionId);
            if (conn)
            {
                await _authenticationService.DisconnectAsync(Context.ConnectionId);
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMsgUnencrypted(string user, string message)
        {
            if (user == null || user.Equals(""))
            {
                user = Context.ConnectionId;
            }

            await Clients.AllExcept(Context.ConnectionId).ReceiveMessage(user, message);
        }

        public async Task SendMsgEncrypted(string user, string messageStr)
        {
            if (user == null || user.Equals(""))
            {
                user = Context.ConnectionId;
            }
            string[] parts = messageStr.Split(',');
            List<int> parsedIntegers = new List<int>();

            foreach (string part in parts)
            {
                if (int.TryParse(part, out int parsedValue))
                {
                    parsedIntegers.Add(parsedValue);
                }
            }

            // Convert the list of parsed integers to an array
            int[] message = parsedIntegers.ToArray();
            var decryptedMsg = _authenticationService.DecryptMsg(Context.ConnectionId, message);
            List<int> encryptedMsg;

            foreach (var userClient in _authenticationService.GetCurrentUsersExceptCaller(Context.ConnectionId))
            {
                encryptedMsg = _authenticationService.EncryptMsg(userClient.Id, userClient.Key, userClient.Algorithm, decryptedMsg.ToArray());
                var id = userClient.Id;
                await Clients.Client(id).StartMessage();
                foreach (int val in encryptedMsg)
                {
                    await Clients.Client(id).ReceiveMessageChar(val);
                }
                await Clients.Client(id).StopMessage(user);
            }
        }

        public async Task SendJoinMsg(string user)
        {
            if (user == null || user.Equals(""))
            {
                user = Context.ConnectionId;
            }

            await Clients.All.JoinMessage(user);
        }

        public async Task SendMsgToCallerUnencrypted(string connId, string user, string message)
        {
            if (user == null || user.Equals(""))
            {
                user = connId;
            }

            await Clients.Caller.ReceiveMessage(user, message);
        }
    }
}
