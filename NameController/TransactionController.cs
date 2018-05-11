using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebTask1.Dto;
using System.Net;
using System.Linq;

namespace Controllers
{
    [Route("api/v2/[controller]")]
    public class TransactionController : Controller
    {
        static readonly List<TransactionDto> transactionList = new List<TransactionDto>();
        static string id = "0";

        public TransactionController()
        {

        }

        [HttpPost("register")]
        public void registerTransaction([FromBody] RegisterDto registerDto)
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
                GeneratedId = id,
                IdSender = registerDto.IdSender,
                IdReceiver = registerDto.IdReceiver,
                Sum = registerDto.Sum,
                Currency = registerDto.Currency
            };

            transactionList.Add(transaction);
            string result = String.Format($"Transaction added, unique id - {id}");

            id = (Convert.ToInt32(id) + 1).ToString();

            Response.StatusCode = (int)HttpStatusCode.OK;
        }

        [HttpGet("getall")]
        public List<TransactionDto> getAllTransations()
        {
            if (transactionList.Count == 0) return null;

            return transactionList;
        }

        [HttpGet("getbyid")]
        public TransactionDto getTransactionByID([FromQuery] string uniqueId)
        {
            if (String.IsNullOrEmpty(uniqueId)) return null;
            
            return transactionList.FirstOrDefault(transaction => transaction.GeneratedId == uniqueId);
        }

        [HttpGet("Marco")]
        public IActionResult Polo()
        {
            return Ok("Polo");
        }
    }
}
