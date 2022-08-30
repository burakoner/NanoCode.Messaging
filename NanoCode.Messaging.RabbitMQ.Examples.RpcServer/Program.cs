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
            Console.WriteLine("Press <ENTER> to start listening!..");
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

            Func<NanoRpcRequest, NanoRpcResponse> handler = (request) => {
                Console.WriteLine($"New Request => Method: {request.Method}");
                return new NanoRpcResponse
                {
                    Method = request.Method + " Response",
                    Arguments = request.Arguments,
                };
            };
            broker.CreateRpcServer(new RabbitMQNanoRpcServerOptions
            {
                Label = "Rpc-Server-01",
                Session = session.Session,
                RoutingKey = "rpc-01-route",
                Function = handler
            });
            broker.RpcServerStartListening("Rpc-Server-01");

            Console.ReadLine();
            Console.WriteLine("Done!");
        }
    }
}