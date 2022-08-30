using System;
using System.Collections.Generic;
using System.Text;

namespace NanoCode.Messaging.Interfaces
{
    public interface INanoRpcRequest
    {
        public string Method { get; set; }
        public Dictionary<string, object> Arguments { get; set; }
    }
}
