namespace server.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessageChar(int charValue);
        Task ReceiveMessage(string user, string message);
        Task JoinMessage(string user);

        Task StartKeyCall();
        Task StopKeyCall();
        Task ReceiveChar(int charValue);

        Task StartMessage();
        Task StopMessage(string user);

        Task ReceiveFile(string user, string fileName, string hash);
    }
}