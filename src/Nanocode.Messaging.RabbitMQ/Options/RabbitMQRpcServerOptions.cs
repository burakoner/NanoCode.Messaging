namespace Nanocode.Messaging.RabbitMQ.Options
{
    public class RabbitMQRpcServerOptions : INanoRpcServerOptions
    {
        public string RoutingKey { get; set; }
        public Func<INanoRpcRequest, INanoRpcResponse> OnRequest { get; set; }
    }
}