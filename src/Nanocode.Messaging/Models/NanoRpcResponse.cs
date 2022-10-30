using NanoCode.Messaging.Interfaces;
using System.Collections.Generic;

namespace NanoCode.Messaging.Models
{
    public class NanoRpcResponse
    {
        public string RequestId { get; set; }
        public string RequestMethod { get; set; }
        public object ResponseObject { get; set; }
        public NanoRpcResponseStatus ResponseStatus { get; set; }
    }

    public enum NanoRpcResponseStatus
    {
        Success = 1,
        Timeout = 2,
    }
}
