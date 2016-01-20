namespace SoldOutBusiness.Models
{
    public class SearchCriteria
    {
        public long SearchCriteriaID { get; set; }
        public long SearchID { get; set; }
        public string Criteria { get; set; }
        public string Value { get; set; }

        public virtual Search Search { get; set; }
    }
}
