using NanoCode.Messaging.Interfaces;
using NanoCode.Messaging.RabbitMQ.Enums;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace NanoCode.Messaging.RabbitMQ.Options
{
    public class RabbitMQNanoExchangeOptions : INanoExchangeOptions
    {
        public IModel Session { get; set; }
        public string ExchangeName { get; set; }
        public RabbitMQExchangeType ExchangeType { get; set; }
        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }
        public IDictionary<string, object> Arguments { get; set; }
    }
}