using System.ComponentModel.DataAnnotations;

namespace SoldOutBusiness.Models
{
    public class Ccy
    {   
        [Key]
        public string Currency { get; set; }        
    }
}
