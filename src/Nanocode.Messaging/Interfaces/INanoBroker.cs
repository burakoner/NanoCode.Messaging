using NanoCode.Messaging.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NanoCode.Messaging.Interfaces
{
    public interface INanoBroker
    {
        #region Options
        public INanoBrokerOptions Options { get; }
        #endregion

        #region Construction
        public void Construct(INanoBrokerOptions options);
        #endregion

        #region Connection
        public void Connect();
        public void Disconnect();
        #endregion

        #region Publisher
        public string CreatePublisher(INanoPublisherOptions options);
        public void Publish(string identifier, object data);
        public Task PublishAsync(string identifier, object data, TimeSpan? delay = null);
        #endregion

        #region Consumer
        public string CreateConsumer(INanoConsumerOptions options);
        public void StartConsuming(string identifier);
        public void StopConsuming(string identifier);
        public void DestroyConsumer(string identifier);
        #endregion

        #region RPC Client
        public string CreateRpcClient(INanoRpcClientOptions options);
        public Task<NanoRpcResponse> RpcClientCallAsync(string identifier, NanoRpcRequest request, CancellationToken ct);
        public void DestroyRpcClient(string identifier);
        #endregion

        #region RPC Server
        public string CreateRpcServer(INanoRpcServerOptions options);
        public void RpcServerStartListening(string identifier);
        public void RpcServerStopListening(string identifier);
        public void DestroyRpcServer(string identifier);
        #endregion

    }
}