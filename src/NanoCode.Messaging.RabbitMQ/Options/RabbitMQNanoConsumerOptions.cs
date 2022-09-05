using NanoCode.Messaging.Interfaces;
using NanoCode.Messaging.RabbitMQ.Enums;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace NanoCode.Messaging.RabbitMQ.Options
{
    public class RabbitMQNanoConsumerOptions : INanoConsumerOptions
    {
        public string Label { get; set; }
        public IModel Session { get; set; }
        public EventHandler<BasicDeliverEventArgs> OnReceived { get; set; }
        public EventHandler<ConsumerEventArgs> OnRegistered { get; set; }
        public EventHandler<ConsumerEventArgs> OnUnregistered { get; set; }
        public EventHandler<ShutdownEventArgs> OnShutdown { get; set; }
    }
}