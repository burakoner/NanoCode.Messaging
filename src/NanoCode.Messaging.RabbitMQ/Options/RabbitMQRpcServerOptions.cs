using NanoCode.Messaging.Interfaces;
using NanoCode.Messaging.Models;
using System;

namespace NanoCode.Messaging.RabbitMQ.Options
{
    public class RabbitMQRpcServerOptions : INanoRpcServerOptions
    {
        public string RoutingKey { get; set; }
        public Func<NanoRpcRequest, NanoRpcResponse> OnRequest { get; set; }
    }
}