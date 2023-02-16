using RabbitMQ.Client;

namespace BLL.QueueServices
{
	public class RabbitMQClientService : IDisposable
	{
		private readonly ConnectionFactory _connectionFactory;
		private IConnection _connection;
		private IModel _channel;

		public RabbitMQClientService(ConnectionFactory connectionFactory)
		{
			_connectionFactory= connectionFactory;
		}

		private void Connect()
		{
			if (_connection is null || _connection is { IsOpen: false })
			{ 
				_connection = _connectionFactory.CreateConnection();
			}
		}

		public IModel CreateChannel(uint prefetchSize = 0, ushort prefetchCount = 1, bool global = false)
		{
			Connect();
			if (_channel is null || _channel is { IsOpen: false })
			{
				var channel = _connection.CreateModel();
				channel.BasicQos(prefetchSize, prefetchCount, global);

				return channel;
			}
			return _channel;
		}

		// queue listener olusturur
		public IModel GetFanoutChannel(string exchangeName, bool exchangeDurable = true, bool exchangeAutoDelete = false)
		{
			_channel = CreateChannel();
			_channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, exchangeDurable, exchangeAutoDelete);
			return _channel;
		}

		// queue publisher olusturur
		public IModel GetDirectChannel(string exchangeName, string routingKey, string queueName, bool exchangeDurable = true, bool exchangeAutoDelete = false, bool queueDurable = true, bool queueAutoDelete = false)
		{
			_channel = CreateChannel();
			_channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, exchangeDurable, exchangeAutoDelete);
			_channel.QueueDeclare(queueName, queueDurable, false, queueAutoDelete);
			_channel.QueueBind(queueName, exchangeName, routingKey);
			return _channel;
		}

		// queue listener olusturur
		public IModel GetTopicChannel(string exchangeName, bool exchangeDurable = true, bool exchangeAutoDelete = false)
		{
			_channel = CreateChannel();
			_channel.ExchangeDeclare(exchangeName, ExchangeType.Topic, exchangeDurable, exchangeAutoDelete);
			return _channel;
		}

		// queue listener olusturur
		public IModel GetHeadersChannel(string exchangeName, bool exchangeDurable = true, bool exchangeAutoDelete = false)
		{
			_channel = CreateChannel();
			_channel.ExchangeDeclare(exchangeName, ExchangeType.Topic, exchangeDurable, exchangeAutoDelete);
			return _channel;
		}

		public void Dispose()
		{
			_channel?.Close();
			_channel?.Dispose();
			_channel = default;
			_connection?.Close();
			_connection?.Dispose();
			_connection = default;
		}
	}
}
