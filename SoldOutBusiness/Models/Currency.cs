using System.ComponentModel.DataAnnotations;

namespace SoldOutBusiness.Models
{
    public class Currency
    {   
        [Key]
        public string CurrencyCode { get; set; }        
    }
}
