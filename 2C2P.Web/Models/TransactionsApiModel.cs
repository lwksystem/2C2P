using System;

namespace _2C2P.Web.Models
{
    public class TransactionSearchRequest
    {
        public string CurrencyCode { get; set; }
        public DateTime? TransactionDateFrom { get; set; }

        public DateTime? TransactionDateTo { get; set; }
        public string Status { get; set; }
    }

    public class TransactionSearchResult
    {
        public string id { get; set; }
        public string payment { get; set; }
        public string Status { get; set; }
    }
}
