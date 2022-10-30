namespace Nanocode.Messaging.Enums
{
    public enum MessageBroker
    {
        /*
        AmazonSNS,

        AmazonSQS,

        GooglePubSub
        */

        Kafka,

        NATS,

        RabbitMQ,

        Redis,

        ServiceBus,
    }
}
