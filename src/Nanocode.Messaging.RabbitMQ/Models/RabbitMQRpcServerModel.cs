namespace Nanocode.Messaging.RabbitMQ.Models
{
    public class RabbitMQRpcServerModel
    {
        public string Identifier { get; internal set; }
        public string RoutingKey { get; set; }
        public string Tag { get; internal set; }
        public EventingBasicConsumer Consumer { get; internal set; }
    }
}