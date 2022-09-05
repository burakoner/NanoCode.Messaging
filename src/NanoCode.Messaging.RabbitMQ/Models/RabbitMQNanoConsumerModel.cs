using NanoCode.Messaging.Interfaces;
using NanoCode.Messaging.RabbitMQ.Enums;
using NanoCode.Messaging.RabbitMQ.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace NanoCode.Messaging.RabbitMQ.Models
{
    public class RabbitMQNanoConsumerModel : INanoSessionModel
    {
        public RabbitMQNanoConsumerOptions ConsumerOptions { get; internal set; }
        public RabbitMQNanoConsumingOptions ConsumingOptions { get; internal set; }
        public EventingBasicConsumer Consumer { get;internal set; }
        public string Label { get;internal set; }
        public string Tag { get;internal set; }
    }
}