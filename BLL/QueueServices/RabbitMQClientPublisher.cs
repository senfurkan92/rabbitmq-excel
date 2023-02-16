using RabbitMQ.Client;
using System.Text;

namespace BLL.QueueServices
{
	public class RabbitMQClientPublisher : IDisposable
	{
		private readonly RabbitMQClientService _rabbitmqClientService;
		private IModel channel;

		public RabbitMQClientPublisher(RabbitMQClientService rabbitmqClientService)
		{
			_rabbitmqClientService= rabbitmqClientService;
		}

		public void FanoutPublish(string msg, string exchangeName, bool exchangeDurable = true, bool exchangeAutoDelete = false)
		{
			channel = _rabbitmqClientService.GetFanoutChannel(exchangeName,exchangeDurable,exchangeAutoDelete);
			channel.BasicPublish(exchangeName, string.Empty, null, Encoding.UTF8.GetBytes(msg));
		}

		public void DirectPublish(string msg, string exchangeName, string routingKey, string queueName, bool exchangeDurable = true, bool exchangeAutoDelete = false, bool queueDurable = true, bool queueAutoDelete = false)
		{
			channel = _rabbitmqClientService.GetDirectChannel(exchangeName,routingKey,queueName,exchangeDurable,exchangeAutoDelete,queueDurable,queueAutoDelete);
			channel.BasicPublish(exchangeName, routingKey, null, Encoding.UTF8.GetBytes(msg));
		}

		public void TopicPublish(string msg, string exchangeName, string routingKey, bool exchangeDurable = true, bool exchangeAutoDelete = false)
		{
			channel = _rabbitmqClientService.GetTopicChannel(exchangeName, exchangeDurable, exchangeAutoDelete);
			channel.BasicPublish(exchangeName, routingKey, null, Encoding.UTF8.GetBytes(msg));
		}

		public void HeadersPublish(string msg, string exchangeName, IDictionary<string,object> headers, bool exchangeDurable = true, bool exchangeAutoDelete = false)
		{
			channel = _rabbitmqClientService.GetHeadersChannel(exchangeName, exchangeDurable, exchangeAutoDelete);
			var properties = channel.CreateBasicProperties();
			properties.Headers = headers;
			properties.Persistent = true;
			channel.BasicPublish(exchangeName, string.Empty, properties, Encoding.UTF8.GetBytes(msg));
		}

		public void Dispose()
		{
			//channel?.Close();
			//channel?.Dispose();
		}
	}
}
