using Library.Domain;
using Library.Domain.Models;
using Xunit;

namespace Library.Tests
{
    public class LoanRulesTests
    {
        [Fact]
        public void Loan_Is_Overdue_When_DueDate_Has_Passed_And_Not_Returned()
        {
            var loan = new Loan
            {
                LoanDate = DateTime.Today.AddDays(-10),
                DueDate = DateTime.Today.AddDays(-1),
                ReturnedDate = null
            };

            Assert.True(loan.IsOverdue);
        }

        [Fact]
        public void Loan_Is_Not_Overdue_When_Returned()
        {
            var loan = new Loan
            {
                LoanDate = DateTime.Today.AddDays(-10),
                DueDate = DateTime.Today.AddDays(-1),
                ReturnedDate = DateTime.Today
            };

            Assert.False(loan.IsOverdue);
        }

        [Fact]
        public void Loan_With_Future_DueDate_Should_Not_Be_Overdue()
        {
            var loan = new Loan
            {
                LoanDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(7),
                ReturnedDate = null
            };

            Assert.False(loan.IsOverdue);
        }

        [Fact]
        public void Returned_Loan_Should_Keep_ReturnedDate()
        {
            var returnedDate = DateTime.Today;

            var loan = new Loan
            {
                LoanDate = DateTime.Today.AddDays(-7),
                DueDate = DateTime.Today.AddDays(-1),
                ReturnedDate = returnedDate
            };

            Assert.Equal(returnedDate, loan.ReturnedDate);
        }

        [Fact]
        public void New_Book_Should_Be_Available_By_Default()
        {
            var book = new Book
            {
                Title = "Clean Code",
                Author = "Robert C. Martin",
                Isbn = "123456",
                Category = "Programming"
            };

            Assert.True(book.IsAvailable);
        }
    }
}