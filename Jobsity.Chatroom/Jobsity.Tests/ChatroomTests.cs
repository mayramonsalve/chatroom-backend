using Jobsity.Chatroom.DTOs;
using Jobsity.Chatroom.Hubs;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using Moq;
using System.Dynamic;
using HubCallerContext = Microsoft.AspNetCore.SignalR.HubCallerContext;
using IClientProxy = Microsoft.AspNetCore.SignalR.IClientProxy;

namespace Jobsity.Tests
{
    public class ChatroomTests
    {
        ChatHub _hub;
        IDictionary<string, UserConnection> _connections;
        Mock<IClientContract> _mockGroups;
        Mock<IHubCallerClients> _mockClients;
        Mock<IClientProxy> _mockClientProxy;
        Mock<HubCallerContext> _mockClientContext;

        public ChatroomTests()
        {
            string connectionId = Guid.NewGuid().ToString();
            _connections = new Dictionary<string, UserConnection>() { { connectionId,
                                                                        new UserConnection() { User = "Test User", Room = "Test Room" } 
                                                                    }  };
            _mockClients = new Mock<IHubCallerClients>();
            _mockGroups = new Mock<IClientContract>();
            _mockClientProxy = new Mock<IClientProxy>();
            _mockClientContext = new Mock<HubCallerContext>();
            _mockGroups.Setup(g => g.SendMessage(It.IsAny<UserMessage>())).Verifiable();
            _mockClients.Setup(c => c.Group("Test Group")).Returns(_mockClientProxy.Object);
            _mockClientContext.Setup(c => c.ConnectionId).Returns(connectionId);

            _hub = new ChatHub(_connections)
            {
                Clients = _mockClients.Object,
                Context = _mockClientContext.Object,
                Groups = new Mock<IGroupManager>().Object
            };
            _hub.Groups.AddToGroupAsync(connectionId, "Test Group");
        }

        public interface IClientContract
        {
            void SendMessage(UserMessage userMessage);
        }

        //[Fact]
        //public async Task Hub_SendMessage_ShouldReturn1Message()
        //{
        //    await _hub.SendMessage(new UserMessage() { Message = "Test Message", Date = DateTime.Now });

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
        //    await _hub.SendUsers("Test Room");

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

            ChatHub chatHub = new ChatHub(null)
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