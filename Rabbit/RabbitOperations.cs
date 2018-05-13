using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using RabbitMQ.Client;
using WebTask1.Dto;
using WebTask1.Utils;

namespace WebTask1.Rabbit
{
    public class RabbitOperations
    {
        private static IConnection _conn;
        private static IModel _channelSend;
        private static IModel _channelReceive;

        private const string ExchangeName = "forwebtest.direct";
        private const string QueryName = "catchStuff";
        private const string RoutingKey = "candy";

        public RabbitOperations(IConnection conn)
        {
            _conn = conn;

            ConfigurateOperator();
        }

        private void ConfigurateOperator()
        {
            _channelSend = _conn.CreateModel();
            _channelReceive = _conn.CreateModel();

            _channelSend.ExchangeDeclare(ExchangeName, ExchangeType.Direct, true);
            _channelSend.QueueDeclare(QueryName, true, false, false, null);
            _channelSend.QueueBind(QueryName, ExchangeName, RoutingKey, null);

            _channelSend.ExchangeDeclare(ExchangeName, ExchangeType.Direct, true);
            _channelSend.QueueDeclare(QueryName, true, false, false, null);
            _channelSend.QueueBind(QueryName, ExchangeName, RoutingKey, null);
        }

        public bool WriteMessage(object obj)
        {
            try
            {
                byte[] messageBodyBytes = ConvertUtils.ConvertObjectToJsonByteArray(obj);
                _channelSend.BasicPublish(ExchangeName, RoutingKey, null, messageBodyBytes);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public List<TransactionDto> GetMessages()
        {
            List<TransactionDto> results = new List<TransactionDto>();
            Random random = new Random();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TransactionDto));
            BasicGetResult askResult = _channelReceive.BasicGet(QueryName, false);

            while (askResult != null)
            {
                // acknowledge receipt of the message
                _channelReceive.BasicAck(askResult.DeliveryTag, false);

                using (MemoryStream body = new MemoryStream(askResult.Body))
                {
                    try
                    {
                        TransactionDto transaction = (TransactionDto)serializer.ReadObject(body);

                        transaction.Status = random.Next(0, 2) == 0 ? "Filled" : "Rejected";

                        results.Add(transaction);
                    }
                    catch (Exception)
                    {
                        //invalid serialization object
                    }
                }
                askResult = _channelReceive.BasicGet(QueryName, false);
            }

            return results;
        }
    }
}
