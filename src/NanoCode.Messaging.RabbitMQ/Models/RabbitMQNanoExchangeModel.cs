using NanoCode.Messaging.Interfaces;
using NanoCode.Messaging.RabbitMQ.Enums;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace NanoCode.Messaging.RabbitMQ.Models
{
    public class RabbitMQNanoExchangeModel : INanoExchangeOptions
    {
        public IModel Session { get; internal set; }
        public string ExchangeName { get; internal set; }
        public RabbitMQExchangeType ExchangeType { get; internal set; }
        public bool Durable { get; internal set; }
        public bool AutoDelete { get; internal set; }
        public IDictionary<string, object> Arguments { get; internal set; }
    }
}