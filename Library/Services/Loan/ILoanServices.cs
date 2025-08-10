using Library.DTO.Loan;

namespace Library.Services.Loan
{
    public interface ILoanServices
    {
        Task<List<LoanListItem>> GetLoans(CancellationToken ct = default);
        Task<LoanDetails?> GetLoanById(Guid id, CancellationToken ct = default);
        Task<LoanResponse> AddLoan(LoanRequest request, CancellationToken ct = default);
        Task<LoanResponse> ReturnLoan(Guid id, CancellationToken ct = default);
        Task<LoanResponse> RenewLoan(Guid id, int extraDays ,CancellationToken ct = default);
    }    
}
