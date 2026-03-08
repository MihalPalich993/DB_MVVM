using Microsoft.EntityFrameworkCore;
using DB_MVVM.Models;

namespace DB_MVVM.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<User> Users { get; set; }      // Новая таблица
        public DbSet<LogEntry> Logs { get; set; }   // Таблица логов

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Теперь база — это просто файл "TradingCompany.db" в папке с программой
            optionsBuilder.UseSqlite("Data Source=TradingCompany.db");
        }

        // МАГИЯ ЛОГИРОВАНИЯ
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Смотрим, какие объекты изменились прямо сейчас
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Deleted || e.State == EntityState.Modified)
                .ToList();

            foreach (var entry in entries)
            {
                var log = new LogEntry
                {
                    Username = CurrentUser.Name,
                    Action = entry.State.ToString(), // Added, Deleted, Modified
                    EntityName = entry.Entity.GetType().Name, // Client или Order
                    Timestamp = DateTime.Now
                };
                Logs.Add(log); // Добавляем запись в таблицу логов
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}