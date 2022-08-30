using NanoCode.Messaging.Interfaces;
using NanoCode.Messaging.RabbitMQ.Enums;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace NanoCode.Messaging.RabbitMQ.Models
{
    public class RabbitMQNanoRpcServerModel : INanoRpcServerModel
    {
        public string Label { get; internal set; }
        public string RoutingKey { get; set; }
        public IModel Session { get; set; }
        public string Tag { get; internal set; }
        public EventingBasicConsumer Consumer { get; internal set; }
    }
}