using Nanocode.Messaging.Enums;
using System.Collections.Generic;

namespace Nanocode.Messaging.Interfaces
{
    public interface INanoRpcResponse
    {
        public string RequestId { get; set; }
        public string RequestMethod { get; set; }
        public string ResponseJson { get; set; }
        public object ResponseObject { get; set; }
        public Dictionary<string, object> ResponseArguments { get; set; }
        public NanoRpcResponseStatus ResponseStatus { get; set; }
    }

    public enum NanoRpcResponseStatus
    {
        Success = 1,
        Timeout = 2,
    }
}
