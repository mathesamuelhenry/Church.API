using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Church.API.Data.DBContext;
using Church.API.Models;
using Church.API.Data;

namespace Church.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IronChurchContext _context;

        public AccountsController(IronChurchContext context)
        {
            _context = context;
        }

        // GET: api/Accounts
        [HttpGet]
        public async Task<List<Account>> Get()
        {
            return await _context.Account.ToListAsync();
        }

        // GET: api/Accounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            var account = await _context.Account.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

        // PUT: api/Accounts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(int id, Account account)
        {
            if (id != account.AccountId)
            {
                return BadRequest();
            }

            _context.Entry(account).Property(x => x.AccountNumber).IsModified = true;
            _context.Entry(account).Property(x => x.AccountName).IsModified = true;
            _context.Entry(account).Property(x => x.BankName).IsModified = true;
            _context.Entry(account).Property(x => x.InitialBalance).IsModified = true;
            _context.Entry(account).Property(x => x.DateChanged).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Accounts
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount(Account account)
        {
            account.AccountId = Utils.GetNextIdAsync(_context, "account").Result;
            account.Status = 1;

            _context.Account.Add(account);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAccount", new { id = account.AccountId }, account);
        }

        // DELETE: api/Accounts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Account>> DeleteAccount(int id)
        {
            var account = await _context.Account.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            _context.Account.Remove(account);
            await _context.SaveChangesAsync();

            return account;
        }

        // PUT: api/EndAccount/5
        [HttpPut]
        [Route("EndAccount/{id}")]
        public async Task<IActionResult> EndAccount(int id, Account account)
        {
            if (id != account.AccountId)
            {
                return BadRequest();
            }
            
            if (account.AccountEndDate == null)
                account.AccountEndDate = DateTime.UtcNow;

            if (account.DateChanged == null)
                account.DateChanged = DateTime.UtcNow;
            

            _context.Entry(account).Property(x => x.AccountEndDate).IsModified = true;
            _context.Entry(account).Property(x => x.DateChanged).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool AccountExists(int id)
        {
            return _context.Account.Any(e => e.AccountId == id);
        }
    }
}
