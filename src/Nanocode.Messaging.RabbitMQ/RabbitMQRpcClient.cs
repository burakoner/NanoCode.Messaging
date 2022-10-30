using System.Threading.Channels;

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
            this._callbackMapper = new ConcurrentDictionary<string, TaskCompletionSource<INanoRpcResponse>>();

            // Declare a Server-Named Queue
            this._replyQueueName = this._session.QueueDeclare(queue: "").QueueName;

            // Consumer
            this._consumer = new EventingBasicConsumer(this._session);
            this._consumer.Received += (model, ea) =>
            {
                if (!_callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out TaskCompletionSource<INanoRpcResponse> tcs))
                    return;

                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                var responseObject = JsonConvert.DeserializeObject<RabbitMQRpcResponseModel>(response);
                tcs.TrySetResult(responseObject);
            };

            // Create Consumer
            this._session.BasicConsume(consumer: _consumer, queue: _replyQueueName, autoAck: true);
        }

        internal Task<INanoRpcResponse> CallAsync(INanoRpcRequest request, CancellationToken ct = default)
        {
            // Arrange
            var props = this._session.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = _replyQueueName;
            var json = JsonConvert.SerializeObject(request);
            var bytes = Encoding.UTF8.GetBytes(json);
            var tcs = new TaskCompletionSource<INanoRpcResponse>();
            this._callbackMapper.TryAdd(correlationId, tcs);

            // Send Request
            this._session.BasicPublish(exchange: "", routingKey: this._routingKey, basicProperties: props, body: bytes);

            // Cancellation Token
            ct.Register(() => this._callbackMapper.TryRemove(correlationId, out var tmp));

            // Return
            return tcs.Task;
        }
    }
}
