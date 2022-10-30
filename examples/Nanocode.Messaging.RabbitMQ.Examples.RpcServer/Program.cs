using NanoCode.Messaging.Models;
using NanoCode.Messaging.RabbitMQ.Options;
using System;

namespace NanoCode.Messaging.RabbitMQ.Examples.RpcServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "RPC Server";
            Console.WriteLine("Press <ENTER> to start listening RPC Requests!..");
            Console.ReadLine();

            var broker = new RabbitMQBroker(new RabbitMQBrokerOptions
            {
                Host = "localhost",
                Port = 5672,
                Username = "guest",
                Password = "123456"
            });

            Func<NanoRpcRequest, NanoRpcResponse> handler = (request) => {
                Console.WriteLine($"New Request => Id: {request.Id} Method: {request.Method}");
                return new NanoRpcResponse
                {
                    RequestId = request.Id,
                    RequestMethod = request.Method + " Response",
                    Response = "RPC Response Object"
                };
            };
            var rpcServerId = broker.CreateRpcServer(new RabbitMQRpcServerOptions
            {
                RoutingKey = "RPC-Route-01",
                OnRequest = handler
            });
            broker.RpcServerStartListening(rpcServerId);

            Console.ReadLine();
            Console.WriteLine("Done!");
        }
    }
}