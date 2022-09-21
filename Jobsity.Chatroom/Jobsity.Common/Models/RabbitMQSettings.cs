namespace Jobsity.Common.Models
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; }
        public string ExchangeName { get; set; }
        public string KeyName { get; set; }
        public string ExchangeType { get; set; }
    }
}
