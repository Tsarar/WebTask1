using System.Runtime.Serialization;

namespace WebTask1.Dto
{
    [DataContract]
    public class TransactionDto
    {
        [DataMember]
        public string GeneratedId { get; set; }

        [DataMember]
        public string IdSender { get; set; }

        [DataMember]
        public string IdReceiver { get; set; }

        [DataMember]
        public decimal Sum { get; set; }

        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public string Status { get; set; }
    }
}
