using System;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using WebTask1.Utils;

namespace WebTask1.RabbitMQMessaging
{
    public class RabbitMQSend
    {
        private readonly IConnection _conn;

        private IModel _channelSend;

        private readonly string _exchangeName;
        private readonly string _queueNew;
        private readonly string _routingKeyNew;

        public RabbitMQSend(IConnection conn, IConfiguration config)
        {
            _conn = conn;

            _exchangeName = config["RabbitMQ:ExchangeName"];
            _queueNew = config["RabbitMQ:QueueNew"];
            _routingKeyNew = config["RabbitMQ:RoutingKeyNew"];

            ConfigureChannels();
        }

        private void ConfigureChannels()
        {
            if (_channelSend == null)
            {
                _channelSend = _conn.CreateModel();

                _channelSend.ExchangeDeclare(_exchangeName, ExchangeType.Direct, true);
                _channelSend.QueueDeclare(_queueNew, true, false, false, null);
                _channelSend.QueueBind(_queueNew, _exchangeName, _routingKeyNew, null);
            }
        }

        public bool WriteMessage(object obj)
        {
            try
            {
                byte[] messageBodyBytes = ConvertUtils.ConvertObjectToJsonByteArray(obj);
                _channelSend.BasicPublish(_exchangeName, _routingKeyNew, null, messageBodyBytes);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
