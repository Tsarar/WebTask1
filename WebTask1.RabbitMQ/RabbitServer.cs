using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using WebTask1.Dto;
using WebTask1.Utils;

namespace WebTask1.RabbitMQ
{
    public class RabbitServer : IDisposable, IHostedService
    {
        private const string EXCHANGE_NAME = "forwebtest.direct";

        private const string QUEUE_NAME_PROCESSED = "processed.issues";
        private const string ROUTING_KEY_PROCESSED = "processed";

        private const string QUEUE_NAME_NEW = "new.issues";
        private const string ROUTING_KEY_NEW = "new";

        private readonly IConnection _conn;

        private IModel _channelSend;
        private IModel _channelReceive;

        private EventingBasicConsumer _consumer;

        public RabbitServer()
        {
            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = "cdkxtrhe",
                Password = "ALMkgEdwWx2M6ECRhqnnQPsTPgGDpz5U",
                VirtualHost = "cdkxtrhe",
                HostName = "sheep-01.rmq.cloudamqp.com"
            };

            _conn = factory.CreateConnection();
        }

        public void Dispose()
        {
            if (_conn.IsOpen) _conn.Close();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ConfigurateOperator();
            CreateMessageConsumer();

            _channelReceive.BasicConsume(queue: QUEUE_NAME_NEW,
                autoAck: true,
                consumer: _consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_conn.IsOpen) _conn.Close();

            return Task.CompletedTask;
        }

        private void ConfigurateOperator()
        {
            if (_channelSend == null)
            {
                _channelSend = _conn.CreateModel();

                _channelSend.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Direct, true);
                _channelSend.QueueDeclare(QUEUE_NAME_PROCESSED, true, false, false, null);
                _channelSend.QueueBind(QUEUE_NAME_PROCESSED, EXCHANGE_NAME, ROUTING_KEY_PROCESSED, null);
            }

            if (_channelReceive == null)
            {
                _channelReceive = _conn.CreateModel();

                _channelReceive.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Direct, true);
                _channelReceive.QueueDeclare(QUEUE_NAME_NEW, true, false, false, null);
                _channelReceive.QueueBind(QUEUE_NAME_NEW, EXCHANGE_NAME, ROUTING_KEY_NEW, null);
            }
        }

        public void CreateMessageConsumer()
        {
           _consumer = new EventingBasicConsumer(_channelReceive);
           _consumer.Received += RedirectMessageWithNewStatus;
        }

        public bool WriteMessage(object obj)
        {
            try
            {
                byte[] messageBodyBytes = ConvertUtils.ConvertObjectToJsonByteArray(obj);
                _channelSend.BasicPublish(EXCHANGE_NAME, ROUTING_KEY_PROCESSED, null, messageBodyBytes);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void RedirectMessageWithNewStatus(object model, BasicDeliverEventArgs eventArgs)
        {
            var random = new Random();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TransactionDto));

            using (MemoryStream body = new MemoryStream(eventArgs.Body))
            {
                try
                {
                    TransactionDto transaction = (TransactionDto)serializer.ReadObject(body);

                    transaction.Status = random.Next(0, 2) == 0 ? TransactionStatus.Filled : TransactionStatus.Rejected;

                    WriteMessage(transaction);
                }
                catch (Exception)
                {
                    //invalid serialization object
                }
            }
        }
    }
}
