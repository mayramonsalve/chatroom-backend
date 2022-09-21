using Jobsity.Chatroom.Hubs;
using Jobsity.Common.Models;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Jobsity.Chatroom.RabbitMQ
{
	public class Receiver : IHostedService
	{
		private readonly RabbitMQSettings _rabbitSettings;
		private readonly IModel _channel;
		private readonly IHubContext<ChatHub> _chatHub;
		private readonly string _chatBot;
		public Receiver(RabbitMQSettings rabbitSettings, IModel channel, IHubContext<ChatHub> chatHub)
		{
			_rabbitSettings = rabbitSettings;
			_channel = channel;
			_chatHub = chatHub;
			_chatBot = "Jobsity Chat Bot";
		}


		public Task StartAsync(CancellationToken cancellationToken)
		{
			ReceiveMessage();
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_channel.Dispose();
			return Task.CompletedTask;
		}

		private void ReceiveMessage()
		{
			_channel.ExchangeDeclare(_rabbitSettings.ExchangeName,
				type: _rabbitSettings.ExchangeType);

			var queueName = _channel.QueueDeclare().QueueName;


			_channel.QueueBind(queue: queueName,
							  exchange: _rabbitSettings.ExchangeName,
							  routingKey: _rabbitSettings.KeyName);


			var consumerAsync = new AsyncEventingBasicConsumer(_channel);
			consumerAsync.Received += async (_, ea) =>
			{
				var body = ea.Body.ToArray();
				var message = Encoding.UTF8.GetString(body);
				var jsonMessage = JsonSerializer.Deserialize<string>(message);
				if (!String.IsNullOrEmpty(jsonMessage))
				{
					var messageArray = jsonMessage.Split(":");
					string room = messageArray[0];
					string text = messageArray[1];
					await _chatHub.Clients.Group(room).SendAsync("ReceiveMessage", _chatBot, text, DateTime.Now);
				}

				_channel.BasicAck(ea.DeliveryTag, false);
			};

			_channel.BasicConsume(queue: queueName,
								 autoAck: false,
								 consumer: consumerAsync);
		}
	}
}
