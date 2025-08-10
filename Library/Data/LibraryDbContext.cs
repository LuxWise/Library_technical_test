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
                e.ToTable("user");
                
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnType("char(36)").ValueGeneratedNever();
                e.Property(x => x.Name).IsRequired().HasMaxLength(100);
                e.Property(x => x.Email).IsRequired().HasMaxLength(200);
                e.Property(x => x.Password).IsRequired().HasMaxLength(100);
                
                e.Property(x => x.CreatedAt)
                    .HasColumnType("datetime(6)")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                    .ValueGeneratedOnAdd();

                e.Property(x => x.UpdatedAt)
                    .HasColumnType("datetime(6)")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)")
                    .ValueGeneratedOnAddOrUpdate();
                
                e.HasIndex(x => x.Email).IsUnique();
            });
            
            modelBuilder.Entity<Book>(e =>
            {   
                e.ToTable("book");
                
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnType("char(36)").ValueGeneratedNever();
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
                
                e.Property(x => x.CreatedAt)
                    .HasColumnType("datetime(6)")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")   // default al insertar
                    .ValueGeneratedOnAdd();

                e.Property(x => x.UpdatedAt)
                    .HasColumnType("datetime(6)")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)") // update auto
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<Category>(e =>
            {
                e.ToTable("category");
                
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnType("char(36)").ValueGeneratedNever();
                e.Property(x => x.Name).IsRequired().HasMaxLength(100);
                e.HasIndex(x => x.Name).IsUnique(); 
            });
            
            modelBuilder.Entity<Loan>(e =>
            {
                e.ToTable("loan");

                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnType("char(36)").ValueGeneratedNever();
                e.Property(x => x.LoanDate).IsRequired();
                e.Property(x => x.DueDate).IsRequired();
                e.Property(x => x.Status).HasConversion<int>().IsRequired();
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
                
                e.Property(x => x.DueDate)
                    .HasColumnType("datetime(6)")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")   // default al insertar
                    .ValueGeneratedOnAdd();

                e.Property(x => x.ReturnDate)
                    .HasColumnType("datetime(6)")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)") // update auto
                    .ValueGeneratedOnAddOrUpdate();
            });

        }
        
    }
}