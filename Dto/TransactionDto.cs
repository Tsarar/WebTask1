namespace WebTask1.Dto
{
    public class TransactionDto
    {
        public string GeneratedId { get; set; }
        public string IdSender { get; set; }
        public string IdReceiver { get; set; }
        public decimal Sum { get; set; }
        public string Currency { get; set; }
    }
}
