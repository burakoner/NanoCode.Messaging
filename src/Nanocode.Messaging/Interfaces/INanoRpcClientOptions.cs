using System.Collections.Generic;

namespace Nanocode.Messaging.Interfaces
{
    public interface INanoRpcClientOptions
    {
        public string RoutingKey { get; set; }
    }
}
