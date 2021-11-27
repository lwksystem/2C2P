using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using _2C2P.Core.Data;

namespace _2C2P.DataAccess.Models
{
    [Table("Transactions")]
    public class TransactionsModel : BaseModel<TransactionsModel>
    {
        [Column("TransactionId")]
        [Display(Name = "Transaction Id")]
        [StringLength(50)]
        [Required]
        public string TransactionId { get; set; }

        [Column("TransactionAmount")]
        [Display(Name = "Transaction Amount")]
        [Required]
        public decimal TransactionAmount { get; set; }

        [Column("CurrencyCode")]
        [Display(Name = "CurrencyCode")]
        [StringLength(3, MinimumLength =3)]
        [Required]
        public string CurrencyCode { get; set; }


        [Column("TransactionDate")]
        [Display(Name = "Transaction Date")]
        [Required]
        public DateTime TransactionDate { get; set; }

        [Column("FileType")]
        [Display(Name = "File Type")]
        [StringLength(3, MinimumLength = 3)]
        [Required]
        public string FileType { get; set; }

        [Column("InputStatus")]
        [Display(Name = "Input Status")]
        [StringLength(10)]
        [Required]
        public string InputStatus { get; set; }

        [Column("OutputStatus")]
        [Display(Name = "Output Status")]
        [StringLength(1, MinimumLength = 1)]
        [Required]
        public string OutputStatus { get; set; }

        


    }
}
