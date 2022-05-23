using Microsoft.EntityFrameworkCore;

namespace TelegramBotWebhook
{
    public class MPEISessionDbContext : DbContext
    {
        public virtual DbSet<MPEISession> MPEISessions { get; set; } = null!;

        public MPEISessionDbContext()
        {
            Database.EnsureCreated();
        }
        public MPEISessionDbContext(DbContextOptions<MPEISessionDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MPEISession>(e =>
            {
                e.Property(s => s.UserId);
            });
        }
    }
}
