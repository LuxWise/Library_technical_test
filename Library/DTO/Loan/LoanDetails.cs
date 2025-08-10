namespace Library.DTO.Loan;

public record LoanDetails(
    Guid Id,
    string BorrowerName,
    string BookTitle,
    DateTime LoanDate,
    DateTime? DueDate,
    DateTime? ReturnDate,
    int Renewals
);