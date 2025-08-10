namespace Library.DTO.Loan;

public class LoanResponse
{
    public string message { get; set; }
    public LoanDetails? Loan { get; set; }
}