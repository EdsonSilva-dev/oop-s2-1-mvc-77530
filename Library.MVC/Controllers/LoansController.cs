using Library.Domain.Models;
using Library.MVC.Data;
using Library.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.MVC.Controllers
{
    [Authorize]
    public class LoansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoansController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var loans = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .OrderByDescending(l => l.LoanDate)
                .ToListAsync();

            return View(loans);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new CreateLoanViewModel
            {
                AvailableBooks = await _context.Books
                    .Where(b => b.IsAvailable)
                    .OrderBy(b => b.Title)
                    .Select(b => new SelectListItem
                    {
                        Value = b.Id.ToString(),
                        Text = b.Title + " - " + b.Author
                    })
                    .ToListAsync(),

                Members = await _context.Members
                    .OrderBy(m => m.FullName)
                    .Select(m => new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.FullName
                    })
                    .ToListAsync()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateLoanViewModel vm)
        {
            if (vm.DueDate < vm.LoanDate)
            {
                ModelState.AddModelError("", "Due date cannot be earlier than loan date.");
            }

            bool activeLoanExists = await _context.Loans
                .AnyAsync(l => l.BookId == vm.BookId && l.ReturnedDate == null);

            if (activeLoanExists)
            {
                ModelState.AddModelError("", "This book is already on an active loan.");
            }

            if (!ModelState.IsValid)
            {
                vm.AvailableBooks = await _context.Books
                    .Where(b => b.IsAvailable)
                    .OrderBy(b => b.Title)
                    .Select(b => new SelectListItem
                    {
                        Value = b.Id.ToString(),
                        Text = b.Title + " - " + b.Author
                    })
                    .ToListAsync();

                vm.Members = await _context.Members
                    .OrderBy(m => m.FullName)
                    .Select(m => new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.FullName
                    })
                    .ToListAsync();

                return View(vm);
            }

            var loan = new Loan
            {
                BookId = vm.BookId,
                MemberId = vm.MemberId,
                LoanDate = vm.LoanDate,
                DueDate = vm.DueDate,
                ReturnedDate = null
            };

            _context.Loans.Add(loan);

            var book = await _context.Books.FindAsync(vm.BookId);
            if (book != null)
            {
                book.IsAvailable = false;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkReturned(int id)
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null) return NotFound();

            if (loan.ReturnedDate == null)
            {
                loan.ReturnedDate = DateTime.Today;

                if (loan.Book != null)
                {
                    loan.Book.IsAvailable = true;
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}