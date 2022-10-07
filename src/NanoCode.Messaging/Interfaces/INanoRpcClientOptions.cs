using System.Collections.Generic;

namespace NanoCode.Messaging.Interfaces
{
    public interface INanoRpcClientOptions
    {
        public string RoutingKey { get; set; }
    }
}
