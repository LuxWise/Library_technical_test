namespace Library.Model;

public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public List<Book> Books { get; set; } = new List<Book>();
}