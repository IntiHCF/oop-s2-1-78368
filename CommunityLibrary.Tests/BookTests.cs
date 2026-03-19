using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityLibrary.Tests
{
    public class BookTests
    {
        [Fact]
        public void BookSearchReturnsMatches()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            var searchTerm = "C#";

            var results = context.Books
                .Where(b => b.Title.Contains(searchTerm))
                .ToList();

            Assert.All(results, b => Assert.Contains(searchTerm, b.Title));
        }
    }
}
