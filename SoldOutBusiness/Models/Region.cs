using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoldOutBusiness.Models
{
    public class Region
    {
        [Key]
        public string Name { get; set; }
    }
}
