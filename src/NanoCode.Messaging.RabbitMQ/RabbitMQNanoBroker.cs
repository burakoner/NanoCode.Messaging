using NanoCode.Data.Attributes;
using NanoCode.Messaging.Interfaces;
using NanoCode.Messaging.Models;
using NanoCode.Messaging.RabbitMQ.Enums;
using NanoCode.Messaging.RabbitMQ.Models;
using NanoCode.Messaging.RabbitMQ.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NanoCode.Messaging.RabbitMQ
{
    public class RabbitMQNanoBroker : NanoBroker<

        // Connection
        IConnection,

        // Session
        RabbitMQNanoSessionModel, int, RabbitMQNanoSessionOptions,

        // Exchange
        RabbitMQNanoExchangeModel, string, RabbitMQNanoExchangeOptions,

        // Queue
        RabbitMQNanoQueueModel, string, RabbitMQNanoQueueOptions,

        // Binding
        RabbitMQNanoQueueBindingModel, string, RabbitMQNanoQueueBindingOptions,

        // Publisher
        RabbitMQNanoPublisherModel, string, RabbitMQNanoPublisherOptions, RabbitMQNanoPublishingOptions,

        // Consumer
        RabbitMQNanoConsumerModel, string, RabbitMQNanoConsumerOptions, RabbitMQNanoConsumingOptions,

        // RPC Client
        RabbitMQNanoRpcClientModel, RabbitMQNanoRpcClientOptions,

        // RPC Server
        RabbitMQNanoRpcServerModel, RabbitMQNanoRpcServerOptions
        >
    {
        #region Options
        public override INanoBrokerOptions Options { get; protected set; }
        #endregion

        #region Construction
        public RabbitMQNanoBroker(INanoBrokerOptions options)
        {
            this.Construct(options);
        }

        public override void Construct(INanoBrokerOptions options)
        {
            // Set Options
            this.Options = options;

            // Initialize
            this.Sessions = new List<RabbitMQNanoSessionModel>();
            this.Exchanges = new List<RabbitMQNanoExchangeModel>();
            this.Queues = new List<RabbitMQNanoQueueModel>();
            this.QueueBindings = new List<RabbitMQNanoQueueBindingModel>();
            this.Publishers = new List<RabbitMQNanoPublisherModel>();
            this.Consumers = new List<RabbitMQNanoConsumerModel>();
            this.RpcClients = new List<RabbitMQNanoRpcClientModel>();
            this.RpcServers = new List<RabbitMQNanoRpcServerModel>();

            // Connect
            this.Connect();
        }
        #endregion

        #region Connection
        private IConnection connection;
        public IConnection Connection
        {
            get
            {
                if (connection == null || !connection.IsOpen) Connect();
                return connection;
            }
        }
        public override void Connect()
        {
            if (connection == null || connection.IsOpen)
            {
                // Arrange
                var connectionFactory = new ConnectionFactory()
                {
                    Uri = new Uri(this.Options.ConnectionString),
                    HostName = this.Options.Host,
                    Port = this.Options.Port,
                    UserName = this.Options.Username,
                    Password = this.Options.Password,
                };

                // Action
                connection = connectionFactory.CreateConnection();
            }
        }
        public override void Disconnect()
        {
            this.Connection.Close();
        }
        #endregion

        #region Session
        public List<RabbitMQNanoSessionModel> Sessions { get; protected set; }
        public override RabbitMQNanoSessionModel GetSession()
        {
            return this.Sessions.LastOrDefault();
        }
        public override RabbitMQNanoSessionModel GetSession(int id)
        {
            return this.Sessions.FirstOrDefault(x => x.Session.ChannelNumber == id);
        }
        public override List<RabbitMQNanoSessionModel> GetSessions()
        {
            return this.Sessions;
        }
        public override RabbitMQNanoSessionModel CreateSession(RabbitMQNanoSessionOptions options = null)
        {
            // Action
            var session = Connection.CreateModel();
            var model = new RabbitMQNanoSessionModel
            {
                Session = session,
            };
            this.Sessions.Add(model);

            // Return
            return model;
        }
        public override void DestroySession()
        {
            var session = GetSession();
            if (session != null) this.DestroySession(session.Session.ChannelNumber);
        }
        public override void DestroySession(int id)
        {
            var model = this.Sessions.FirstOrDefault(x => x.Session.ChannelNumber == id);
            if (model != null)
            {
                this.Sessions.Remove(model);
                model.Session.Close();
            }
        }
        #endregion

        #region Exchange
        public List<RabbitMQNanoExchangeModel> Exchanges { get; protected set; }
        public override RabbitMQNanoExchangeModel GetExchange()
        {
            return this.Exchanges.LastOrDefault();
        }
        public override RabbitMQNanoExchangeModel GetExchange(string exchange)
        {
            return this.Exchanges.FirstOrDefault(x => x.ExchangeName == exchange);
        }
        public override List<RabbitMQNanoExchangeModel> GetExchanges()
        {
            return this.Exchanges;
        }
        public override RabbitMQNanoExchangeModel CreateExchange(RabbitMQNanoExchangeOptions options)
        {
            // Arrange
            var session = options.Session != null ? options.Session : GetSession().Session;

            // Check Point
            if (session == null || !session.IsOpen)
                throw new Exception("Invalid Session");

            // Action
            var model = new RabbitMQNanoExchangeModel
            {
                Session = options.Session,
                ExchangeName = options.ExchangeName,
                ExchangeType = options.ExchangeType,
                Durable = options.Durable,
                AutoDelete = options.AutoDelete,
                Arguments = options.Arguments
            };
            session.ExchangeDeclare(options.ExchangeName, options.ExchangeType.GetLabel(), options.Durable, options.AutoDelete, options.Arguments);
            this.Exchanges.Add(model);

            // Return
            return model;
        }
        public override void DestroyExchange(string exchange, bool ifUnused = false)
        {
            var model = this.Exchanges.FirstOrDefault(x => x.ExchangeName == exchange);
            if (model != null)
            {
                // Arrange
                var session = model.Session != null ? model.Session : GetSession().Session;

                // Check Point
                if (session == null || !session.IsOpen)
                    throw new Exception("Invalid Session");

                // Action
                this.Exchanges.Remove(model);
                session.ExchangeDelete(model.ExchangeName, ifUnused);
            }
        }
        #endregion

        #region Queue
        public List<RabbitMQNanoQueueModel> Queues { get; protected set; }
        public override RabbitMQNanoQueueModel GetQueue()
        {
            return this.Queues.LastOrDefault();
        }
        public override RabbitMQNanoQueueModel GetQueue(string queue)
        {
            return this.Queues.FirstOrDefault(x => x.Options.QueueName == queue);
        }
        public override List<RabbitMQNanoQueueModel> GetQueues()
        {
            return this.Queues;
        }
        public override RabbitMQNanoQueueModel CreateQueue(RabbitMQNanoQueueOptions options)
        {
            // Arrange
            var session = options.Session != null ? options.Session : GetSession().Session;

            // Check Point
            if (session == null || !session.IsOpen)
                throw new Exception("Invalid Session");

            // Action
            var model = new RabbitMQNanoQueueModel
            {
                Options = options,
                Session = session,
            };
            model.QueueDeclare = session.QueueDeclare(options.QueueName, options.Durable, options.Exclusive, options.AutoDelete, options.Arguments);
            this.Queues.Add(model);

            // Return
            return model;
        }
        public override void DestroyQueue(string queue, bool ifUnused = false, bool ifEmpty = false)
        {
            var model = this.Queues.FirstOrDefault(x => x.Options.QueueName == queue);
            if (model != null)
            {
                // Arrange
                var session = model.Session != null ? model.Session : GetSession().Session;

                // Check Point
                if (session == null || !session.IsOpen)
                    throw new Exception("Invalid Session");

                // Action
                this.Queues.Remove(model);
                session.QueueDelete(model.QueueDeclare.QueueName, ifUnused, ifEmpty);
            }
        }
        #endregion

        #region Queue Binding
        public List<RabbitMQNanoQueueBindingModel> QueueBindings { get; protected set; }
        public override RabbitMQNanoQueueBindingModel GetQueueBinding()
        {
            return this.QueueBindings.LastOrDefault();
        }
        public override RabbitMQNanoQueueBindingModel GetQueueBinding(string routingKey)
        {
            return this.QueueBindings.FirstOrDefault(x => x.RoutingKey == routingKey);
        }
        public override List<RabbitMQNanoQueueBindingModel> GetQueueBindings()
        {
            return this.QueueBindings;
        }
        public override RabbitMQNanoQueueBindingModel CreateQueueBinding(RabbitMQNanoQueueBindingOptions options)
        {
            // Arrange
            var session = options.Session != null ? options.Session : GetSession().Session;

            // Check Point
            if (session == null || !session.IsOpen)
                throw new Exception("Invalid Session");

            // Action
            var model = new RabbitMQNanoQueueBindingModel
            {
                Session = options.Session,
                ExchangeName = options.ExchangeName,
                QueueName = options.QueueName,
                RoutingKey = options.RoutingKey,
                Arguments = options.Arguments,
            };

            // Action
            session.QueueBind(options.QueueName, options.ExchangeName, options.RoutingKey, options.Arguments);
            this.QueueBindings.Add(model);

            // Return
            return model;
        }
        public override void DestroyQueueBinding(string routingKey)
        {
            var model = this.QueueBindings.FirstOrDefault(x => x.RoutingKey == routingKey);
            if (model != null)
            {
                // Arrange
                var session = model.Session != null ? model.Session : GetSession().Session;

                // Check Point
                if (session == null || !session.IsOpen)
                    throw new Exception("Invalid Session");

                // Action
                this.QueueBindings.Remove(model);
                session.QueueUnbind(model.QueueName, model.ExchangeName, model.RoutingKey);
            }
        }
        #endregion

        #region Publisher
        public List<RabbitMQNanoPublisherModel> Publishers { get; protected set; }
        public override RabbitMQNanoPublisherModel GetPublisher()
        {
            throw new NotImplementedException();
        }
        public override RabbitMQNanoPublisherModel GetPublisher(string id)
        {
            throw new NotImplementedException();
        }
        public override List<RabbitMQNanoPublisherModel> GetPublishers()
        {
            throw new NotImplementedException();
        }
        public override RabbitMQNanoPublisherModel CreatePublisher(RabbitMQNanoPublisherOptions options)
        {
            throw new NotImplementedException();
        }
        public override void DestroyPublisher(string publisher)
        {
            throw new NotImplementedException();
        }

        public override void PreparePublishing(RabbitMQNanoPublisherOptions publisherOptions, RabbitMQNanoPublishingOptions publishingOptions)
        {
            // Arrange
            var session = publishingOptions.Session != null ? publishingOptions.Session : GetSession().Session;

            // Check Point
            if (session == null || !session.IsOpen)
                throw new Exception("Invalid Session");

            // Check Exchange
            var exchange = this.GetExchange(publishingOptions.ExchangeName);
            if (exchange == null) exchange = this.CreateExchange(new RabbitMQNanoExchangeOptions
            {
                Session = session,
                ExchangeName = publishingOptions.ExchangeName,
                ExchangeType = publishingOptions.ExchangeType,
                AutoDelete = publishingOptions.AutoDelete,
                Durable = publishingOptions.Durable,
                Arguments = publishingOptions.Arguments,
            });

            // Check Queue
            var queue = this.GetQueue(publishingOptions.QueueName);
            if (queue == null) queue = this.CreateQueue(new RabbitMQNanoQueueOptions
            {
                Session = session,
                QueueName = publishingOptions.QueueName,
                Durable = publishingOptions.Durable,
                Exclusive = publishingOptions.Exclusive,
                AutoDelete = publishingOptions.AutoDelete,
                Arguments = publishingOptions.Arguments,
            });

            // Check Queue Binding
            var binding = this.GetQueueBinding(publishingOptions.RoutingKey);
            if (binding == null) binding = this.CreateQueueBinding(new RabbitMQNanoQueueBindingOptions
            {
                Session = session,
                ExchangeName = publishingOptions.ExchangeName,
                QueueName = publishingOptions.QueueName,
                RoutingKey = publishingOptions.RoutingKey,
                Arguments = publishingOptions.Arguments,
            });
        }
        public override void Publish(RabbitMQNanoPublishingOptions options, object data)
        {
            // Session
            var session = options.Session != null ? options.Session : GetSession().Session;

            // Action
            var bytes =
                  data is byte[]? (byte[])data
                : data is string ? Encoding.UTF8.GetBytes((string)data)
                : Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));

            // Action
            session.BasicPublish(options.ExchangeName, options.RoutingKey, false, session.CreateBasicProperties(), bytes);
        }
        public override async Task PublishAsync(RabbitMQNanoPublishingOptions options, object data, TimeSpan? delay = null)
        {
            // Wait
            if (delay.HasValue) await Task.Delay(delay.Value);

            // Session
            var session = options.Session != null ? options.Session : GetSession().Session;

            // Action
            var bytes =
                  data is byte[]? (byte[])data
                : data is string ? Encoding.UTF8.GetBytes((string)data)
                : Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));

            // Action
            session.BasicPublish(options.ExchangeName, options.RoutingKey, false, session.CreateBasicProperties(), bytes);

            // Return
            await Task.CompletedTask;
        }
        #endregion

        #region Consumer
        public List<RabbitMQNanoConsumerModel> Consumers { get; protected set; }
        public override RabbitMQNanoConsumerModel GetConsumer()
        {
            return this.Consumers.LastOrDefault();
        }
        public override RabbitMQNanoConsumerModel GetConsumer(string label)
        {
            return this.Consumers.FirstOrDefault(x => x.Label == label);
        }
        public override List<RabbitMQNanoConsumerModel> GetConsumers()
        {
            return this.Consumers;
        }
        public override RabbitMQNanoConsumerModel CreateConsumer(RabbitMQNanoConsumerOptions options)
        {
            // Arrange
            var session = options.Session != null ? options.Session : GetSession().Session;

            // Check Point
            if (session == null || !session.IsOpen)
                throw new Exception("Invalid Session");

            // Check Point
            if (this.Consumers.Count(x => x.Label == options.Label) > 0)
                throw new Exception("Consumer with this label is already exists");

            // Action
            var consumer = new EventingBasicConsumer(session);
            if (options.OnReceived != null) consumer.Received += options.OnReceived;
            if (options.OnRegistered != null) consumer.Registered += options.OnRegistered;
            if (options.OnUnregistered != null) consumer.Unregistered += options.OnUnregistered;
            if (options.OnShutdown != null) consumer.Shutdown += options.OnShutdown;

            // Action
            var model = new RabbitMQNanoConsumerModel
            {
                Label = options.Label,
                ConsumerOptions = options,
                Consumer = consumer,
            };
            this.Consumers.Add(model);

            // Return
            return model;
        }
        public override void DestroyConsumer(string label)
        {
            var model = this.Consumers.FirstOrDefault(x => x.Label == label);
            if (model == null) throw new Exception("No consumer found!");

            // Arrange
            var session = model.ConsumerOptions.Session != null ? model.ConsumerOptions.Session : GetSession().Session;

            // Check Point
            if (session == null || !session.IsOpen)
                throw new Exception("Invalid Session");

            // Action
            this.Consumers.Remove(model);
            this.StopConsuming(model.Label);
        }

        public override void PrepareConsuming(RabbitMQNanoConsumerOptions consumerOptions, RabbitMQNanoConsumingOptions consumingOptions)
        {
            // Arrange
            var session = consumingOptions.Session != null ? consumingOptions.Session : GetSession().Session;

            // Check Point
            if (session == null || !session.IsOpen)
                throw new Exception("Invalid Session");

            // Check Exchange
            var exchange = this.GetExchange(consumingOptions.ExchangeName);
            if (exchange == null) exchange = this.CreateExchange(new RabbitMQNanoExchangeOptions
            {
                Session = session,
                ExchangeName = consumingOptions.ExchangeName,
                ExchangeType = consumingOptions.ExchangeType,
                AutoDelete = consumingOptions.AutoDelete,
                Durable = consumingOptions.Durable,
                Arguments = consumingOptions.Arguments,
            });

            // Check Queue
            var queue = this.GetQueue(consumingOptions.QueueName);
            if (queue == null) queue = this.CreateQueue(new RabbitMQNanoQueueOptions
            {
                Session = session,
                QueueName = consumingOptions.QueueName,
                Durable = consumingOptions.Durable,
                Exclusive = consumingOptions.Exclusive,
                AutoDelete = consumingOptions.AutoDelete,
                Arguments = consumingOptions.Arguments,
            });

            // Check Queue Binding
            var binding = this.GetQueueBinding(consumingOptions.RoutingKey);
            if (binding == null) binding = this.CreateQueueBinding(new RabbitMQNanoQueueBindingOptions
            {
                Session = session,
                ExchangeName = consumingOptions.ExchangeName,
                QueueName = consumingOptions.QueueName,
                RoutingKey = consumingOptions.RoutingKey,
                Arguments = consumingOptions.Arguments,
            });

            // Check Consumer
            var consumer = this.GetConsumer(consumerOptions.Label);
            if (consumer == null) consumer = this.CreateConsumer(consumerOptions);
        }
        public override void StartConsuming(string label, RabbitMQNanoConsumingOptions options)
        {
            var model = this.Consumers.FirstOrDefault(x => x.Label == label);
            if (model == null) throw new Exception("No consumer found!");

            // Arrange
            var session = model.ConsumerOptions.Session != null ? model.ConsumerOptions.Session : GetSession().Session;

            // Check Point
            if (session == null || !session.IsOpen)
                throw new Exception("Invalid Session");

            // Action
            model.ConsumingOptions = options;
            model.Tag = session.BasicConsume(
                model.ConsumingOptions.QueueName,
                model.ConsumingOptions.AutoAcknowledgement,
                model.Consumer);
        }
        public override void StopConsuming(string label)
        {
            var model = this.Consumers.FirstOrDefault(x => x.Label == label);
            if (model == null) throw new Exception("No consumer found!");
            if (string.IsNullOrEmpty(model.Tag)) throw new Exception("No consumer tag found!");

            // Arrange
            var session = model.ConsumerOptions.Session != null ? model.ConsumerOptions.Session : GetSession().Session;

            // Check Point
            if (session == null || !session.IsOpen)
                throw new Exception("Invalid Session");

            // Action
            session.BasicCancel(model.Tag);
        }
        #endregion

        #region RPC Client
        public List<RabbitMQNanoRpcClientModel> RpcClients { get; protected set; }
        public override RabbitMQNanoRpcClientModel GetRpcClient()
        {
            return this.RpcClients.LastOrDefault();
        }
        public override RabbitMQNanoRpcClientModel GetRpcClient(string label)
        {
            return this.RpcClients.FirstOrDefault(x => x.Label == label);
        }
        public override List<RabbitMQNanoRpcClientModel> GetRpcClients()
        {
            return this.RpcClients;
        }
        public override RabbitMQNanoRpcClientModel CreateRpcClient(RabbitMQNanoRpcClientOptions options)
        {
            // Arrange
            var session = options.Session != null ? options.Session : GetSession().Session;

            // Check Point
            if (session == null || !session.IsOpen)
                throw new Exception("Invalid Session");

            // Check Point
            if (this.RpcClients.Count(x => x.Label == options.Label) > 0)
                throw new Exception("RpcClient with this label is already exists");

            // Action
            var rpcClient = new RabbitMQNanoRpcClient(session, options.RoutingKey);
            var model = new RabbitMQNanoRpcClientModel
            {
                Client = rpcClient,
                Label = options.Label,
                RoutingKey = options.RoutingKey,
            };
            this.RpcClients.Add(model);

            // Return
            return model;
        }
        public override void DestroyRpcClient(string label)
        {
            var model = this.RpcClients.FirstOrDefault(x => x.Label == label);
            if (model == null) throw new Exception("No client found!");

            // Action
            this.RpcClients.Remove(model);
        }

        public override async Task<NanoRpcResponse> RpcClientCallAsync(string label, NanoRpcRequest request, CancellationToken ct = default)
        {
            var model = this.RpcClients.FirstOrDefault(x => x.Label == label);
            if (model == null) throw new Exception("No client found!");

            // Action
            return await model.Client.CallAsync(request, ct);
        }
        #endregion

        #region RPC Server
        public List<RabbitMQNanoRpcServerModel> RpcServers { get; protected set; }
        public override RabbitMQNanoRpcServerModel GetRpcServer()
        {
            return this.RpcServers.LastOrDefault();
        }
        public override RabbitMQNanoRpcServerModel GetRpcServer(string label)
        {
            return this.RpcServers.FirstOrDefault(x => x.Label == label);
        }
        public override List<RabbitMQNanoRpcServerModel> GetRpcServers()
        {
            return this.RpcServers;
        }
        public override RabbitMQNanoRpcServerModel CreateRpcServer(RabbitMQNanoRpcServerOptions options)
        {
            // Arrange
            var session = options.Session != null ? options.Session : GetSession().Session;

            // Check Point
            if (session == null || !session.IsOpen)
                throw new Exception("Invalid Session");

            // Check Point
            if (this.RpcServers.Count(x => x.Label == options.Label) > 0)
                throw new Exception("An Rpc Server with this label is already exists");

            // Create Consumer
            var consumer = new EventingBasicConsumer(session);
            consumer.Received += (model, ea) =>
            {
                var response = new NanoRpcResponse();

                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = session.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                try
                {
                    var json = Encoding.UTF8.GetString(body);
                    var request = JsonConvert.DeserializeObject<NanoRpcRequest>(json);
                    response = options.Function(request);
                }
                catch
                {
                    response = null;
                }
                finally
                {
                    var json = JsonConvert.SerializeObject(response);
                    var jsonBytes = Encoding.UTF8.GetBytes(json);
                    session.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: jsonBytes);
                    session.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            // Action
            var model = new RabbitMQNanoRpcServerModel
            {
                Label = options.Label,
                Session = session,
                RoutingKey = options.RoutingKey,
                Consumer = consumer,
            };
            this.RpcServers.Add(model);

            // Return
            return model;
        }
        public override void DestroyRpcServer(string label)
        {
            var model = this.RpcServers.FirstOrDefault(x => x.Label == label);
            if (model == null) throw new Exception("No RPC Server found!");

            // Arrange
            var session = model.Session != null ? model.Session : GetSession().Session;

            // Check Point
            if (session == null || !session.IsOpen)
                throw new Exception("Invalid Session");

            // Action
            this.RpcServers.Remove(model);
            this.RpcServerStopListening(model.Label);
        }

        public override void RpcServerStartListening(string label)
        {
            var model = this.RpcServers.FirstOrDefault(x => x.Label == label);
            if (model == null) throw new Exception("No RPC Server found!");

            // Arrange
            var session = model.Session != null ? model.Session : GetSession().Session;

            // Check Point
            if (session == null || !session.IsOpen)
                throw new Exception("Invalid Session");

            // Action
            session.QueueDeclare(queue: model.RoutingKey, durable: false, exclusive: false, autoDelete: false, arguments: null);
            session.BasicQos(0, 1, false);
            model.Tag = session.BasicConsume(queue: model.RoutingKey, autoAck: false, consumer: model.Consumer);
        }
        public override void RpcServerStopListening(string label)
        {
            var model = this.Consumers.FirstOrDefault(x => x.Label == label);
            if (model == null) throw new Exception("No consumer found!");
            if (string.IsNullOrEmpty(model.Tag)) throw new Exception("No consumer tag found!");

            // Arrange
            var session = model.ConsumerOptions.Session != null ? model.ConsumerOptions.Session : GetSession().Session;

            // Check Point
            if (session == null || !session.IsOpen)
                throw new Exception("Invalid Session");

            // Action
            session.BasicCancel(model.Tag);
        }
        #endregion

    }
}
