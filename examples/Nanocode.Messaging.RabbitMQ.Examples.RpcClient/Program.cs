using NanoCode.Messaging.RabbitMQ.Options;
using System;

namespace NanoCode.Messaging.RabbitMQ.Examples.RpcClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "RPC Client";
            Console.WriteLine("Press <ENTER> to start sending RPC requests!..");
            Console.ReadLine();

            var broker = new RabbitMQBroker(new RabbitMQBrokerOptions
            {
                Host = "localhost",
                Port = 5672,
                Username = "guest",
                Password = "123456"
            });
            var rpcClientId = broker.CreateRpcClient(new RabbitMQRpcClientOptions
            {
                RoutingKey = "RPC-Route-01"
            });
            for (var i = 0; i < 100; i++)
            {
                var response = broker.RpcClientCallAsync(rpcClientId, new Messaging.Models.NanoRpcRequest
                {
                    Id = i.ToString(),
                    Method = $"Method {i}",
                }).Result;

                Console.WriteLine($"[Request {i}] => {response.RequestId} {response.RequestMethod} {response.Response}");
            }

            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}