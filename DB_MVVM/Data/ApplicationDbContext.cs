using DB_MVVM.Models;
using Microsoft.EntityFrameworkCore;

namespace DB_MVVM.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TradingCompanyDB;Trusted_Connection=True;");
        }
    }
}