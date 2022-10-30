using System.Collections.Generic;

namespace Nanocode.Messaging.Interfaces
{
    public interface INanoConsumerOptions
    {
        public string ExchangeName { get; set; }
        public string ExchangeType { get; set; }

        public string QueueName { get; set; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public bool AutoAcknowledgement { get; set; }

        public string RoutingKey { get; set; }

        public IDictionary<string, object> Arguments { get; set; }
    }
}
