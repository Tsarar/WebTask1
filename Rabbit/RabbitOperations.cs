using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WebTask1.Dto;
using WebTask1.Utils;
using WebTask1.Storages;

namespace WebTask1.Rabbit
{
    /*public class RabbitOperations : IDisposable
    {
        private readonly IConnection _conn;
        private readonly TransactionStorage _storage;

        private IModel _channelSend;
        private IModel _channelReceive;

        private const string EXCHANGE_NAME = "forwebtest.direct";
        private const string QUERY_NAME = "catchStuff";
        private const string ROUTING_KEY = "candy";

        public RabbitOperations(IConnection conn, TransactionStorage storage)
        {
            _conn = conn;
            _storage = storage;

            ConfigurateOperator();
            CreateMessageConsumer();
        }

        private void ConfigurateOperator()
        {
            _channelSend = _conn.CreateModel();
            _channelReceive = _conn.CreateModel();

            _channelSend.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Direct, true);
            _channelSend.QueueDeclare(QUERY_NAME, true, false, false, null);
            _channelSend.QueueBind(QUERY_NAME, EXCHANGE_NAME, ROUTING_KEY, null);

            _channelReceive.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Direct, true);
            _channelReceive.QueueDeclare(QUERY_NAME, true, false, false, null);
            _channelReceive.QueueBind(QUERY_NAME, EXCHANGE_NAME, ROUTING_KEY, null);
        }

        public bool WriteMessage(object obj)
        {
            try
            {
                byte[] messageBodyBytes = ConvertUtils.ConvertObjectToJsonByteArray(obj);
                _channelSend.BasicPublish(EXCHANGE_NAME, ROUTING_KEY, null, messageBodyBytes);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void CreateMessageConsumer()
        {
            var consumer = new EventingBasicConsumer(_channelReceive);
            consumer.Received += (model, ea) =>
            {
                var random = new Random();
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TransactionDto));

                using (MemoryStream body = new MemoryStream(ea.Body))
                {
                    try
                    {
                        TransactionDto transaction = (TransactionDto)serializer.ReadObject(body);

                        transaction.Status = random.Next(0, 2) == 0 ? Statuses.Filled : Statuses.Rejected;

                        _storage.AddTransaction(transaction);

                        _channelReceive.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception)
                    {
                        //invalid serialization object
                    }
                }
            };
        }

        public void Dispose()
        {
            if (_conn.IsOpen) _conn.Close();
        }
    }*/
}
