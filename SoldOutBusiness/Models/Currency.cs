using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoldOutBusiness.Models
{
    public class Ccy
    {   
        [Key]
        public string Currency { get; set; }        
    }
}
