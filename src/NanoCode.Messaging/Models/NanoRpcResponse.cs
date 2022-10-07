using NanoCode.Messaging.Interfaces;
using System.Collections.Generic;

namespace NanoCode.Messaging.Models
{
    public class NanoRpcResponse
    {
        public string RequestId { get; set; }
        public string RequestMethod { get; set; }
        public object Response { get; set; }
    }
}
