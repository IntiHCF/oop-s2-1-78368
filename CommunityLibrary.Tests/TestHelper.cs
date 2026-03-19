using Microsoft.EntityFrameworkCore;
using CommunityLibrary.Models;// your Book, Member, Loan models

namespace CommunityLibrary.Tests
{
    public static class TestHelper
    {
        public static ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // fresh DB each test
                .Options;

            var context = new ApplicationDbContext(options);
            DbInitializer.Initialize(context); // optional: seed books/members if needed
            return context;
        }
    }
}