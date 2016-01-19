using EFSQLTest.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace EFSQLTest.Models
{
    public class SearchContext : DbContext
    {
        public DbSet<Search> Searches { get; set; }
        public DbSet<SearchResult> SearchResults { get; set; }

        public SearchContext()
        {
            // Turn off the Migrations, (NOT a code first Db)
            Database.SetInitializer<SearchContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Our database convention does not pluralize table names
            modelBuilder.Conventions
                .Remove<PluralizingTableNameConvention>();
        }
    }
}
