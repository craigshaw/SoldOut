using System;

namespace SoldOutWeb.Models
{
    public class Product
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long SearchID { get; set; }
        public int ProductID { get; set; }
        public DateTime LastRun { get; set; }
        public int TotalResults { get; set; }
        public int ConditionID { get; set; }

        public string ConditionName {
            get {
                if (ConditionID == 2)
                    return "New";
                else
                    return "Used"; }
        }
    }
}