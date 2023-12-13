using Microsoft.EntityFrameworkCore;

namespace REST.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Item> Items => Set<Item>();

        public ApplicationContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost;Database=rest;Trusted_Connection=True;TrustServerCertificate=True");
        }
    }
}
