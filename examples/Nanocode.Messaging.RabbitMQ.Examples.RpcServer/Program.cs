using Nanocode.Messaging.Interfaces;
using Nanocode.Messaging.RabbitMQ.Models;
using Nanocode.Messaging.RabbitMQ.Options;
using System;

namespace Nanocode.Messaging.RabbitMQ.Examples.RpcServer
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

            Func<INanoRpcRequest, INanoRpcResponse> handler = (request) => {
                Console.WriteLine($"New Request => Id: {request.RequestId} Method: {request.RequestMethod}");
                return new RabbitMQRpcResponseModel
                {
                    RequestId = request.RequestId,
                    RequestMethod = request.RequestMethod + " Response",
                    ResponseJson = "RPC Response Object"
                };
            };
            var rpcServerId = broker.CreateRpcServer(new RabbitMQRpcServerOptions
            {
                RoutingKey = "RPC-Route-01",
                OnRequest = handler
            });
            broker.RpcServerStartListening(rpcServerId);
            Console.WriteLine("Started!..");

            Console.ReadLine();
            Console.WriteLine("Done!");
        }
    }
}