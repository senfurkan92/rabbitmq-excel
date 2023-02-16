using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Channels;

namespace BLL.QueueServices
{
	public class RabbitMQClientSubscriber : IDisposable
	{
		public readonly IModel _channel;

		public RabbitMQClientSubscriber(RabbitMQClientService rabbitmqClientService)
		{
			_channel = rabbitmqClientService.CreateChannel();
		}

		public void FanoutConsume(string exchangeName, string queueName, AsyncEventHandler<BasicDeliverEventArgs> receivedFunc, bool queueDurable = false, bool queueAutoDelete = false) 
		{
			_channel.QueueDeclare(queueName, queueDurable, false, queueAutoDelete);
			_channel.QueueBind(queueName, exchangeName, string.Empty);

			var consumer = new AsyncEventingBasicConsumer(_channel);
			_channel.BasicConsume(queueName, false, consumer);
			consumer.Received += receivedFunc;
		}

		public void DirectConsume(string queueName, AsyncEventHandler<BasicDeliverEventArgs> receivedFunc)
		{
			var consumer = new AsyncEventingBasicConsumer(_channel);
			_channel.BasicConsume(queueName, false, consumer);
			consumer.Received += receivedFunc;
		}

		/// <summary>
		/// routing key => *.Error.* icinde error gecen hersey
		/// routing key => #.Error sonu error
		/// routing key => Error.# basi error
		/// </summary>
		public void TopicConsume(string exchangeName, string queueName, string routingKey, AsyncEventHandler<BasicDeliverEventArgs> receivedFunc, bool queueDurable = false, bool queueAutoDelete = false)
		{
			_channel.QueueDeclare(queueName, queueDurable, false, queueAutoDelete);
			_channel.QueueBind(queueName, exchangeName, routingKey);

			var consumer = new AsyncEventingBasicConsumer(_channel);
			_channel.BasicConsume(queueName, false, consumer);
			consumer.Received += receivedFunc;
		}

		/// <summary>
		/// x-match==all/any
		/// </summary>
		public void HeadersConsume(string exchangeName, string queueName, Dictionary<string, object> headers, AsyncEventHandler<BasicDeliverEventArgs> receivedFunc, bool queueDurable = false, bool queueAutoDelete = false)
		{
			_channel.QueueDeclare(queueName, queueDurable, false, queueAutoDelete);
			_channel.QueueBind(queueName, exchangeName, string.Empty, headers);

			var consumer = new AsyncEventingBasicConsumer(_channel);
			_channel.BasicConsume(queueName, false, consumer);
			consumer.Received += receivedFunc;
		}

		public void Dispose()
		{
			_channel?.Close();
			_channel?.Dispose();
		}
	}
}
