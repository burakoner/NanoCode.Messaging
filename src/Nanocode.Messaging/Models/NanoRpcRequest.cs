using NanoCode.Messaging.Interfaces;
using System.Collections.Generic;

namespace NanoCode.Messaging.Models
{
    public class NanoRpcRequest
    {
        public string Id { get; set; }
        public string Method { get; set; }
        public Dictionary<string, object> Arguments { get; set; } = new();
    }
}
