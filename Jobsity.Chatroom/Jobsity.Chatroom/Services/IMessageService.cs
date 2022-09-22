using Jobsity.Chatroom.Models;

namespace Jobsity.Chatroom.Services
{
    public interface IMessageService
    {
        Task SaveMessage(Message message);
    }
}
