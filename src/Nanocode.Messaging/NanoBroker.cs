using NanoCode.Messaging.Interfaces;
using NanoCode.Messaging.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NanoCode.Messaging
{
    public abstract class NanoBroker : INanoBroker
    {
        #region Options
        public abstract INanoBrokerOptions Options { get; protected set; }
        #endregion

        #region Construction
        public abstract void Construct(INanoBrokerOptions options);
        #endregion

        #region Connection
        public abstract void Connect();
        public abstract void Disconnect();
        #endregion

        #region Publisher
        public Dictionary<string, INanoPublisherOptions> PublishingOptions = new();
        public abstract string CreatePublisher(INanoPublisherOptions options);
        public abstract void Publish(string identifier, object data);
        public abstract Task PublishAsync(string identifier, object data, TimeSpan? delay = null);
        #endregion

        #region Consumer
        public Dictionary<string, INanoConsumerOptions> ConsumingOptions = new();
        public abstract string CreateConsumer(INanoConsumerOptions options);
        public abstract void StartConsuming(string identifier);
        public abstract void StopConsuming(string identifier);
        public abstract void DestroyConsumer(string identifier);
        #endregion

        #region RPC Client
        public abstract string CreateRpcClient(INanoRpcClientOptions options);
        public abstract Task<NanoRpcResponse> RpcClientCallAsync(string identifier, NanoRpcRequest request, CancellationToken ct);
        public abstract void DestroyRpcClient(string identifier);
        #endregion

        #region RPC Server
        public abstract string CreateRpcServer(INanoRpcServerOptions options);
        public abstract void RpcServerStartListening(string identifier);
        public abstract void RpcServerStopListening(string identifier);
        public abstract void DestroyRpcServer(string identifier);
        #endregion
    }
}