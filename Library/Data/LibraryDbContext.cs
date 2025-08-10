using Library.Model;
using Microsoft.EntityFrameworkCore;

namespace Library.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Book> Book { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Loan> Loan { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasDefaultValueSql("(UUID())");
                e.Property(x => x.Name).IsRequired().HasMaxLength(100);
                e.Property(x => x.Email).IsRequired().HasMaxLength(200);
                e.Property(x => x.Password).IsRequired().HasMaxLength(100);
            });
            
            modelBuilder.Entity<Book>(e =>
            {   
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasDefaultValueSql("(UUID())");
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
                e.Property(x => x.Id).HasDefaultValueSql("(UUID())");
                e.Property(x => x.Name).IsRequired().HasMaxLength(100);
                e.HasIndex(x => x.Name).IsUnique(); 
            });
            
            modelBuilder.Entity<Loan>(e =>
            {
                e.ToTable("loans");

                e.HasKey(x => x.Id);
                e.Property(x => x.Id)
                    .HasColumnType("char(36)")
                    .HasDefaultValueSql("(UUID())");

                e.Property(x => x.LoanDate).IsRequired();
                e.Property(x => x.DueDate).IsRequired();

                e.Property(x => x.Status)
                    .HasConversion<int>()
                    .IsRequired();

                e.Property(x => x.Renewals).HasDefaultValue(0);

                e.HasOne(x => x.User)
                    .WithMany()          
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Book)
                    .WithMany(b => b.Loans)
                    .HasForeignKey(x => x.BookId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.BookId, x.Status });     
                e.HasIndex(x => new { x.UserId, x.Status });     
                e.HasIndex(x => x.DueDate);

                e.HasIndex(x => new { x.BookId, x.Status })
                    .IsUnique()
                    .HasFilter("Status = 1");
            });

        }
        
    }
}