using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using WebTask1.Dto;

namespace WebTask1.Controllers
{
    [Route("api/v2/[controller]")]
    public class TransactionController : Controller
    {
        static readonly List<TransactionDto> TransactionList = new List<TransactionDto>();
        static string _id = "0";

        public TransactionController()
        {

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

            TransactionDto transaction = new TransactionDto()
            {
                GeneratedId = _id,
                IdSender = registerDto.IdSender,
                IdReceiver = registerDto.IdReceiver,
                Sum = registerDto.Sum,
                Currency = registerDto.Currency
            };

            TransactionList.Add(transaction);

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
