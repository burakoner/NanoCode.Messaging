namespace Nanocode.Messaging.RabbitMQ
{
    public class RabbitMQRpcClient
    {
        private readonly IModel _session;
        private readonly string _replyQueueName;
        private readonly string _routingKey;
        private readonly EventingBasicConsumer _consumer;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<INanoRpcResponse>> _callbackMapper;

        internal RabbitMQRpcClient(IModel session, string rpcRoutingKey)
        {
            // Arrange
            this._session = session;
            this._routingKey = rpcRoutingKey;
            this._replyQueueName = Guid.NewGuid().ToString();
            this._callbackMapper = new ConcurrentDictionary<string, TaskCompletionSource<INanoRpcResponse>>();

            // Declare a Guid-Named Queue
            this._session.QueueDeclare(
               queue: this._replyQueueName,
               durable: false,
               exclusive: true,
               autoDelete: true);
            this._consumer = new EventingBasicConsumer(this._session);
            this._consumer.Received += (model, ea) =>
            {
                if (!_callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out TaskCompletionSource<INanoRpcResponse> tcs))
                    return;
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                var responseObject = JsonConvert.DeserializeObject<INanoRpcResponse>(response);
                tcs.TrySetResult(responseObject);
            };

            // Create Consumer
            var _consumerTag = this._session.BasicConsume(
                consumer: _consumer,
                queue: _replyQueueName,
                autoAck: true);
        }

        internal Task<INanoRpcResponse> CallAsync(INanoRpcRequest request, CancellationToken ct = default)
        {
            // Arrange
            var props = this._session.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = _replyQueueName;
            var json = JsonConvert.SerializeObject(request);
            var data = Encoding.UTF8.GetBytes(json);
            var tcs = new TaskCompletionSource<INanoRpcResponse>();
            this._callbackMapper.TryAdd(correlationId, tcs);

            // Send Request
            this._session.BasicPublish(
                exchange: "",
                routingKey: this._routingKey,
                basicProperties: props,
                body: data);

            // Cancellation Token
            ct.Register(() => this._callbackMapper.TryRemove(correlationId, out var tmp));

            // Return
            return tcs.Task;
        }
    }
}
