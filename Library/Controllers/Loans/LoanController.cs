using Library.Data;
using Library.DTO.Loan;
using Library.Model;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers.Loans
{
    [ApiController]
    [Route("api/loans")]
    
    public class LoanController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        
        public LoanController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public List<Loan> GetLoans()
        {
            return _context.Loan.ToList();
        }
        
        [HttpGet("{id}")]
        public Loan GetLoan(Guid id)
        {
            if (id == Guid.Empty)
            { 
                throw new ArgumentException("Invalid book ID");
            }
            
            return _context.Loan.Find(id) ?? throw new KeyNotFoundException("Loan not found");
        }

        [HttpPost]
        public LoanResponse CreateLoan([FromBody] LoanResponse loan)
        {
            return new LoanResponse{message = "Loan created successfully"};
        }

        [HttpPost("{id}/return")]
        public LoanResponse ReturnLoan(Guid id)
        {
            return new LoanResponse{message = "Loan returned successfully"};
        }

        [HttpPost("{id}/renew")]
        public LoanResponse RenewLoan(Guid id)
        {
            return new LoanResponse{message = "Loan renewed successfully"};
        }
        
    }
}