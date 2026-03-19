using CommunityLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
public static class DbInitializer
{
    public static void Initialize(
     ApplicationDbContext context,
     UserManager<IdentityUser> userManager,
     RoleManager<IdentityRole> roleManager)
    {

        // If there are already books, stop (avoid duplicates)
        if (context.Books.Any())
            return;


        // ------------------ BOOKS ------------------
        var books = new Book[]
        {
                new Book { Title = "C# Basics", Author = "John Smith", Category = "Programming", Isbn = "111", IsAvailable = true },
                new Book { Title = "ASP.NET Core Guide", Author = "Jane Doe", Category = "Programming", Isbn = "112", IsAvailable = true },
                new Book { Title = "Harry Potter", Author = "J.K Rowling", Category = "Fantasy", Isbn = "113", IsAvailable = true },
                new Book { Title = "The Hobbit", Author = "Tolkien", Category = "Fantasy", Isbn = "114", IsAvailable = true },
                new Book { Title = "Clean Code", Author = "Robert Martin", Category = "Programming", Isbn = "115", IsAvailable = true },
                new Book { Title = "1984", Author = "George Orwell", Category = "Dystopian", Isbn = "116", IsAvailable = true },
                new Book { Title = "The Alchemist", Author = "Paulo Coelho", Category = "Fiction", Isbn = "117", IsAvailable = true },
                new Book { Title = "Atomic Habits", Author = "James Clear", Category = "Self-help", Isbn = "118", IsAvailable = true },
                new Book { Title = "The Pragmatic Programmer", Author = "Andrew Hunt", Category = "Programming", Isbn = "119", IsAvailable = true },
                new Book { Title = "Game of Thrones", Author = "George R.R Martin", Category = "Fantasy", Isbn = "120", IsAvailable = true }
        };
        context.Books.AddRange(books);
        context.SaveChanges();

        // ------------------ MEMBERS ------------------
        var members = new Member[]
        {
                new Member { FullName = "Alice Johnson", Email = "alice@test.com", Phone = "123456" },
                new Member { FullName = "Bob Smith", Email = "bob@test.com", Phone = "654321" },
                new Member { FullName = "Charlie Brown", Email = "charlie@test.com", Phone = "789123" },
                new Member { FullName = "David Wilson", Email = "david@test.com", Phone = "321987" },
                new Member { FullName = "Emma Davis", Email = "emma@test.com", Phone = "456789" }
        };
        context.Members.AddRange(members);
        context.SaveChanges();

        // ------------------ LOANS ------------------
        // Retrieve saved entities to get their generated IDs
        var savedBooks = context.Books.ToList();
        var savedMembers = context.Members.ToList();

        var loans = new Loan[]
        {
                new Loan
                {
                    BookId = savedBooks[0].Id,
                    MemberId = savedMembers[0].Id,
                    LoanDate = DateTime.Now.AddDays(-5),
                    DueDate = DateTime.Now.AddDays(5),
                    ReturnedDate = null
                },
                new Loan
                {
                    BookId = savedBooks[1].Id,
                    MemberId = savedMembers[1].Id,
                    LoanDate = DateTime.Now.AddDays(-10),
                    DueDate = DateTime.Now.AddDays(-2),
                    ReturnedDate = null
                }
        };

        var admin = new Member
        {
            FullName = "Admin User",
            Email = "admin@test.com",
            Phone = "000000",
            IsAdmin = true
        };
        context.Members.Add(admin);
        context.SaveChanges();

        // Mark loaned books as unavailable
        savedBooks[0].IsAvailable = false;
        savedBooks[1].IsAvailable = false;

        context.Loans.AddRange(loans);
        context.SaveChanges();

        // Seed Admin role
        if (!roleManager.Roles.Any(r => r.Name == "Admin"))
        {
            roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
        }

        // Seed Admin user
        if (!userManager.Users.Any(u => u.UserName == "admin@test.com"))
        {
            var adminUser = new IdentityUser
            {
                UserName = "admin@test.com",
                Email = "admin@test.com",
                EmailConfirmed = true
            };
            userManager.CreateAsync(adminUser, "Password123!").Wait();
            userManager.AddToRoleAsync(adminUser, "Admin").Wait();
        }
    }
}