namespace Nanocode.Messaging.RabbitMQ
{
    public sealed class RabbitMQBroker : NanoBroker
    {
        #region Options
        public override INanoBrokerOptions Options { get; protected set; }
        #endregion

        #region Construction
        public RabbitMQBroker(INanoBrokerOptions options)
        {
            this.Construct(options);
        }
        public override void Construct(INanoBrokerOptions options)
        {
            // Set Options
            this.Options = options;

            // Connect
            this.Connect();
        }
        #endregion

        #region Connection
        private IConnection _connection;
        public IConnection Connection
        {
            get
            {
                if (_connection == null || !_connection.IsOpen) Connect();
                return _connection;
            }
        }
        public override void Connect()
        {
            if (_connection == null || _connection.IsOpen)
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
                _connection = connectionFactory.CreateConnection();
                _session = _connection.CreateModel();
            }
        }
        public override void Disconnect()
        {
            this.Session.Close();
            this.Connection.Close();
        }
        #endregion

        #region Session
        private IModel _session;
        public IModel Session
        {
            get
            {
                return _session;
            }
        }
        #endregion

        #region Exchange
        private List<RabbitMQExchangeModel> Exchanges { get; set; } = new();
        private RabbitMQExchangeModel GetExchange(string exchange)
        {
            return this.Exchanges.FirstOrDefault(x => x.ExchangeName == exchange);
        }
        private RabbitMQExchangeModel NewExchange(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
        {
            // Action
            var model = new RabbitMQExchangeModel
            {
                ExchangeName = exchange,
                ExchangeType = type,
                Durable = durable,
                AutoDelete = autoDelete,
                Arguments = arguments
            };
            this.Session.ExchangeDeclare(model.ExchangeName, model.ExchangeType, model.Durable, model.AutoDelete, model.Arguments);
            this.Exchanges.Add(model);

            // Return
            return model;
        }
        private void DestroyExchange(string exchange, bool ifUnused = false)
        {
            // Check Popint
            var model = this.Exchanges.FirstOrDefault(x => x.ExchangeName == exchange);
            if (model == null) return;

            // Action
            this.Exchanges.Remove(model);
            this.Session.ExchangeDelete(model.ExchangeName, ifUnused);
        }
        #endregion

        #region Queue
        private List<RabbitMQQueueModel> Queues { get; set; } = new();
        private RabbitMQQueueModel GetQueue(string queue)
        {
            return this.Queues.FirstOrDefault(x => x.QueueName == queue);
        }
        private RabbitMQQueueModel NewQueue(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
        {
            // Action
            var model = new RabbitMQQueueModel
            {
                QueueName = queue,
                Durable = durable,
                Exclusive = exclusive,
                AutoDelete = autoDelete,
                Arguments = arguments,
            };
            model.QueueDeclare = this.Session.QueueDeclare(model.QueueName, model.Durable, model.Exclusive, model.AutoDelete, model.Arguments);
            this.Queues.Add(model);

            // Return
            return model;
        }
        private uint DestroyQueue(string queue, bool ifUnused = false, bool ifEmpty = false)
        {
            // Check Popint
            var model = this.Queues.FirstOrDefault(x => x.QueueName == queue);
            if (model == null) return 0;

            // Action
            this.Queues.Remove(model);
            return this.Session.QueueDelete(model.QueueDeclare.QueueName, ifUnused, ifEmpty);
        }
        #endregion

        #region Binding
        private List<RabbitMQQueueBindingModel> QueueBindings { get; set; } = new();
        private RabbitMQQueueBindingModel GetQueueBinding(string routingKey)
        {
            return this.QueueBindings.FirstOrDefault(x => x.RoutingKey == routingKey);
        }
        private RabbitMQQueueBindingModel NewQueueBinding(string exchange, string queue, string routingKey, IDictionary<string, object> arguments)
        {
            // Action
            var model = new RabbitMQQueueBindingModel
            {
                ExchangeName = exchange,
                QueueName = queue,
                RoutingKey = routingKey,
                Arguments = arguments,
            };

            // Action
            this.Session.QueueBind(model.QueueName, model.ExchangeName, model.RoutingKey, model.Arguments);
            this.QueueBindings.Add(model);

            // Return
            return model;
        }
        private void DestroyQueueBinding(string routingKey)
        {
            var model = this.QueueBindings.FirstOrDefault(x => x.RoutingKey == routingKey);
            if (model != null)
            {
                // Action
                this.QueueBindings.Remove(model);
                this.Session.QueueUnbind(model.QueueName, model.ExchangeName, model.RoutingKey);
            }
        }
        #endregion

        #region Publisher
        /// <summary>
        /// Creates Publisher and returns Publisher ID
        /// </summary>
        /// <param name="options"></param>
        /// <returns>Publisher ID</returns>
        public override string CreatePublisher(INanoPublisherOptions options)
        {
            // Check Exchange
            var exchange = this.GetExchange(options.Exchange);
            exchange ??= this.NewExchange(options.Exchange, options.ExchangeType, options.Durable, options.AutoDelete, options.Arguments);

            // Check Queue
            var queue = this.GetQueue(options.Queue);
            queue ??= this.NewQueue(options.Queue, options.Durable, options.Exclusive, options.AutoDelete, options.Arguments);

            // Check Queue Binding
            var binding = this.GetQueueBinding(options.RoutingKey);
            binding ??= this.NewQueueBinding(options.Exchange, options.Queue, options.RoutingKey, options.Arguments);

            // Add to Dictionary
            var identifier = Guid.NewGuid().ToString();
            this.PublishingOptions[identifier] = options;

            // Return
            return identifier;
        }
        public override void Publish(string identifier, object data)
        {
            // Check Point
            if (!PublishingOptions.ContainsKey(identifier))
                return;

            // Action
            var bytes =
                  data is byte[] b ? b
                : data is string s ? Encoding.UTF8.GetBytes(s)
                : Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            this.Session.BasicPublish(this.PublishingOptions[identifier].Exchange, this.PublishingOptions[identifier].RoutingKey, false, this.Session.CreateBasicProperties(), bytes);
        }
        public override async Task PublishAsync(string identifier, object data, TimeSpan? delay = null)
        {
            // Wait
            if (delay.HasValue) await Task.Delay(delay.Value);

            // Action
            this.Publish(identifier, data);
        }
        #endregion

        #region Consumer
        private List<RabbitMQConsumerModel> Consumers { get; set; } = new();
        private RabbitMQConsumerModel GetConsumer(string identifier)
        {
            return this.Consumers.FirstOrDefault(x => x.Identifier == identifier);
        }
        private RabbitMQConsumerModel NewConsumer(string queue, bool autoAck, EventHandler<BasicDeliverEventArgs> onReceived, EventHandler<ConsumerEventArgs> onRegistered, EventHandler<ConsumerEventArgs> onUnregistered, EventHandler<ShutdownEventArgs> onShutdown)
        {
            // Identifier
            var identifier = Guid.NewGuid().ToString();

            // Action
            var consumer = new EventingBasicConsumer(this.Session);
            if (onReceived != null) consumer.Received += onReceived;
            if (onRegistered != null) consumer.Registered += onRegistered;
            if (onUnregistered != null) consumer.Unregistered += onUnregistered;
            if (onShutdown != null) consumer.Shutdown += onShutdown;

            // Action
            var model = new RabbitMQConsumerModel
            {
                QueueName = queue,
                Identifier = identifier,
                AutoAcknowledgement = autoAck,
                OnReceived = onReceived,
                OnRegistered = onRegistered,
                OnUnregistered = onUnregistered,
                OnShutdown = onShutdown,
                Consumer = consumer,
            };
            this.Consumers.Add(model);

            // Return
            return model;
        }

        /// <summary>
        /// Creates Consumer and returns Consumer ID
        /// </summary>
        /// <param name="options"></param>
        /// <returns>Consumer ID</returns>
        public override string CreateConsumer(INanoConsumerOptions options)
        {
            // Check Exchange
            var exchange = this.GetExchange(options.ExchangeName);
            _ = exchange ?? this.NewExchange(options.ExchangeName, options.ExchangeType, options.Durable, options.AutoDelete, options.Arguments);

            // Check Queue
            var queue = this.GetQueue(options.QueueName);
            _ = queue ?? this.NewQueue(options.QueueName, options.Durable, options.Exclusive, options.AutoDelete, options.Arguments);

            // Check Queue Binding
            var binding = this.GetQueueBinding(options.RoutingKey);
            _ = binding ?? this.NewQueueBinding(options.ExchangeName, options.QueueName, options.RoutingKey, options.Arguments);

            // Check Consumer
            var ops = (RabbitMQConsumerOptions)options;
            var consumer = this.NewConsumer(options.QueueName, options.AutoAcknowledgement, ops.OnReceived, ops.OnRegistered, ops.OnUnregistered, ops.OnShutdown);
            ops.Identifier = consumer.Identifier;

            // Add to Dictionary
            this.ConsumingOptions[consumer.Identifier] = ops;

            // Return
            return consumer.Identifier;
        }
        public override void StartConsuming(string identifier)
        {
            var model = this.Consumers.FirstOrDefault(x => x.Identifier == identifier);
            if (model == null) throw new Exception("No consumer found!");

            // Action
            model.Tag = this.Session.BasicConsume(model.QueueName, model.AutoAcknowledgement, model.Consumer);
        }
        public override void StopConsuming(string identifier)
        {
            var model = this.Consumers.FirstOrDefault(x => x.Identifier == identifier);
            if (model == null) throw new Exception("No consumer found!");
            if (string.IsNullOrEmpty(model.Tag)) throw new Exception("No consumer tag found!");

            // Action
            this.Session.BasicCancel(model.Tag);
        }
        public override void DestroyConsumer(string identifier)
        {
            var model = this.Consumers.FirstOrDefault(x => x.Identifier == identifier);
            if (model == null) return;

            // Action
            this.Consumers.Remove(model);
            this.StopConsuming(model.Identifier);
        }
        #endregion

        #region RPC Client
        private List<RabbitMQRpcClientModel> RpcClients { get; set; } = new();
        private RabbitMQRpcClientModel GetRpcClient(string identifier)
        {
            return this.RpcClients.FirstOrDefault(x => x.Identifier == identifier);
        }
        private string NewRpcClient(string routingKey)
        {
            // Action
            var identifier = Guid.NewGuid().ToString();
            var rpcClient = new RabbitMQRpcClient(this.Session, routingKey);
            var model = new RabbitMQRpcClientModel
            {
                Client = rpcClient,
                Identifier = identifier,
                RoutingKey = routingKey,
            };
            this.RpcClients.Add(model);

            // Return
            return identifier;
        }

        /// <summary>
        /// Creates RPC Client and returns RPC Client ID
        /// </summary>
        /// <param name="routingKey"></param>
        /// <returns></returns>
        public override string CreateRpcClient(INanoRpcClientOptions options)
        {
            // Check Point
            var client = this.GetRpcClient(options.RoutingKey);
            if (client != null) return client.Identifier;

            return NewRpcClient(options.RoutingKey);
        }
        public override Task<INanoRpcResponse> RpcClientCallAsync(string identifier, INanoRpcRequest request, CancellationToken ct = default)
        {
            var model = this.RpcClients.FirstOrDefault(x => x.Identifier == identifier);
            if (model == null) throw new Exception("No client found!");

            // Action
            return model.Client.CallAsync(request, ct);
        }
        public override void DestroyRpcClient(string identifier)
        {
            var model = this.RpcClients.FirstOrDefault(x => x.Identifier == identifier);
            if (model == null) return;

            // Action
            this.RpcClients.Remove(model);
        }
        #endregion

        #region RPC Server
        private List<RabbitMQRpcServerModel> RpcServers { get; set; } = new();
        private RabbitMQRpcServerModel GetRpcServer(string identifier)
        {
            return this.RpcServers.FirstOrDefault(x => x.Identifier == identifier);
        }
        private string NewRpcServer(string routingKey , Func<INanoRpcRequest, INanoRpcResponse> onRequest)
        {
            // Check Point
            var identifier = Guid.NewGuid().ToString();

            // Consumer
            var consumer = new EventingBasicConsumer(this.Session);
            consumer.Received += (model, ea) =>
            {
                INanoRpcResponse response = null;

                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = this.Session.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                try
                {
                    var json = Encoding.UTF8.GetString(body);
                    var request = JsonConvert.DeserializeObject<RabbitMQRpcRequestModel>(json);
                    response = onRequest(request);
                }
                catch
                {
                    response = null;
                }
                finally
                {
                    var json = JsonConvert.SerializeObject(response);
                    var jsonBytes = Encoding.UTF8.GetBytes(json);
                    this.Session.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: jsonBytes);
                    this.Session.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            // Action
            var model = new RabbitMQRpcServerModel
            {
                Identifier = identifier,
                RoutingKey = routingKey,
                Consumer = consumer,
            };
            this.RpcServers.Add(model);

            // Return
            return identifier;
        }

        /// <summary>
        /// Creates RPC Server and returns RPC Server ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        public override string CreateRpcServer(INanoRpcServerOptions options)
            => NewRpcServer(options.RoutingKey, options.OnRequest);
        public override void RpcServerStartListening(string identifier)
        {
            var model = this.RpcServers.FirstOrDefault(x => x.Identifier == identifier);
            if (model == null) throw new Exception("No RPC Server found!");

            // Action
            this.Session.QueueDeclare(queue: model.RoutingKey, durable: false, exclusive: false, autoDelete: false, arguments: null);
            this.Session.BasicQos(0, 1, false);
            model.Tag = this.Session.BasicConsume(queue: model.RoutingKey, autoAck: false, consumer: model.Consumer);
        }
        public override void RpcServerStopListening(string identifier)
        {
            var model = this.Consumers.FirstOrDefault(x => x.Identifier == identifier);
            if (model == null) throw new Exception("No consumer found!");
            if (string.IsNullOrEmpty(model.Tag)) throw new Exception("No consumer tag found!");

            // Action
            this.Session.BasicCancel(model.Tag);
        }
        public override void DestroyRpcServer(string identifier)
        {
            var model = this.RpcServers.FirstOrDefault(x => x.Identifier == identifier);
            if (model == null) throw new Exception("No RPC Server found!");

            // Action
            this.RpcServers.Remove(model);
            this.RpcServerStopListening(model.Identifier);
        }
        #endregion

    }
}
