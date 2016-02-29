using SoldOutBusiness.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SoldOutBusiness.DAL
{
    public class ProductContext: DbContext
    {
        public DbSet<Product> Products { get; set; }

        public DbSet<Product> RelatedProducts { get; set; }

        public ProductContext()
        {
            // Turn off the Migrations, (NOT a code first Db)
            Database.SetInitializer<ProductContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Our database convention does not pluralize table names
            modelBuilder.Conventions
                .Remove<PluralizingTableNameConvention>();
        }
    }
}
