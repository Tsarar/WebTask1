using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using WebTask1.Dto;
using WebTask1.Rabbit;

namespace WebTask1.Controllers
{
    [Route("api/v2/[controller]")]
    public class TransactionController : Controller
    {
        private static readonly List<TransactionDto> TransactionList = new List<TransactionDto>();
        private static string _id = "0";
        private static RabbitOperations _conn;

        public TransactionController(RabbitOperations conn)
        {
            _conn = conn;
        }

        [HttpPost("register")]
        public string RegisterTransaction([FromBody] RegisterDto registerDto)
        {
            if (String.IsNullOrEmpty(registerDto.IdSender) || 
                String.IsNullOrEmpty(registerDto.IdReceiver) ||
                registerDto.Sum <= 0 || 
                String.IsNullOrEmpty(registerDto.Currency))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return "Bad request";
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

            _conn.WriteMessage(transaction);

            MemoryStream bodyStream = new MemoryStream();
            StreamWriter result = new StreamWriter(bodyStream, new UnicodeEncoding());
            result.Write(String.Format($"Transaction added, unique id - {_id}"));
                
            Response.Body = bodyStream;

            _id = (Convert.ToInt32(_id) + 1).ToString();

            Response.StatusCode = (int)HttpStatusCode.OK;
            return transaction.GeneratedId;
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
            return _conn.GetMessages();
        }

        [HttpGet("getbyid")]
        public TransactionDto GetTransactionById([FromQuery] string uniqueId)
        {
            if (String.IsNullOrEmpty(uniqueId)) return null;
            
            return TransactionList.FirstOrDefault(transaction => transaction.GeneratedId == uniqueId);
        }

        [HttpGet("Marco")]
        public IActionResult Polo()
        {
            return Ok("Polo");
        }
    }
}
