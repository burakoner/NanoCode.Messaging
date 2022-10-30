using System.Collections.Generic;

namespace Nanocode.Messaging.Interfaces
{
    public interface INanoPublisherOptions
    {
        public string Exchange { get; set; }
        public string ExchangeType { get; set; }

        public string Queue { get; set; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }

        public string RoutingKey { get; set; }

        public IDictionary<string, object> Arguments { get; set; }
    }
}
