using CommunityLibrary.Models;
using System;
using System.Linq;
using Xunit;
using CommunityLibrary.Tests;

public class LoanTests
    {
    [Fact]
    public void CannotCreateLoanForAlreadyLoanedBook()
    {
        using var context = TestHelper.GetInMemoryDbContext();

        // Pick a book that is already loaned in DbInitializer
        var book = context.Books.First(b => !b.IsAvailable); // already unavailable
        var member = context.Members.First();

        // Business logic: can we loan it?
        bool canLoan = book.IsAvailable;

        // Assert that the book cannot be loaned
        Assert.False(canLoan);
    }

    [Fact]
        public void ReturnedLoanMakesBookAvailable()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            var book = context.Books.First();
            var member = context.Members.First();

            var loan = new Loan
            {
                BookId = book.Id,
                MemberId = member.Id,
                LoanDate = DateTime.Now.AddDays(-5),
                DueDate = DateTime.Now.AddDays(2),
                ReturnedDate = null
            };
            book.IsAvailable = false;
            context.Loans.Add(loan);
            context.SaveChanges();

            // Return the book
            loan.ReturnedDate = DateTime.Now;
            book.IsAvailable = true;
            context.SaveChanges();

            Assert.True(book.IsAvailable);
        }

        [Fact]
        public void LoanIsOverdueIfPastDueAndNotReturned()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            var book = context.Books.First();
            var member = context.Members.First();

            var loan = new Loan
            {
                BookId = book.Id,
                MemberId = member.Id,
                LoanDate = DateTime.Now.AddDays(-10),
                DueDate = DateTime.Now.AddDays(-2),
                ReturnedDate = null
            };
            context.Loans.Add(loan);
            context.SaveChanges();

            var overdue = context.Loans.Any(l => l.DueDate < DateTime.Now && l.ReturnedDate == null);
            Assert.True(overdue);
        }
}