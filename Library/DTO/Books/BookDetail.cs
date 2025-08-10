namespace Library.DTO.Books;

public record BookDetail
(
    Guid Id,
    string Title,
    string Author,
    string ISBN,
    int PublicationYear,
    Guid CategoryId,
    string CategoryName,
    bool Available,
    DateTime CreatedAt,
    DateTime UpdatedA
);