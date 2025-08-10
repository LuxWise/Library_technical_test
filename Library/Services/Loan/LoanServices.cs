using Library.Data;
using Library.DTO.Loan;
using Library.Services.Auth.Users;
using Microsoft.EntityFrameworkCore;

namespace Library.Services.Loan
{
    public class LoanServices : ILoanServices
    {
        private readonly LibraryDbContext _db;
        private readonly ICurrentUserService _currentUserService;
        public LoanServices(LibraryDbContext db, ICurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }
        
        public async Task<List<LoanListItem>> GetLoans(CancellationToken ct = default)
        {
            return await _db.Loan
                .AsNoTracking()
                .OrderBy(l => l.LoanDate)
                .Select(l => new LoanListItem(
                    l.Id,
                    l.User.Name,
                    l.Book.Title,
                    l.LoanDate,
                    l.DueDate
                ))
                .ToListAsync(ct);
        }

        public async Task<LoanDetails?> GetLoanById(Guid id, CancellationToken ct = default)
        {
            return await _db.Loan
                .AsNoTracking()
                .Where(l => l.Id == id)
                .Select(l => new LoanDetails(
                    l.Id,
                    l.User.Name,
                    l.Book.Title,
                    l.LoanDate,
                    l.DueDate,
                    l.ReturnDate,
                    l.Renewals
                ))
                .FirstOrDefaultAsync(ct);
        }
        
        public async Task<LoanResponse> AddLoan(LoanRequest request, CancellationToken ct = default)
        {
            var book = await _db.Book
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == request.BookId, ct);
            if (book == null || !book.Available)
                throw new InvalidOperationException("Book is not available for loan.");
            
            var userId = _currentUserService.GetCurrentUserId();
            var user = await _db.User
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId, ct);
            if (user == null)
                throw new InvalidOperationException("User does not exist.");

            var loan = new Model.Loan()
            {
                UserId = user.Id,
                BookId = book.Id,
                DueDate = DateTime.UtcNow.AddDays(14),
                Status = Model.LoanStatus.Active
            };

            book.Available = false;

            _db.Loan.Add(loan);
            _db.Book.Update(book);
            await _db.SaveChangesAsync(ct);

            return new LoanResponse
            {
                message = "Loan added successfully",
                Loan = new LoanDetails(
                    loan.Id,
                    user.Name,
                    book.Title,
                    loan.LoanDate,
                    loan.DueDate,
                    null, 
                    loan.Renewals
                )
                
            };
        }

        public async Task<LoanResponse> ReturnLoan(Guid id, CancellationToken ct = default)
        {
            var loan = await _db.Loan
                .FirstOrDefaultAsync(l => l.Id == id, ct);

            if (loan == null || loan.Status != Model.LoanStatus.Active)
                throw new InvalidOperationException("Loan not found or already returned.");

            if (loan.BookId == Guid.Empty)
                throw new InvalidOperationException("Loan has no associated book.");

            var book = await _db.Book
                .FirstOrDefaultAsync(b => b.Id == loan.BookId, ct);

            if (book == null)
                throw new InvalidOperationException("Book not found.");

            loan.ReturnDate = DateTime.UtcNow;
            loan.Status = Model.LoanStatus.Returned;

            if (!book.Available)
                book.Available = true;

            _db.Loan.Update(loan);
            _db.Book.Update(book);
            await _db.SaveChangesAsync(ct);

            return new LoanResponse
            {
                message = "Loan returned successfully",
                Loan = new LoanDetails(
                    loan.Id,
                    loan.User.Name,
                    book.Title,
                    loan.LoanDate,
                    loan.DueDate,
                    loan.ReturnDate,
                    loan.Renewals
                )
            };
        }

        public async Task<LoanResponse> RenewLoan(Guid id, int extraDays ,CancellationToken ct = default)
        {
            var loan = await _db.Loan
                .FirstOrDefaultAsync(l => l.Id == id, ct);
            if (loan == null || loan.Status != Model.LoanStatus.Active)
                throw new InvalidOperationException("Loan not found or already returned.");
            
            if (loan.Renewals >= 3)
                throw new InvalidOperationException("Loan cannot be renewed more than 3 times.");
            
            if (extraDays <= 0)
                throw new ArgumentException("Extra days must be greater than zero.");
            
            if (extraDays > 30)
                throw new ArgumentException("Extra days cannot exceed 30 days.");
            
            loan.DueDate = loan.DueDate.AddDays(extraDays); 
            
            _db.Loan.Update(loan);
            await _db.SaveChangesAsync(ct);
            
            return new LoanResponse
            {
                message = "Loan time extends successfully",
                Loan = new LoanDetails(
                    loan.Id,
                    loan.User.Name,
                    loan.Book.Title,
                    loan.LoanDate,
                    loan.DueDate,
                    loan.ReturnDate,
                    loan.Renewals += 1
                )
            };
        }
    }    
}
