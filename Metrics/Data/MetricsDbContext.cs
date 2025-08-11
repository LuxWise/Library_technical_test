using Metrics.Model;
using Microsoft.EntityFrameworkCore;

namespace Metrics.Data
{
    public class MetricsDbContext : DbContext
    {
        public MetricsDbContext(DbContextOptions<MetricsDbContext> options)
            : base(options)
        {
        }

        public DbSet<LogFile> LogFile { get; set; }
        public DbSet<LogEntry> LogEntry { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogFile>(b =>
            {
                b.ToTable("log_files");
                b.HasKey(x => x.Id);
                b.Property(x => x.FileName).IsRequired().HasMaxLength(256);
                b.Property(x => x.Hash).HasMaxLength(64);
                b.Property(x => x.Status).HasMaxLength(32);
                b.HasIndex(x => x.FileName).IsUnique(false);
            });

            modelBuilder.Entity<LogEntry>(b =>
            {
                b.ToTable("log_entries");
                b.HasKey(x => x.Id);
                b.Property(x => x.Level).HasMaxLength(16);
                b.Property(x => x.Message).HasMaxLength(2048);
                b.HasIndex(x => new { x.Timestamp, x.Level });
                b.HasOne(x => x.LogFile)
                    .WithMany(x => x.Entries)
                    .HasForeignKey(x => x.LogFileId);
            });

        }
        
    }
}