using System.ComponentModel.DataAnnotations;

namespace SoldOutBusiness.Models
{
    public class SuspiciousPhrase
    {
        [Key]
        public string Phrase { get; set; }
    }
}
