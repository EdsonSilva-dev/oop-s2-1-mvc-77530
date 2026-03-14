using Library.Domain;
using Library.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Library.MVC.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.MigrateAsync();

            await SeedRolesAndAdminAsync(userManager, roleManager);
            await SeedLibraryDataAsync(context);
        }

        private static async Task SeedRolesAndAdminAsync(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            string adminRole = "Admin";
            string adminEmail = "admin@library.com";
            string adminPassword = "Admin123!";

            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create admin user.");
                }
            }

            if (!await userManager.IsInRoleAsync(adminUser, adminRole))
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }

        private static async Task SeedLibraryDataAsync(ApplicationDbContext context)
        {
            if (!context.Books.Any())
            {
                var books = new List<Book>
                {
                    new() { Title = "Clean Code", Author = "Robert C. Martin", Isbn = "9780132350884", Category = "Programming", IsAvailable = true },
                    new() { Title = "The Pragmatic Programmer", Author = "Andrew Hunt", Isbn = "9780201616224", Category = "Programming", IsAvailable = true },
                    new() { Title = "Design Patterns", Author = "Erich Gamma", Isbn = "9780201633610", Category = "Programming", IsAvailable = true },
                    new() { Title = "Refactoring", Author = "Martin Fowler", Isbn = "9780201485677", Category = "Programming", IsAvailable = true },
                    new() { Title = "C# in Depth", Author = "Jon Skeet", Isbn = "9781617294532", Category = "Programming", IsAvailable = true },
                    new() { Title = "Introduction to Algorithms", Author = "Thomas H. Cormen", Isbn = "9780262033848", Category = "Computer Science", IsAvailable = true },
                    new() { Title = "Code Complete", Author = "Steve McConnell", Isbn = "9780735619678", Category = "Programming", IsAvailable = true },
                    new() { Title = "Head First Design Patterns", Author = "Eric Freeman", Isbn = "9780596007126", Category = "Programming", IsAvailable = true },
                    new() { Title = "Database System Concepts", Author = "Silberschatz", Isbn = "9780073523323", Category = "Database", IsAvailable = true },
                    new() { Title = "Operating System Concepts", Author = "Abraham Silberschatz", Isbn = "9781119456339", Category = "Computer Science", IsAvailable = true },
                    new() { Title = "Computer Networks", Author = "Andrew S. Tanenbaum", Isbn = "9780132126953", Category = "Networking", IsAvailable = true },
                    new() { Title = "Artificial Intelligence: A Modern Approach", Author = "Stuart Russell", Isbn = "9780134610993", Category = "AI", IsAvailable = true },
                    new() { Title = "Deep Learning", Author = "Ian Goodfellow", Isbn = "9780262035613", Category = "AI", IsAvailable = true },
                    new() { Title = "Python Crash Course", Author = "Eric Matthes", Isbn = "9781593279288", Category = "Programming", IsAvailable = true },
                    new() { Title = "Eloquent JavaScript", Author = "Marijn Haverbeke", Isbn = "9781593279509", Category = "Programming", IsAvailable = true },
                    new() { Title = "The Clean Coder", Author = "Robert C. Martin", Isbn = "9780137081073", Category = "Professional Development", IsAvailable = true },
                    new() { Title = "Domain-Driven Design", Author = "Eric Evans", Isbn = "9780321125217", Category = "Software Engineering", IsAvailable = true },
                    new() { Title = "Patterns of Enterprise Application Architecture", Author = "Martin Fowler", Isbn = "9780321127426", Category = "Software Engineering", IsAvailable = true },
                    new() { Title = "Working Effectively with Legacy Code", Author = "Michael Feathers", Isbn = "9780131177055", Category = "Software Engineering", IsAvailable = true },
                    new() { Title = "Cracking the Coding Interview", Author = "Gayle Laakmann McDowell", Isbn = "9780984782857", Category = "Interview Prep", IsAvailable = true }
                };

                context.Books.AddRange(books);
                await context.SaveChangesAsync();
            }

            if (!context.Members.Any())
            {
                var members = new List<Member>
                {
                    new() { FullName = "Alice Johnson", Email = "alice@example.com", Phone = "111111111" },
                    new() { FullName = "Bob Smith", Email = "bob@example.com", Phone = "222222222" },
                    new() { FullName = "Charlie Brown", Email = "charlie@example.com", Phone = "333333333" },
                    new() { FullName = "Diana Prince", Email = "diana@example.com", Phone = "444444444" },
                    new() { FullName = "Edward Green", Email = "edward@example.com", Phone = "555555555" },
                    new() { FullName = "Fiona White", Email = "fiona@example.com", Phone = "666666666" },
                    new() { FullName = "George Black", Email = "george@example.com", Phone = "777777777" },
                    new() { FullName = "Hannah Lee", Email = "hannah@example.com", Phone = "888888888" },
                    new() { FullName = "Ian Walker", Email = "ian@example.com", Phone = "999999999" },
                    new() { FullName = "Julia Adams", Email = "julia@example.com", Phone = "101010101" }
                };

                context.Members.AddRange(members);
                await context.SaveChangesAsync();
            }

            if (!context.Loans.Any())
            {
                var books = context.Books.OrderBy(b => b.Id).ToList();
                var members = context.Members.OrderBy(m => m.Id).ToList();

                var loans = new List<Loan>
                {
                    new() { BookId = books[0].Id, MemberId = members[0].Id, LoanDate = DateTime.Today.AddDays(-10), DueDate = DateTime.Today.AddDays(4), ReturnedDate = null },
                    new() { BookId = books[1].Id, MemberId = members[1].Id, LoanDate = DateTime.Today.AddDays(-20), DueDate = DateTime.Today.AddDays(-5), ReturnedDate = null },
                    new() { BookId = books[2].Id, MemberId = members[2].Id, LoanDate = DateTime.Today.AddDays(-18), DueDate = DateTime.Today.AddDays(-2), ReturnedDate = DateTime.Today.AddDays(-1) },
                    new() { BookId = books[3].Id, MemberId = members[3].Id, LoanDate = DateTime.Today.AddDays(-7), DueDate = DateTime.Today.AddDays(7), ReturnedDate = null },
                    new() { BookId = books[4].Id, MemberId = members[4].Id, LoanDate = DateTime.Today.AddDays(-15), DueDate = DateTime.Today.AddDays(-1), ReturnedDate = null },
                    new() { BookId = books[5].Id, MemberId = members[5].Id, LoanDate = DateTime.Today.AddDays(-12), DueDate = DateTime.Today.AddDays(2), ReturnedDate = null },
                    new() { BookId = books[6].Id, MemberId = members[6].Id, LoanDate = DateTime.Today.AddDays(-21), DueDate = DateTime.Today.AddDays(-6), ReturnedDate = DateTime.Today.AddDays(-3) },
                    new() { BookId = books[7].Id, MemberId = members[7].Id, LoanDate = DateTime.Today.AddDays(-5), DueDate = DateTime.Today.AddDays(9), ReturnedDate = null },
                    new() { BookId = books[8].Id, MemberId = members[8].Id, LoanDate = DateTime.Today.AddDays(-30), DueDate = DateTime.Today.AddDays(-15), ReturnedDate = DateTime.Today.AddDays(-10) },
                    new() { BookId = books[9].Id, MemberId = members[9].Id, LoanDate = DateTime.Today.AddDays(-3), DueDate = DateTime.Today.AddDays(11), ReturnedDate = null },
                    new() { BookId = books[10].Id, MemberId = members[0].Id, LoanDate = DateTime.Today.AddDays(-9), DueDate = DateTime.Today.AddDays(5), ReturnedDate = null },
                    new() { BookId = books[11].Id, MemberId = members[1].Id, LoanDate = DateTime.Today.AddDays(-25), DueDate = DateTime.Today.AddDays(-10), ReturnedDate = null },
                    new() { BookId = books[12].Id, MemberId = members[2].Id, LoanDate = DateTime.Today.AddDays(-14), DueDate = DateTime.Today, ReturnedDate = null },
                    new() { BookId = books[13].Id, MemberId = members[3].Id, LoanDate = DateTime.Today.AddDays(-11), DueDate = DateTime.Today.AddDays(3), ReturnedDate = DateTime.Today.AddDays(-2) },
                    new() { BookId = books[14].Id, MemberId = members[4].Id, LoanDate = DateTime.Today.AddDays(-8), DueDate = DateTime.Today.AddDays(6), ReturnedDate = null }
                };

                context.Loans.AddRange(loans);

                foreach (var loan in loans.Where(l => l.ReturnedDate == null))
                {
                    var book = books.First(b => b.Id == loan.BookId);
                    book.IsAvailable = false;
                }

                await context.SaveChangesAsync();
            }
        }
    }
}