using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WebTask1.Dto;
using WebTask1.Storages;

namespace WebTask1.Messaging
{
    public class RabbitMQReceiver : IDisposable, IHostedService
    {
        private readonly IConnection _conn;

        private IModel _channelReceive;

        private EventingBasicConsumer consumer;

        private readonly TransactionStorage _storage;

        private const string EXCHANGE_NAME = "forwebtest.direct";
        private const string QUEUE_NAME = "notNew";
        private const string ROUTING_KEY = "watermelon";

        public RabbitMQReceiver(IConnection conn, TransactionStorage storage)
        {
            _conn = conn;
            _storage = storage;
        }

        private void ConfigureChannels()
        {
            if (_channelReceive == null)
            {
                _channelReceive = _conn.CreateModel();

                _channelReceive.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Direct, true);
                _channelReceive.QueueDeclare(QUEUE_NAME, true, false, false, null);
                _channelReceive.QueueBind(QUEUE_NAME, EXCHANGE_NAME, ROUTING_KEY, null);
            }
        }

        public void CreateMessageConsumer()
        {
            consumer = new EventingBasicConsumer(_channelReceive);
            consumer.Received += (model, ea) =>
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TransactionDto));

                using (MemoryStream body = new MemoryStream(ea.Body))
                {
                    try
                    {
                        TransactionDto transaction = (TransactionDto)serializer.ReadObject(body);

                        _storage.AddTransaction(transaction);
                    }
                    catch (Exception)
                    {
                        //invalid serialization object
                    }
                }
            };

            _channelReceive.BasicConsume(queue: QUEUE_NAME,
                                         autoAck: true,
                                         consumer: consumer);
        }

        public void Dispose()
        {
            if (_conn.IsOpen) _conn.Close();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ConfigureChannels();
            CreateMessageConsumer();

            _channelReceive.BasicConsume(queue: QUEUE_NAME,
                                         autoAck: true,
                                         consumer: consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_conn.IsOpen) _conn.Close();

            return Task.CompletedTask;
        }
    }
}

