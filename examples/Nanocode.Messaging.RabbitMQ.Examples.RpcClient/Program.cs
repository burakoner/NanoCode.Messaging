using Nanocode.Messaging.RabbitMQ.Models;
using Nanocode.Messaging.RabbitMQ.Options;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Nanocode.Messaging.RabbitMQ.Examples.RpcClient
{
    internal class Program
    {
        static async Task Main(string[] args)
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
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < 100; i++)
            {
                var response = await broker.RpcClientCallAsync(rpcClientId, new RabbitMQRpcRequestModel
                {
                    RequestId = i.ToString(),
                    RequestMethod = $"Method {i}",
                    RequestJson = "Request JSON",
                });

                Console.WriteLine($"[Request {i}] => {response.RequestId} {response.RequestMethod} {response.ResponseJson}");
            }
            sw.Stop();
            // Console.WriteLine("Done in "+ sw.Elapsed.ToString());

            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}