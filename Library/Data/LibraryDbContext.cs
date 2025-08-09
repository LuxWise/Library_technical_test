using Microsoft.EntityFrameworkCore;

namespace Library.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Book { get; set; }
        public DbSet<Category> Category { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>(e =>
            {   e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasDefaultValueSql("UUID()");
                e.Property(x => x.Title).IsRequired().HasMaxLength(100);
                e.Property(x => x.Author).IsRequired().HasMaxLength(100);
                e.Property(x => x.ISBN).IsRequired().HasMaxLength(13);
                e.Property(x => x.ISBN).IsUnicode();
                e.Property(x => x.PublicationYear).IsRequired();
                e.Property(x => x.Available).IsRequired();
                e.HasOne(x => x.Category)
                    .WithMany(x => x.Books)
                    .HasForeignKey(x => x.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Category>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasDefaultValueSql("UUID()");
                e.Property(x => x.Name).IsRequired().HasMaxLength(100);
                e.HasIndex(x => x.Name).IsUnique(); 
            });
        }
        
    }
}