namespace Nanocode.Messaging.RabbitMQ.Options
{
    public class RabbitMQRpcClientOptions : INanoRpcClientOptions
    {
        public string RoutingKey { get; set; }
    }
}