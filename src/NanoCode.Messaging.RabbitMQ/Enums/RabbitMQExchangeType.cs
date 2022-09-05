using NanoCode.Data.Attributes;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoCode.Messaging.RabbitMQ.Enums
{
    public enum RabbitMQExchangeType
    {
        [EnumLabel("direct")]
        Direct,

        [EnumLabel("fanout")]
        Fanout,

        [EnumLabel("headers")]
        Headers,

        [EnumLabel("topic")]
        Topic,
    }
}