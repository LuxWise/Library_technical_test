namespace Library.DTO.Loan;

public record LoanListItem(
    Guid Id,
    string BorrowerName,
    string BookTitle,
    DateTime LoanDate,
    DateTime? DueDate
);