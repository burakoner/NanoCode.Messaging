using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using NanoCode.Messaging.RabbitMQ.Models;
using Newtonsoft.Json;
using NanoCode.Messaging.Models;

namespace NanoCode.Messaging.RabbitMQ
{
    public class RabbitMQNanoRpcClient
    {
        private readonly IModel _session;
        private readonly string _replyQueueName;
        private readonly string _routingKey;
        private readonly EventingBasicConsumer _consumer;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<NanoRpcResponse>> _callbackMapper;

        internal RabbitMQNanoRpcClient(IModel session, string rpcRoutingKey)
        {
            // Arrange
            this._session = session;
            this._routingKey = rpcRoutingKey;
            this._replyQueueName = Guid.NewGuid().ToString();
            this._callbackMapper = new ConcurrentDictionary<string, TaskCompletionSource<NanoRpcResponse>>();

            // Declare a Guid-Named Queue
            this._session.QueueDeclare(
               queue: this._replyQueueName,
               durable: false,
               exclusive: true,
               autoDelete: true);
            this._consumer = new EventingBasicConsumer(this._session);
            this._consumer.Received += (model, ea) =>
            {
                if (!_callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out TaskCompletionSource<NanoRpcResponse> tcs))
                    return;
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                var responseObject = JsonConvert.DeserializeObject<NanoRpcResponse>(response);
                tcs.TrySetResult(responseObject);
            };

            // Create Consumer
            var _consumerTag = this._session.BasicConsume(
                consumer: _consumer,
                queue: _replyQueueName,
                autoAck: true);
        }

        internal Task<NanoRpcResponse> CallAsync(NanoRpcRequest request, CancellationToken ct = default)
        {
            // Arrange
            var props = this._session.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = _replyQueueName;
            var json = JsonConvert.SerializeObject(request);
            var data = Encoding.UTF8.GetBytes(json);
            var tcs = new TaskCompletionSource<NanoRpcResponse>();
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
