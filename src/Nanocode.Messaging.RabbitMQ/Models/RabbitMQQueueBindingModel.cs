namespace Nanocode.Messaging.RabbitMQ.Models
{
    public class RabbitMQQueueBindingModel
    {
        public string ExchangeName { get; internal set; }
        public string QueueName { get; internal set; }
        public string RoutingKey { get; internal set; }
        public IDictionary<string, object> Arguments { get; internal set; }
    }
}