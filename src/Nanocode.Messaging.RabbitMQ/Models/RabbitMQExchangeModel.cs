namespace Nanocode.Messaging.RabbitMQ.Models
{
    public class RabbitMQExchangeModel
    {
        public string ExchangeName { get; internal set; }
        public string ExchangeType { get; internal set; }
        public bool Durable { get; internal set; }
        public bool AutoDelete { get; internal set; }
        public IDictionary<string, object> Arguments { get; internal set; }
    }
}