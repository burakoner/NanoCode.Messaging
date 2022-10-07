namespace NanoCode.Messaging.RabbitMQ.Enums
{
    public static class RabbitMQExchangeType
    {
        public const string Direct = "direct";
        public const string Fanout = "fanout";
        public const string Headers = "headers";
        public const string Topic = "topic";
    }
}