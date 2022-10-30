namespace Nanocode.Messaging.RabbitMQ.Models
{
    public class RabbitMQRpcRequestModel: INanoRpcRequest
    {
        public string RequestId { get; set; }

        public string RequestMethod { get; set; }

        public string RequestJson { get; set; }

        public object RequestObject { get; set; }

        public Dictionary<string, object> RequestArguments { get; set; }
    }
}