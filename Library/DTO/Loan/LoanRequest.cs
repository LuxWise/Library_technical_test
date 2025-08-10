namespace Library.DTO.Loan;

public class LoanRequest
{
    public Guid UserId { get; set; }
    public Guid BookId { get; set; }
    public DateTime DueDate { get; set; }
}