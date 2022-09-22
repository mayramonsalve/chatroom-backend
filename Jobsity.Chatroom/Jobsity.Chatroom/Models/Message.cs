using System.ComponentModel.DataAnnotations;

namespace Jobsity.Chatroom.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public string User { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}
