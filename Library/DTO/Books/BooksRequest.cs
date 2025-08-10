namespace Library.DTO.Books;

public class BooksRequest
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public int PublicationYear { get; set; }
    public Guid CategoryId { get; set; }
}