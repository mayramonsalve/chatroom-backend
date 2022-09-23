using Jobsity.Chatroom.DTOs;
using Jobsity.Chatroom.Interfaces;
using Jobsity.Chatroom.Models;
using Jobsity.Common.Models;
using Microsoft.AspNetCore.SignalR;

namespace Jobsity.Chatroom.Hubs
{
    public class ChatHub : Hub
    {
        private readonly string _chatBot;
        private readonly IDictionary<string, UserConnection> _connections;
        private readonly IList<string> _users;
        private readonly IMessageService _messageService;

        public ChatHub(IMessageService messageService, IDictionary<string, UserConnection> connections, IList<string> users)
        {
            _messageService = messageService;
            _chatBot = "Jobsity Chat Bot";
            _connections = connections;
            _users = users;
        }

        public async Task JoinRoom(UserConnection userConnection)
        {
            if (_users.Contains(userConnection.User))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);
                _connections[Context.ConnectionId] = userConnection;
                await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _chatBot,
                                    $"{userConnection.User} has joined {userConnection.Room} room.", DateTime.Now);
                await SendUsers(userConnection.Room);
            }
            else
                throw new AppException("User not in the database.");
        }

        public async Task SendMessage(UserMessage userMessage)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                await _messageService.SaveMessage(new Message() { User = userConnection.User, Text = userMessage.Message, Date = userMessage.Date });
                await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", userConnection.User, userMessage.Message, userMessage.Date);
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if(_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                _connections.Remove(Context.ConnectionId);
                Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _chatBot, $"{userConnection.User} has left {userConnection.Room} room.");
                SendUsers(userConnection.Room);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public Task SendUsers(string room)
        {
            var users = _connections.Values.Where(c => c.Room == room).Select(c => c.User);
            return Clients.Group(room).SendAsync("UsersInRoom", users);
        }

        public async Task Welcome()
        {
            await Clients.All.SendAsync("Welcome", "Welcome!");
        }
    }
}
