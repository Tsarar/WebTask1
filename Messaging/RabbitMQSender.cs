using System;
using RabbitMQ.Client;
using WebTask1.Utils;

namespace WebTask1.RabbitMQMessaging
{
    public class RabbitMQSend
    {
        private readonly IConnection _conn;

        private IModel _channelSend;

        private const string EXCHANGE_NAME = "forwebtest.direct";
        private const string QUEUE_NAME = "catchStuff";
        private const string ROUTING_KEY = "candy";

        public RabbitMQSend(IConnection conn)
        {
            _conn = conn;

            ConfigureChannels();
        }

        private void ConfigureChannels()
        {
            if (_channelSend == null)
            {
                _channelSend = _conn.CreateModel();

                _channelSend.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Direct, true);
                _channelSend.QueueDeclare(QUEUE_NAME, true, false, false, null);
                _channelSend.QueueBind(QUEUE_NAME, EXCHANGE_NAME, ROUTING_KEY, null);
            }
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
    }
}
