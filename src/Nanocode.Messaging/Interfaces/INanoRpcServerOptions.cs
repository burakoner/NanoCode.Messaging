using System;

namespace Nanocode.Messaging.Interfaces
{
    public interface INanoRpcServerOptions
    {
        public string RoutingKey { get; set; }
        public Func<INanoRpcRequest, INanoRpcResponse> OnRequest { get; set; }
    }
}
