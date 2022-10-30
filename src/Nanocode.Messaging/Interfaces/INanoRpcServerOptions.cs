using NanoCode.Messaging.Models;
using System;

namespace NanoCode.Messaging.Interfaces
{
    public interface INanoRpcServerOptions
    {
        public string RoutingKey { get; set; }
        public Func<NanoRpcRequest, NanoRpcResponse> OnRequest { get; set; }
    }
}
