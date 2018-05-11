using System;

namespace Dto
{
    public class TransactionDto
    {
        public string generatedId { get; set; }
        public string idSender { get; set; }
        public string idReceiver { get; set; }
        public decimal sum { get; set; }
        public string currency { get; set; }
    }
}
