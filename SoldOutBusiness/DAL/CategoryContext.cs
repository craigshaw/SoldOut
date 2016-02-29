using SoldOutBusiness.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SoldOutBusiness.DAL
{
    public class CategoryContext: DbContext
    {
        public DbSet<Category> Categories { get; set; }

        public CategoryContext()
        {
            // Turn off the Migrations, (NOT a code first Db)
            Database.SetInitializer<CategoryContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Our database convention does not pluralize table names
            modelBuilder.Conventions
                .Remove<PluralizingTableNameConvention>();
        }
    }
}
