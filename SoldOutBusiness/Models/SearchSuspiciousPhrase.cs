using System.ComponentModel.DataAnnotations;

namespace SoldOutBusiness.Models
{
    public class SearchSuspiciousPhrase
    {
        [Key]
        public string Phrase { get; set; }

        public long SearchId { get; set; }
    }
}
