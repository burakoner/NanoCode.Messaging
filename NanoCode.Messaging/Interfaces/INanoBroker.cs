using NanoCode.Messaging.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NanoCode.Messaging.Interfaces
{
    public interface INanoBroker<
        TConnection,
        TSessionModel, TSessionIdType, TSessionOptions,
        TExchangeModel, TExchangeIdType, TExchangeOptions,
        TQueueModel, TQueueIdType, TQueueOptions,
        TQueueBindingModel, TQueueBindingIdType, TQueueBindingOptions,
        TPublisherModel, TPublisherIdType, TPublisherOptions, TPublishingOptions,
        TConsumerModel, TConsumerIdType, TConsumerOptions, TConsumingOptions,
        TRpcClientModel, TRpcClientOptions,
        TRpcServerModel, TRpcServerOptions
        >
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

        #region Session
        public TSessionModel GetSession();
        public TSessionModel GetSession(TSessionIdType id);
        public List<TSessionModel> GetSessions();
        public TSessionModel CreateSession(TSessionOptions options);
        public void DestroySession();
        public void DestroySession(TSessionIdType id);
        #endregion

        #region Exchange
        public TExchangeModel GetExchange();
        public TExchangeModel GetExchange(TExchangeIdType id);
        public List<TExchangeModel> GetExchanges();
        public TExchangeModel CreateExchange(TExchangeOptions options);
        public void DestroyExchange(TExchangeIdType id, bool ifUnused = false);
        #endregion

        #region Queue
        public TQueueModel GetQueue();
        public TQueueModel GetQueue(TQueueIdType queue);
        public List<TQueueModel> GetQueues();
        public TQueueModel CreateQueue(TQueueOptions options);
        public void DestroyQueue(TQueueIdType queue, bool ifUnused = false, bool ifEmpty = false);
        #endregion

        #region Binding
        public TQueueBindingModel GetQueueBinding();
        public TQueueBindingModel GetQueueBinding(TQueueBindingIdType binding);
        public List<TQueueBindingModel> GetQueueBindings();
        public TQueueBindingModel CreateQueueBinding(TQueueBindingOptions options);
        public void DestroyQueueBinding(TQueueBindingIdType binding);
        #endregion

        #region Publisher
        public TPublisherModel GetPublisher();
        public TPublisherModel GetPublisher(TPublisherIdType id);
        public List<TPublisherModel> GetPublishers();
        public TPublisherModel CreatePublisher(TPublisherOptions options);
        public void DestroyPublisher(TPublisherIdType publisher);

        public void PreparePublishing(TPublisherOptions publisherOptions, TPublishingOptions publishingOptions);
        public void Publish(TPublishingOptions options, object data);
        public Task PublishAsync(TPublishingOptions options, object data, TimeSpan? delay = null);
        #endregion

        #region Consumer
        public TConsumerModel GetConsumer();
        public TConsumerModel GetConsumer(TConsumerIdType id);
        public List<TConsumerModel> GetConsumers();
        public TConsumerModel CreateConsumer(TConsumerOptions options);
        public void DestroyConsumer(TConsumerIdType consumer);

        public void PrepareConsuming(TConsumerOptions consumerOptions, TConsumingOptions consumingOptions);
        public void StartConsuming(string label, TConsumingOptions options);
        public void StopConsuming(string label);
        #endregion

        #region RPC Client
        public TRpcClientModel GetRpcClient();
        public TRpcClientModel GetRpcClient(string label);
        public List<TRpcClientModel> GetRpcClients();
        public TRpcClientModel CreateRpcClient(TRpcClientOptions options);
        public void DestroyRpcClient(string label);

        public Task<NanoRpcResponse> RpcClientCallAsync(string label, NanoRpcRequest request, CancellationToken ct = default);
        #endregion

        #region RPC Server
        public TRpcServerModel GetRpcServer();
        public TRpcServerModel GetRpcServer(string label);
        public List<TRpcServerModel> GetRpcServers();
        public TRpcServerModel CreateRpcServer(TRpcServerOptions options);
        public void DestroyRpcServer(string label);

        public void RpcServerStartListening(string label);
        public void RpcServerStopListening(string label);
        #endregion

    }
}