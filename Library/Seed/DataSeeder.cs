using Library.Data;
using Library.Model;

namespace Library.Seed;

public class DataSeeder
{
    public static void SeedBooks(LibraryDbContext db)
    {
        if (db.Book.Any())
            return;

        var random = new Random();
        var categories = db.Category.Select(c => c.Id).ToList();

        if (!categories.Any())
            throw new Exception("Unknown categories. Please seed categories first.");

        var books = new List<Book>();

        for (int i = 1; i <= 120; i++)
        {
            var categoryId = categories[random.Next(categories.Count)];
            books.Add(new Book
            {
                Title = $"Book test {i}",
                Author = $"Author {random.Next(1, 50)}",
                ISBN = $"{random.Next(100, 999)}-{random.Next(1000, 9999)}-{random.Next(1000, 9999)}",
                PublicationYear = random.Next(1980, 2025),
                Available = true,
                CategoryId = categoryId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        db.Book.AddRange(books);
        db.SaveChanges();
    }
    
    public static void SeedCategories(LibraryDbContext db)
    {
        if (db.Category.Any())
            return;

        var categories = new List<Category>
        {
            new Category { Name = "Fiction" },
            new Category { Name = "Non-Fiction" },
            new Category { Name = "Science" },
            new Category { Name = "History" },
            new Category { Name = "Technology" },
            new Category { Name = "Children's Literature" },
            new Category { Name = "Fantasy" },
            new Category { Name = "Mystery" },
            new Category { Name = "Romance" },
            new Category { Name = "Horror" },
            new Category { Name = "Thriller" },
            new Category { Name = "Biography" },
            new Category { Name = "Autobiography" },
            new Category { Name = "Poetry" },
            new Category { Name = "Drama" },
            new Category { Name = "Adventure" },
            new Category { Name = "Science Fiction" },
            new Category { Name = "Self-Help" },
            new Category { Name = "Spirituality" },
            new Category { Name = "Essay" }
        };
        
        db.Category.AddRange(categories);
        db.SaveChanges();
    }
}