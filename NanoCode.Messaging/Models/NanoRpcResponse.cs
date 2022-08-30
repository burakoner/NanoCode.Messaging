﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using NanoCode.Messaging.Interfaces;

namespace NanoCode.Messaging.Models
{
    public class NanoRpcResponse : INanoRpcResponse
    {
        public string Method { get; set; }
        public Dictionary<string, object> Arguments { get; set; }

        public NanoRpcResponse()
        {
            this.Arguments = new Dictionary<string, object>();
        }
    }
}
