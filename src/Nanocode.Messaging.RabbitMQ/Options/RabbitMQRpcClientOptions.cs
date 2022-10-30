using NanoCode.Messaging.Interfaces;
using System.Collections.Generic;

namespace NanoCode.Messaging.RabbitMQ.Options
{
    public class RabbitMQRpcClientOptions : INanoRpcClientOptions
    {
        public string RoutingKey { get; set; }
    }
}