using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CommunityLibrary.Models;

namespace CommunityLibrary.Controllers
{
    public class LoansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Loans
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Loans.Include(l => l.Book).Include(l => l.Member);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Loans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // GET: Loans/CreateForBook?bookId=1
        public IActionResult CreateForBook(int bookId)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null)
                return NotFound();

            if (!book.IsAvailable)
            {
                TempData["Error"] = "This book is already on loan.";
                return RedirectToAction("Index", "Books");
            }

            // Pass the book and members to the view
            ViewData["Book"] = book;
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "FullName");
            return View();
        }
        // GET: Loans/Create
        public IActionResult Create()
        {
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Id");
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateForBook(int bookId, int memberId)
        {
            var book = await _context.Books.FindAsync(bookId);
            var member = await _context.Members.FindAsync(memberId);

            if (book == null || member == null || !book.IsAvailable)
            {
                TempData["Error"] = "Cannot create loan.";
                return RedirectToAction("Index", "Books");
            }

            var loan = new Loan
            {
                BookId = book.Id,
                MemberId = member.Id,
                LoanDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                ReturnedDate = null
            };

            book.IsAvailable = false;
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Loan created for {book.Title}.";
            return RedirectToAction("Index", "Books");
        }

        // POST: Loans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookId,MemberId,LoanDate,DueDate,ReturnedDate")] Loan loan)
        {
            //Check if book is already on loan
            bool isLoaned = _context.Loans
                .Any(l => l.BookId == loan.BookId && l.ReturnedDate == null);

            if (isLoaned)
            {
                ModelState.AddModelError("", "This book is already on loan!");
            }

            if (ModelState.IsValid)
            {
                // Mark book as unavailable
                var book = _context.Books.Find(loan.BookId);
                if (book != null)
                {
                    book.IsAvailable = false;
                }

                _context.Add(loan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Id", loan.BookId);
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "Id", loan.MemberId);
            return View(loan);
        }

        // GET: Loans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans.FindAsync(id);
            if (loan == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Id", loan.BookId);
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "Id", loan.MemberId);
            return View(loan);
        }

        // POST: Loans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,MemberId,LoanDate,DueDate,ReturnedDate")] Loan loan)
        {
            if (id != loan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingLoan = await _context.Loans.AsNoTracking().FirstOrDefaultAsync(l => l.Id == loan.Id);

                    if (existingLoan != null)
                    {
                        // If book is being returned now
                        if (existingLoan.ReturnedDate == null && loan.ReturnedDate != null)
                        {
                            var book = _context.Books.Find(loan.BookId);
                            if (book != null)
                            {
                                book.IsAvailable = true;
                            }
                        }
                    }

                    _context.Update(loan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoanExists(loan.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Id", loan.BookId);
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "Id", loan.MemberId);
            return View(loan);
        }

        // GET: Loans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // POST: Loans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan != null)
            {
                _context.Loans.Remove(loan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoanExists(int id)
        {
            return _context.Loans.Any(e => e.Id == id);
        }
    }
}
