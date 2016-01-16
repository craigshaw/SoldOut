using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoldOutBusiness.Models
{
    [Serializable]
    public class SoldItem
    {
        public double FinalValue { get; set; }
        public DateTime EndTime { get; set; }
        public string ItemID { get; set; }
        public string Description { get; set; }
        public string ItemURL { get; set; }
    }
}
