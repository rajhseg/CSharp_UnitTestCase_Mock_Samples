using Microsoft.AspNetCore.SignalR;

namespace WebBlazorApp.Hubs
{
    public class ChatHub : Hub
    {
        static Dictionary<string, string> _users = new Dictionary<string, string>();

        public void Connect(string username)
        {
            var connectionId = this.Context.ConnectionId;
            _users.Add(connectionId, username);
        }

        public void SendMessage(string message)
        {
            var username = _users[this.Context.ConnectionId];
            this.Clients.Others.SendAsync("ReceiveMessage",username, message);
        }
    }
}
