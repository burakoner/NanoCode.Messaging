using NanoCode.Messaging.RabbitMQ.Enums;
using NanoCode.Messaging.RabbitMQ.Options;
using System;
using System.Diagnostics;

namespace NanoCode.Messaging.RabbitMQ.Examples.Consumer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Consumer";
            Console.WriteLine("Press <ENTER> to start consuming!..");
            Console.ReadLine();

            var counter = 0;
            var limit = 100000;
            var sw = new Stopwatch();
            var options = new RabbitMQNanoBrokerOptions
            {
                Host = "localhost",
                Port = 5672,
                Username = "guest",
                Password = "123456"
            };
            var broker = new RabbitMQNanoBroker(options);
            var session = broker.CreateSession();
            var consumerOptions = new RabbitMQNanoConsumerOptions
            {
                Session = session.Session,
                Label = "new-consumer",

                OnRegistered = (ch, ea) =>
                {
                    Console.WriteLine($"CONSUMER REGISTERED");
                },
                OnUnregistered = (ch, ea) =>
                {
                    Console.WriteLine($"CONSUMER UNREGISTERED");
                },
                OnShutdown = (ch, ea) =>
                {
                    Console.WriteLine($"CONSUMER SHUTDOWN");
                },
                OnReceived = (ch, ea) =>
                {
                    /*
                    var data = Encoding.UTF8.GetString(ea.Body.ToArray());
                    Console.WriteLine($"Received Data : {data}");
                    */

                    if (counter % limit == 0) sw.Start();
                    counter++;

                    if (counter % limit == 0)
                    {
                        sw.Stop();
                        Console.WriteLine($"[{counter / limit}] Received {limit} messages in {sw.Elapsed}");
                        sw.Reset();
                    }
                },
            };
            var consumingOptions = new RabbitMQNanoConsumingOptions
            {
                Session = session.Session,
                ExchangeName = "new-exchange",
                ExchangeType = RabbitMQExchangeType.Direct,

                QueueName = "new-queue",
                Durable = true,
                Exclusive = false,
                AutoDelete = false,
                AutoAcknowledgement = true,

                RoutingKey = "new-route",
            };
            broker.PrepareConsuming(consumerOptions, consumingOptions);
            broker.StartConsuming(consumerOptions.Label, consumingOptions);

            Console.ReadLine();
            Console.WriteLine("Done!");
        }
    }
}