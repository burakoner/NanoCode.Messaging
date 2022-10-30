namespace Nanocode.Messaging.RabbitMQ.Models
{
    public class RabbitMQConsumerModel
    {
        public string Tag { get; internal set; }
        public string Identifier { get; internal set; }
        public string QueueName { get; internal set; }
        public bool AutoAcknowledgement { get; internal set; }
        public EventingBasicConsumer Consumer { get;internal set; }
        public EventHandler<BasicDeliverEventArgs> OnReceived { get; internal set; }
        public EventHandler<ConsumerEventArgs> OnRegistered { get; internal set; }
        public EventHandler<ConsumerEventArgs> OnUnregistered { get; internal set; }
        public EventHandler<ShutdownEventArgs> OnShutdown { get; internal set; }
    }
}