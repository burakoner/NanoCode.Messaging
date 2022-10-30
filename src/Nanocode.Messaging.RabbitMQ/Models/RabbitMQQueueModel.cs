namespace Nanocode.Messaging.RabbitMQ.Models
{
    public class RabbitMQQueueModel
    {
        public string QueueName { get; internal set; }
        public bool Durable { get; internal set; }
        public bool Exclusive { get; internal set; }
        public bool AutoDelete { get; internal set; }
        public IDictionary<string, object> Arguments { get; internal set; }

        public QueueDeclareOk QueueDeclare { get; internal set; }
    }
}