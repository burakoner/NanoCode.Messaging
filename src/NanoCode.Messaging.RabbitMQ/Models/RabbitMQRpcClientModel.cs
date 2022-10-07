using NanoCode.Messaging.Interfaces;
using NanoCode.Messaging.RabbitMQ.Enums;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace NanoCode.Messaging.RabbitMQ.Models
{
    public class RabbitMQRpcClientModel
    {
        public string Identifier { get; internal set; }
        public string RoutingKey { get; set; }
        public RabbitMQRpcClient Client { get; internal set; }
    }
}