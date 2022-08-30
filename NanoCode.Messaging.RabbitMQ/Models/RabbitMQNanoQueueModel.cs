using NanoCode.Messaging.Interfaces;
using NanoCode.Messaging.RabbitMQ.Enums;
using NanoCode.Messaging.RabbitMQ.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace NanoCode.Messaging.RabbitMQ.Models
{
    public class RabbitMQNanoQueueModel : INanoQueueOptions
    {
        public IModel Session { get; internal set; }
        public RabbitMQNanoQueueOptions Options { get; internal set; }
        public QueueDeclareOk QueueDeclare { get; internal set; }
    }
}