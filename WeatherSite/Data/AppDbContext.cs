using Microsoft.EntityFrameworkCore;
using WeatherSite.Models;


namespace WeatherSite.Data
{
    public class AppDbContext : DbContext
    {
        // Конструктор для конфигурации
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }
        // Связываем модель Report с таблицей Reports в БД
        public DbSet<Report> Reports { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Report>().HasKey(report => new { report.Date, report.Time });
        }
    }
}
