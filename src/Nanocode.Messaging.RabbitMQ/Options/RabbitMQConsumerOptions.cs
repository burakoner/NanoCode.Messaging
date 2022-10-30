namespace Nanocode.Messaging.RabbitMQ.Options
{
    public class RabbitMQConsumerOptions : INanoConsumerOptions
    {
        public string Identifier { get; internal set; }

        public string ExchangeName { get; set; }
        public string ExchangeType { get; set; }

        public string QueueName { get; set; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public bool AutoAcknowledgement { get; set; }

        public string RoutingKey { get; set; }

        public IDictionary<string, object> Arguments { get; set; }

        public EventHandler<BasicDeliverEventArgs> OnReceived { get; set; }
        public EventHandler<ConsumerEventArgs> OnRegistered { get; set; }
        public EventHandler<ConsumerEventArgs> OnUnregistered { get; set; }
        public EventHandler<ShutdownEventArgs> OnShutdown { get; set; }
    }
}