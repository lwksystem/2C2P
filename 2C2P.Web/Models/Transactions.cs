using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _2C2P.Web.Models
{
    [Serializable, XmlRoot("Transactions")]
    public class Transaction
    {
        [XmlAttribute("id")]
        public string id { get; set; }
        public DateTime TransactionDate { get; set; }
        public PaymentDetails PaymentDetails { get; set; }
        public string Status { get; set; }
    }

    public class PaymentDetails
    {
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
    }
}
