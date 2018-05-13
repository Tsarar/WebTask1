using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using WebTask1.Dto;
using RabbitMQ.Client;

namespace WebTask1.Controllers
{
    [Route("api/v2/[controller]")]
    public class TransactionController : Controller
    {
        private static readonly List<TransactionDto> TransactionList = new List<TransactionDto>();
        private static string _id = "0";
        private static IConnection _conn;
        private static IModel _channel;

        public TransactionController(IConnection conn)
        {
            _conn = conn;

            ConfigurateController();
        }

        private void ConfigurateController()
        {
            _channel = _conn.CreateModel();

            _channel.ExchangeDeclare("forwebtest.direct", ExchangeType.Direct, true);
            _channel.QueueDeclare("catchStuff", true, false, false, null);
            _channel.QueueBind("catchStuff", "forwebtest.direct", "candy", null);
        }

        [HttpPost("register")]
        public void RegisterTransaction([FromBody] RegisterDto registerDto)
        {
            if (String.IsNullOrEmpty(registerDto.IdSender) || 
                String.IsNullOrEmpty(registerDto.IdReceiver) ||
                registerDto.Sum <= 0 || 
                String.IsNullOrEmpty(registerDto.Currency))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            TransactionDto transaction = new TransactionDto
            {
                GeneratedId = _id,
                IdSender = registerDto.IdSender,
                IdReceiver = registerDto.IdReceiver,
                Sum = registerDto.Sum,
                Currency = registerDto.Currency,
                Status = "New"
            };

            TransactionList.Add(transaction);

            byte[] messageBodyBytes = ConvertUtils.ConvertObjectToJsonByteArray(transaction);
            _channel.BasicPublish("forwebtest.direct", "candy", null, messageBodyBytes);

            MemoryStream bodyStream = new MemoryStream();
            StreamWriter result = new StreamWriter(bodyStream, new UnicodeEncoding());
            result.Write(String.Format($"Transaction added, unique id - {_id}"));
                
            Response.Body = bodyStream;

            _id = (Convert.ToInt32(_id) + 1).ToString();

            Response.StatusCode = (int)HttpStatusCode.OK;
        }

        [HttpGet("getall")]
        public List<TransactionDto> GetAllTransations()
        {
            if (TransactionList.Count == 0) return null;

            return TransactionList;
        }

        [HttpGet("getallrabbit")]
        public List<TransactionDto> GetAllTransationsRabbit()
        {
            List<TransactionDto> results = new List<TransactionDto>();
            Random random = new Random();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TransactionDto));
            BasicGetResult askResult = _channel.BasicGet("catchStuff", false);

            while (askResult != null)
            {
                // acknowledge receipt of the message
                _channel.BasicAck(askResult.DeliveryTag, false);

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
                askResult = _channel.BasicGet("catchStuff", false);
            }

            return results;
        }

        [HttpGet("getbyid")]
        public TransactionDto GetTransactionById([FromQuery] string uniqueId)
        {
            if (String.IsNullOrEmpty(uniqueId)) return null;
            
            return TransactionList.FirstOrDefault(transaction => transaction.GeneratedId == uniqueId);
        }

        [HttpGet("getonerabbit")]
        public TransactionDto GetOneTransactionRabbit()
        {
            TransactionDto result = new TransactionDto();
            Random random = new Random();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TransactionDto));
            BasicGetResult askResult = _channel.BasicGet("catchStuff", false);

            if (askResult == null) { return result; }
            // acknowledge receipt of the message
            _channel.BasicAck(askResult.DeliveryTag, false);

            using (MemoryStream body = new MemoryStream(askResult.Body))
            {
                try
                {
                    result = (TransactionDto)serializer.ReadObject(body);

                    result.Status = random.Next(0, 1) == 0 ? "Filled" : "Rejected";
                }
                catch (Exception)
                {
                    //invalid serialization object
                }
            }

            return result;
        }

        [HttpGet("Marco")]
        public IActionResult Polo()
        {
            return Ok("Polo");
        }
    }
}
