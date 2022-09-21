using Jobsity.Common.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Jobsity.Bot.RabbitMQ
{
    public class Sender
	{
		private readonly IModel _channel;
		private readonly RabbitMQSettings _rabbitSettings;
		
		public Sender(RabbitMQSettings rabbitSettings, IModel channel)
		{
			_channel = channel;
			_rabbitSettings = rabbitSettings;
		}

		public void SendMessage(string message)
		{
			var messageJson = JsonSerializer.Serialize(message);
			var body = Encoding.UTF8.GetBytes(messageJson);
			_channel.BasicPublish(exchange: _rabbitSettings.ExchangeName,
										 routingKey: _rabbitSettings.KeyName,
										 basicProperties: null,
										 body: body);
		}
	}
}
