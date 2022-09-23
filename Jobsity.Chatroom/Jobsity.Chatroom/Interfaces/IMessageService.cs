using Jobsity.Chatroom.Models;

namespace Jobsity.Chatroom.Interfaces
{
    public interface IMessageService
    {
        Task SaveMessage(Message message);
    }
}
