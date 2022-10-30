namespace Nanocode.Messaging.RabbitMQ.Options
{
    public class RabbitMQBrokerOptions : INanoBrokerOptions
    {
        public string ConnectionString
        {
            get
            {
                return $"amqp://{Username}:{Password}@{Host}:{Port}";
            }
        }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
