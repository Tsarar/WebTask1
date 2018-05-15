using System;
using System.Collections.Generic;
using System.Net;
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
                Status = TransactionStatus.New
            };

            _send.WriteMessage(transaction);
            
            return transaction.GeneratedId;
        }

        [HttpGet("getall")]
        public List<TransactionDto> GetAllTransations()
        {
            return _storage.GetAll();
        }

        [HttpGet("getbyid")]
        public TransactionDto GetTransactionById([FromQuery] string id)
        {
            if (String.IsNullOrEmpty(id)) return null;
            
            return _storage.GetById(id);
        }

        [HttpGet("Marco")]
        public IActionResult Polo()
        {
            return Ok("Polo");
        }
    }
}
