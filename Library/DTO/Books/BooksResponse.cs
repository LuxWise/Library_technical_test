using Library.DTO.Books;

public class BooksResponse
{
    public string message { get; set; } = string.Empty;
    public BookDetail? Book { get; set; }
}