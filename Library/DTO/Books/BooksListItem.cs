namespace Library.DTO.Books;

public record BooksListItem
(
    Guid Id,
    string Title,
    string Author,
    string ISBN,
    int PublicationYear,
    string CategoryName,
    bool Available
);
