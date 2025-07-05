using Microsoft.EntityFrameworkCore;

namespace PruebaTecnWPF.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Crud.db");
        }
    }
}