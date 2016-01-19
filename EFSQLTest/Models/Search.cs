using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFSQLTest.Models
{
    public class Search
    {
        public Search()
        {
            SearchResults = new List<SearchResult>();
        }

        public long SearchId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<SearchResult> SearchResults { get; set; }
    }
}
