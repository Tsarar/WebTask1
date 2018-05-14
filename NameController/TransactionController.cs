using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using WebTask1.Storages;
using WebTask1.Dto;
using WebTask1.RabbitMQMessaging;

namespace WebTask1.Controllers
{
    [Route("api/v2/[controller]")]
    public class TransactionController : Controller
    {
        private readonly RabbitMQSend _send;
        private readonly TransactionStorage _storage;

        public TransactionController(RabbitMQSend conn, TransactionStorage storage)
        {
            _send = conn;
            _storage = storage;
        }

        [HttpPost("register")]
        public string RegisterTransaction([FromBody] RegisterDto registerDto)
        {
            if (String.IsNullOrEmpty(registerDto.SenderId) || 
                String.IsNullOrEmpty(registerDto.ReceiverId) ||
                registerDto.Sum <= 0 || 
                String.IsNullOrEmpty(registerDto.Currency))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return "Bad request";
            }

            TransactionDto transaction = new TransactionDto
            {
                GeneratedId = Guid.NewGuid().ToString(),
                SenderId = registerDto.SenderId,
                ReceiverId = registerDto.ReceiverId,
                Sum = registerDto.Sum,
                Currency = registerDto.Currency,
                Status = Statuses.New
            };

            _send.WriteMessage(transaction);

            MemoryStream bodyStream = new MemoryStream();
            StreamWriter result = new StreamWriter(bodyStream, new UnicodeEncoding());
            result.Write(String.Format($"Transaction added, unique id - {transaction.GeneratedId}"));
                
            Response.Body = bodyStream;

            Response.StatusCode = (int)HttpStatusCode.OK;
            return transaction.GeneratedId;
        }

        [HttpGet("getall")]
        public List<TransactionDto> GetAllTransations()
        {
            return _storage.GetAll();
        }

        [HttpGet("getbyid")]
        public TransactionDto GetTransactionById([FromQuery] string uniqueId)
        {
            if (String.IsNullOrEmpty(uniqueId)) return null;
            
            return _storage.GetById(uniqueId);
        }

        [HttpGet("Marco")]
        public IActionResult Polo()
        {
            return Ok("Polo");
        }
    }
}
