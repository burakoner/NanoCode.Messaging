using NanoCode.Messaging.RabbitMQ.Options;
using System;

namespace NanoCode.Messaging.RabbitMQ.Examples.RpcClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "RPc Client";
            Console.WriteLine("Press <ENTER> to start RPC requests!..");
            Console.ReadLine();

            var options = new RabbitMQNanoBrokerOptions
            {
                Host = "localhost",
                Port = 5672,
                Username = "guest",
                Password = "guest"
            };
            var broker = new RabbitMQNanoBroker(options);
            var session = broker.CreateSession();

            broker.CreateRpcClient(new RabbitMQNanoRpcClientOptions
            {
                Label = "Rpc-Server-01",
                Session = session.Session,
                RoutingKey = "rpc-01-route",
            });
            for (var i = 0; i < 50; i++)
            {
                var response = broker.RpcClientCallAsync("Rpc-Server-01", new Messaging.Models.NanoRpcRequest
                {
                    Method = $"Method {i}"
                }).Result;

                Console.WriteLine($"[Request {i}] => {response.Method}");
            }

            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}