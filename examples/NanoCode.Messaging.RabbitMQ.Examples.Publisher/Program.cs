using System.Diagnostics;
using System;
using NanoCode.Messaging.RabbitMQ.Options;
using NanoCode.Messaging.RabbitMQ.Enums;

namespace NanoCode.Messaging.RabbitMQ.Examples.Publisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Publisher";
            Console.WriteLine("Press <ENTER> to start publishing!..");
            Console.ReadLine();

            var options = new RabbitMQNanoBrokerOptions
            {
                Host = "localhost",
                Port = 5672,
                Username = "guest",
                Password = "123456"
            };
            var broker = new RabbitMQNanoBroker(options);
            var session = broker.CreateSession();
            var publisherOptions = new RabbitMQNanoPublisherOptions();
            var publishingOptions = new RabbitMQNanoPublishingOptions
            {
                Session = session.Session,
                ExchangeName = "new-exchange",
                ExchangeType = RabbitMQExchangeType.Direct,

                QueueName = "new-queue",
                Durable = true,
                Exclusive = false,
                AutoDelete = false,

                RoutingKey = "new-route"
            };
            broker.PreparePublishing(publisherOptions, publishingOptions);
            for (var i = 1; i <= 30; i++)
            {
                var limit = 100000;
                var sw = new Stopwatch();
                sw.Start();
                for (var j = 1; j <= limit; j++)
                {
                    // broker.Publish(publishingOptions, new byte[] { 1, 5, 8, 145 });
                    broker.Publish(publishingOptions, "DATA");
                    // broker.Publish(publishingOptions, new {abc ="123", def="456"});
                }
                sw.Stop();
                Console.WriteLine($"[{i}] Published {limit} messages in {sw.Elapsed}");
            }

            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}