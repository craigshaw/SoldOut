using System.Collections.Generic;

namespace SoldOutBusiness.Models
{
    /// <summary>
    /// A catalogue is a collection of searches that we're interested in
    /// </summary>
    public class Catalogue
    {
        public IList<Search> Searches { get; set; }
    }
}
