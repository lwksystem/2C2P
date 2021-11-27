using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using _2C2P.Core.Data;

namespace _2C2P.DataAccess.Models
{
    [Table("Currency")]
    public class CurrencyModel : BaseModel<CurrencyModel>
    {
        [Column("CurrencyCode")]
        [Display(Name = "CurrencyCode")]
        [StringLength(3, MinimumLength = 3)]
        [Required]
        public string CurrencyCode { get; set; }


        [Column("CurrencyName")]
        [Display(Name = "Currency Name")]
        [StringLength(50)]
        public string CurrencyName { get; set; }

    }
}
