using NanoCode.Messaging.Interfaces;
using NanoCode.Messaging.RabbitMQ.Enums;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace NanoCode.Messaging.RabbitMQ.Options
{
    public class RabbitMQNanoRpcClientOptions : INanoRpcClientOptions
    {
        public string Label { get; set; }
        public IModel Session { get; set; }
        public string RoutingKey { get; set; }
    }
}