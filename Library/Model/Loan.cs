namespace Library.Model;

public enum LoanStatus { Active = 1, Returned = 2, Overdue = 3, Canceled = 4 }

public class Loan
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
    public Guid BookId { get; set; }
    public Book Book { get; set; } = default!;
    public DateTime LoanDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public int Renewals { get; set; } = 0;
    public LoanStatus Status { get; set; } = LoanStatus.Active;

}