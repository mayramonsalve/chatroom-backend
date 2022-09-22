using Jobsity.Chatroom.Models;

namespace Jobsity.Chatroom.Services
{
    public class MessageService : IMessageService
    {
        private readonly DataContext _dataContext;

        public MessageService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task SaveMessage(Message message)
        {
            message.Id = _dataContext.Messages.Count() + 1;
            await _dataContext.Messages.AddAsync(message);
            _dataContext.SaveChanges();
        }
    }
}
