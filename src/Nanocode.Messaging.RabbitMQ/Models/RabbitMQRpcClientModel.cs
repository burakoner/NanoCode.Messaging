namespace Nanocode.Messaging.RabbitMQ.Models
{
    public class RabbitMQRpcClientModel
    {
        public string Identifier { get; internal set; }
        public string RoutingKey { get; set; }
        public RabbitMQRpcClient Client { get; internal set; }
    }
}