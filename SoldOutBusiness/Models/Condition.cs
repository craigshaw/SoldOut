using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoldOutBusiness.Models
{
    public class Condition
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Not auto incrementing
        public int ConditionId { get; set; }
        public string Description { get; set; }
        public int eBayConditionId { get; set; }
    }
}
