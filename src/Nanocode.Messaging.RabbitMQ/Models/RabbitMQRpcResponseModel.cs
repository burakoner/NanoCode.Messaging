namespace Nanocode.Messaging.RabbitMQ.Models
{
    public class RabbitMQRpcResponseModel : INanoRpcResponse
    {
        public string RequestId { get; set; }

        public string RequestMethod { get; set; }

        public string ResponseJson { get; set; }

        public object ResponseObject { get; set; }

        public Dictionary<string, object> ResponseArguments { get; set; }

        public NanoRpcResponseStatus ResponseStatus { get; set; }
    }
}