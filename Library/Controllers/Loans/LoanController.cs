using Library.DTO.Loan;
using Library.Services.Loan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers.Loans
{
    [ApiController]
    [Route("api/loans")]
    
    public class LoanController : ControllerBase
    {
        private readonly ILoanServices _loan;
        
        public LoanController(ILoanServices loan)
        {
            _loan = loan;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<LoanListItem>>> GetLoans()
        { 
            var resp = await _loan.GetLoans();
            return Ok(resp);
        }
        
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<LoanDetails?>> GetLoanById(Guid id)
        {
            var resp = await _loan.GetLoanById(id);
            if (resp is null) return NotFound(new { message = "Loan not found" });
            return Ok(resp);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<LoanResponse>> CreateLoan([FromBody] LoanRequest loan)
        {
            var resp = await _loan.AddLoan(loan);
            return Ok(resp);
        }

        [HttpPost("{id}/return")]
        [Authorize]
        public async Task<ActionResult<LoanResponse>> ReturnLoan(Guid id)
        {
            var resp = await _loan.ReturnLoan(id);
            return Ok(resp);        
        }

        [HttpPost("{id}/renew")]
        [Authorize]
        public async Task<ActionResult<LoanResponse>> RenewLoan(Guid id, [FromQuery] int extraDays)
        {
            var resp = await _loan.RenewLoan(id, extraDays);
            return Ok(resp);
        }

        
    }
}