using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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

        private EventingBasicConsumer _consumer;

        private readonly TransactionStorage _storage;

        private readonly string _exchangeName;
        private readonly string _queueNameProcessed;
        private readonly string _routingKeyProcessed;

        public RabbitMQReceiver(IConnection conn, TransactionStorage storage, IConfiguration configuration)
        {
            _conn = conn;
            _storage = storage;

            _exchangeName = configuration["RabbitMQ:ExchangeName"];
            _queueNameProcessed = configuration["RabbitMQ:QueueProcessed"];
            _routingKeyProcessed = configuration["RabbitMQ:RoutingKeyProcessed"];
        }

        private void ConfigureChannels()
        {
            if (_channelReceive == null)
            {
                _channelReceive = _conn.CreateModel();

                _channelReceive.ExchangeDeclare(_exchangeName, ExchangeType.Direct, true);
                _channelReceive.QueueDeclare(_queueNameProcessed, true, false, false, null);
                _channelReceive.QueueBind(_queueNameProcessed, _exchangeName, _routingKeyProcessed, null);
            }
        }

        public void CreateMessageConsumer()
        {
            _consumer = new EventingBasicConsumer(_channelReceive);
            _consumer.Received += (model, ea) =>
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

            _channelReceive.BasicConsume(queue: _queueNameProcessed,
                                         autoAck: true,
                                         consumer: _consumer);
        }

        public void Dispose()
        {
            if (_conn.IsOpen) _conn.Close();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ConfigureChannels();
            CreateMessageConsumer();

            _channelReceive.BasicConsume(queue: _queueNameProcessed,
                                         autoAck: true,
                                         consumer: _consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_conn.IsOpen) _conn.Close();

            return Task.CompletedTask;
        }
    }
}

