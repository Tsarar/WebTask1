using System.Runtime.Serialization;

namespace WebTask1.Dto
{
    public enum TransactionStatus
    {
        New,
        Filled,
        Rejected
    }

    [DataContract]
    public class TransactionDto
    {
        [DataMember]
        public string GeneratedId { get; set; }

        [DataMember]
        public string SenderId { get; set; }

        [DataMember]
        public string ReceiverId { get; set; }

        [DataMember]
        public decimal Sum { get; set; }

        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public TransactionStatus Status { get; set; }
    }
}
