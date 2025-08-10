using Library.Model;

public class Book
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public int PublicationYear { get; set; }
    public bool Available { get; set; } = true;
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}