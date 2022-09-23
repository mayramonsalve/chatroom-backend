using Jobsity.Chatroom.DTOs;
using Jobsity.Chatroom.Hubs;
using Jobsity.Chatroom.Interfaces;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using Moq;
using HubCallerContext = Microsoft.AspNetCore.SignalR.HubCallerContext;
using IClientProxy = Microsoft.AspNetCore.SignalR.IClientProxy;

namespace Jobsity.Tests
{
    public class ChatroomTests
    {
        ChatHub _hub;
        IDictionary<string, UserConnection> _connections;
        IList<string> _users;
        IMessageService _messageService;
        Mock<IGroupManager> _mockGroups;
        Mock<IHubCallerClients> _mockClients;
        Mock<IClientProxy> _mockClientProxy;
        Mock<HubCallerContext> _mockClientContext;

        public ChatroomTests()
        {
            string connectionId = Guid.NewGuid().ToString();
            _connections = new Dictionary<string, UserConnection>() { { connectionId,
                                                                        new UserConnection() { User = "TestUser", Room = "TestRoom" } 
                                                                    }  };
            _users = new List<string>() { "Jobsity", "TestUser", "Mayra" };
            _messageService = null;
            //List<string> groupIds = new List<string>()
            //{
            //    Guid.NewGuid().ToString(),
            //    Guid.NewGuid().ToString(),
            //};
            //List<string> clientIds = new List<string>() { "1", "2", "3" };

            _mockClients = new Mock<IHubCallerClients>();
            //_mockGroups = new Mock<IGroupManager>();// new Mock<IClientContract>();
            _mockClientProxy = new Mock<IClientProxy>();
            _mockClientContext = new Mock<HubCallerContext>();
            _mockClients.Setup(c => c.Group("Test Group")).Returns(_mockClientProxy.Object);
            _mockClientContext.Setup(c => c.ConnectionId).Returns(connectionId);

            //_mockClients.Setup(client => client.All).Returns(_mockClientProxy.Object);
            //_mockClients.Setup(client => client.OthersInGroup(It.IsIn<string>(groupIds))).Returns(_mockClientProxy.Object);
            //_mockGroups.Setup(group => group.AddToGroupAsync(It.IsIn<string>(clientIds), It.IsIn<string>(groupIds), new System.Threading.CancellationToken())).Returns(Task.FromResult(true));
            //_mockGroups.Setup(group => group.RemoveFromGroupAsync(It.IsIn<string>(clientIds), It.IsIn<string>(groupIds), new System.Threading.CancellationToken())).Returns(Task.FromResult(true));

            _hub = new ChatHub(_messageService, _connections, _users)
            {
                Clients = _mockClients.Object,
                Context = _mockClientContext.Object//,
                //Groups = new Mock<IGroupManager>().Object
            };
            //_hub.Groups.AddToGroupAsync(connectionId, "TestGroup");
        }

        //public interface IClientContract
        //{
        //    void SendMessage(UserMessage userMessage);
        //}

        //[Fact]
        //public async Task Hub_SendMessage_ShouldReturn1Message()
        //{
        //    await _hub.SendMessage(new UserMessage() { Message = "TestMessage", Date = DateTime.Now });

        //    _mockClients.Verify(clients => clients.All, Times.Once);

        //    _mockClientProxy.Verify(
        //        clientProxy => clientProxy.SendCoreAsync(
        //            "ReceiveMessage",
        //            It.Is<object[]>(o => o != null && o.Length == 1),
        //            default(CancellationToken)),
        //        Times.Once);
        //}

        //[Fact]
        //public async Task Hub_SendUsers_ShouldReturn1User()
        //{
        //    await _hub.SendUsers("TestRoom");

        //    _mockClients.Verify(clients => clients.All, Times.Once);

        //    _mockClientProxy.Verify(
        //        clientProxy => clientProxy.SendCoreAsync(
        //            "UsersInRoom",
        //            It.Is<object[]>(o => o != null && o.Length == 1),
        //            default(CancellationToken)),
        //        Times.Once);
        //}

        [Fact]
        public async Task Welcome_ShouldReturn1Message1()
        {
            // arrange
            Mock<IHubCallerClients> mockClients = new Mock<IHubCallerClients>();
            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();

            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);

            ChatHub chatHub = new ChatHub(null, null, null)
            {
                Clients = mockClients.Object
            };

            // act
            await chatHub.Welcome();

            // assert
            mockClients.Verify(clients => clients.All, Times.Once);

            mockClientProxy.Verify(
                clientProxy => clientProxy.SendCoreAsync(
                    "Welcome",
                    It.Is<object[]>(o => o != null && o.Length == 1 && ((string)o[0]) == "Welcome!"),
                    default(CancellationToken)),
                Times.Once);
        }
    }
}