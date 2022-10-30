using System.Collections.Generic;

namespace Nanocode.Messaging.Interfaces
{
    public interface INanoRpcRequest
    {
        public string RequestId { get; set; }
        public string RequestMethod { get; set; }
        public string RequestJson { get; set; }
        public object RequestObject { get; set; }
        public Dictionary<string, object> RequestArguments { get; set; }
    }
}
