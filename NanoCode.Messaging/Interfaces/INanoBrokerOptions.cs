using System;
using System.Collections.Generic;
using System.Text;

namespace NanoCode.Messaging.Interfaces
{
    public interface INanoBrokerOptions
    {
        public string ConnectionString { get; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
