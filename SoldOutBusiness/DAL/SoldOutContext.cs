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
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Currency> Currencies { get; set; }

        public SoldOutContext()
        {
            // Turn off the Migrations, (NOT a code first Db)
            //Database.SetInitializer<SoldOutContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Our database convention does not pluralize table names
            modelBuilder.Conventions
                .Remove<PluralizingTableNameConvention>();

            // ProductSubProduct mapping table
            modelBuilder.Entity<Product>()
                .HasMany(p => p.SubProducts)
                .WithMany(p => p.ParentProducts)
                .Map(pp =>
                {
                    pp.MapLeftKey("ParentProductId");
                    pp.MapRightKey("SubProductId");
                    pp.ToTable("ProductSubProduct");
                });

            // ProductCategory mapping table
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Categories)
                .WithMany(p => p.Products)
                .Map(pc =>
                {
                    pc.MapLeftKey("ProductId");
                    pc.MapRightKey("CategoryId");
                    pc.ToTable("ProductCategory");
                });
        }
    }
}
