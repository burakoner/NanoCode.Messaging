using NanoCode.Messaging.Interfaces;
using NanoCode.Messaging.RabbitMQ.Enums;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace NanoCode.Messaging.RabbitMQ.Options
{
    public class RabbitMQNanoQueueBindingOptions : INanoQueueBindingOptions
    {
        public IModel Session { get; set; }
        public string ExchangeName { get; set; }
        public string QueueName { get; set; }
        public string RoutingKey { get; set; }
        public IDictionary<string, object> Arguments { get; set; }
    }
}