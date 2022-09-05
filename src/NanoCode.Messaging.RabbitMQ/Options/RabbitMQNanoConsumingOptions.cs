using NanoCode.Messaging.Interfaces;
using NanoCode.Messaging.RabbitMQ.Enums;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace NanoCode.Messaging.RabbitMQ.Options
{
    public class RabbitMQNanoConsumingOptions : INanoConsumingOptions
    {
        public IModel Session { get; set; }

        public string ExchangeName { get; set; }
        public RabbitMQExchangeType ExchangeType { get; set; }

        public string QueueName { get; set; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public bool AutoAcknowledgement { get; set; }

        public string RoutingKey { get; set; }

        public IDictionary<string, object> Arguments { get; set; }
    }
}