using System;
using System.Collections.Generic;
using System.Text;

namespace NanoCode.Messaging.Interfaces
{
    public interface INanoPublishingOptions
    {
        /// <summary>
        /// Mandatory for RabbitMQ
        /// </summary>
        public string ExchangeName { get; set; }

        /// <summary>
        /// Mandatory for RabbitMQ
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// Mandatory for RabbitMQ
        /// </summary>
        public string RoutingKey { get; set; }

        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
    }
}
