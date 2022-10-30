using Nanocode.Messaging.RabbitMQ.Enums;
using Nanocode.Messaging.RabbitMQ.Options;
using System;
using System.Diagnostics;

namespace Nanocode.Messaging.RabbitMQ.Examples.Publisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Publisher";
            Console.WriteLine("Press <ENTER> to start publishing!..");
            Console.ReadLine();

            var broker = new RabbitMQBroker(new RabbitMQBrokerOptions
            {
                Host = "localhost",
                Port = 5672,
                Username = "guest",
                Password = "123456"
            });
            var publisherId = broker.CreatePublisher(new RabbitMQPublisherOptions
            {
                Exchange = "new-exchange",
                ExchangeType = RabbitMQExchangeType.Direct,

                Queue = "new-queue",
                Durable = true,
                Exclusive = false,
                AutoDelete = false,

                RoutingKey = "new-route"
            });
            for (var i = 1; i <= 30; i++)
            {
                var limit = 100000;
                var sw = new Stopwatch();
                sw.Start();
                for (var j = 1; j <= limit; j++)
                {
                    // broker.Publish(publishingOptions, new byte[] { 1, 5, 8, 145 });
                    broker.Publish(publisherId, "DATA");
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