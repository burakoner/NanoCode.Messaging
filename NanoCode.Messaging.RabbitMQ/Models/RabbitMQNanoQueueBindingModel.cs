using NanoCode.Messaging.Interfaces;
using NanoCode.Messaging.RabbitMQ.Enums;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace NanoCode.Messaging.RabbitMQ.Models
{
    public class RabbitMQNanoQueueBindingModel : INanoQueueBindingOptions
    {
        public IModel Session { get; internal set; }
        public string ExchangeName { get; internal set; }
        public string QueueName { get; internal set; }
        public string RoutingKey { get; internal set; }
        public IDictionary<string, object> Arguments { get; internal set; }
    }
}