using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Dto;
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
        public void registerTransaction([FromBody] RegisterDto)
        {
            if (String.IsNullOrEmpty(idSender) || 
                String.IsNullOrEmpty(idReceiver) || 
                sum <= 0 || 
                String.IsNullOrEmpty(currency))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            TransactionDto transaction = new TransactionDto();

            transaction.generatedId = id;
            transaction.idSender = idSender;
            transaction.idReceiver = idReceiver;
            transaction.sum = sum;
            transaction.currency = currency;

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
            
            return transactionList.FirstOrDefault(transaction => transaction.generatedId == uniqueId);
        }

        [HttpGet("Marco")]
        public IActionResult Polo()
        {
            return Ok("Polo");
        }
    }
}
