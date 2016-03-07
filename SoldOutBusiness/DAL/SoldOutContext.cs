using SoldOutBusiness.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SoldOutBusiness.DAL
{
    public class SoldOutContext : DbContext
    {
        public DbSet<Search> Searches { get; set; }
        public DbSet<SearchResult> SearchResults { get; set; }
        public DbSet<SearchCriteria> SearchCriteria { get; set; }
        public DbSet<SuspiciousPhrase> SuspiciousPhrases { get; set; }
        public DbSet<SearchSuspiciousPhrase> SearchSuspiciousPhrases { get; set; }
        public DbSet<Condition> Conditions { get; set; }

        public SoldOutContext()
        {
            // Turn off the Migrations, (NOT a code first Db)
            Database.SetInitializer<SoldOutContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Our database convention does not pluralize table names
            modelBuilder.Conventions
                .Remove<PluralizingTableNameConvention>();
        }
    }
}
