using NanoCode.Messaging.Interfaces;
using NanoCode.Messaging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NanoCode.Messaging
{
    public abstract class NanoBroker<
        TConnection,
        TSessionModel, TSessionIdType, TSessionOptions,
        TExchangeModel, TExchangeIdType, TExchangeOptions,
        TQueueModel, TQueueIdType, TQueueOptions,
        TQueueBindingModel, TQueueBindingIdType, TQueueBindingOptions,
        TPublisherModel, TPublisherIdType, TPublisherOptions, TPublishingOptions,
        TConsumerModel, TConsumerIdType, TConsumerOptions, TConsumingOptions,
        TRpcClientModel, TRpcClientOptions,
        TRpcServerModel, TRpcServerOptions>
        : INanoBroker<
        TConnection,
        TSessionModel, TSessionIdType, TSessionOptions,
        TExchangeModel, TExchangeIdType, TExchangeOptions,
        TQueueModel, TQueueIdType, TQueueOptions,
        TQueueBindingModel, TQueueBindingIdType, TQueueBindingOptions,
        TPublisherModel, TPublisherIdType, TPublisherOptions, TPublishingOptions,
        TConsumerModel, TConsumerIdType, TConsumerOptions, TConsumingOptions,
        TRpcClientModel, TRpcClientOptions,
        TRpcServerModel, TRpcServerOptions>
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

        #region Session
        public abstract TSessionModel GetSession();
        public abstract TSessionModel GetSession(TSessionIdType id);
        public abstract List<TSessionModel> GetSessions();
        public abstract TSessionModel CreateSession(TSessionOptions options);
        public abstract void DestroySession();
        public abstract void DestroySession(TSessionIdType id);
        #endregion

        #region Exchange
        public abstract TExchangeModel GetExchange();
        public abstract TExchangeModel GetExchange(TExchangeIdType id);
        public abstract List<TExchangeModel> GetExchanges();
        public abstract TExchangeModel CreateExchange(TExchangeOptions options);
        public abstract void DestroyExchange(TExchangeIdType id, bool ifUnused = false);
        #endregion

        #region Queue
        public abstract TQueueModel GetQueue();
        public abstract TQueueModel GetQueue(TQueueIdType queue);
        public abstract List<TQueueModel> GetQueues();
        public abstract TQueueModel CreateQueue(TQueueOptions options);
        public abstract void DestroyQueue(TQueueIdType queue, bool ifUnused = false, bool ifEmpty = false);
        #endregion

        #region Binding
        public abstract TQueueBindingModel GetQueueBinding();
        public abstract TQueueBindingModel GetQueueBinding(TQueueBindingIdType binding);
        public abstract List<TQueueBindingModel> GetQueueBindings();
        public abstract TQueueBindingModel CreateQueueBinding(TQueueBindingOptions options);
        public abstract void DestroyQueueBinding(TQueueBindingIdType binding);
        #endregion

        #region Publisher
        public abstract TPublisherModel GetPublisher();
        public abstract TPublisherModel GetPublisher(TPublisherIdType id);
        public abstract List<TPublisherModel> GetPublishers();
        public abstract TPublisherModel CreatePublisher(TPublisherOptions options);
        public abstract void DestroyPublisher(TPublisherIdType publisher);

        public abstract void PreparePublishing(TPublisherOptions publisherOptions, TPublishingOptions publishingOptions);
        public abstract void Publish(TPublishingOptions options, object data);
        public abstract Task PublishAsync(TPublishingOptions options, object data, TimeSpan? delay = null);
        #endregion

        #region Consumer
        public abstract TConsumerModel GetConsumer();
        public abstract TConsumerModel GetConsumer(TConsumerIdType id);
        public abstract List<TConsumerModel> GetConsumers();
        public abstract TConsumerModel CreateConsumer(TConsumerOptions options);
        public abstract void DestroyConsumer(TConsumerIdType consumer);

        public abstract void PrepareConsuming(TConsumerOptions consumerOptions, TConsumingOptions consumingOptions);
        public abstract void StartConsuming(string label, TConsumingOptions options);
        public abstract void StopConsuming(string label);
        #endregion

        #region RPC Client
        public abstract TRpcClientModel GetRpcClient();
        public abstract TRpcClientModel GetRpcClient(string label);
        public abstract List<TRpcClientModel> GetRpcClients();
        public abstract TRpcClientModel CreateRpcClient(TRpcClientOptions options);
        public abstract void DestroyRpcClient(string label);

        public abstract Task<NanoRpcResponse> RpcClientCallAsync(string label, NanoRpcRequest request, CancellationToken ct = default);
        #endregion

        #region RPC Server
        public abstract TRpcServerModel GetRpcServer();
        public abstract TRpcServerModel GetRpcServer(string label);
        public abstract List<TRpcServerModel> GetRpcServers();
        public abstract TRpcServerModel CreateRpcServer(TRpcServerOptions options);
        public abstract void DestroyRpcServer(string label);

        public abstract void RpcServerStartListening(string label);
        public abstract void RpcServerStopListening(string label);
        #endregion

    }
}