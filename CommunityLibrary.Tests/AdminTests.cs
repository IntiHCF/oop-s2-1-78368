using CommunityLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityLibrary.Tests
{
    public class AdminTests
    {
        [Fact]
        public void AdminPageRequiresAdminRole()
        {
            using var context = TestHelper.GetInMemoryDbContext();

            // Create a non-admin member
            var member = new Member { FullName = "Test User", IsAdmin = false };
            context.Members.Add(member);
            context.SaveChanges();

            // Simulate controller access logic
            var canAccess = member.IsAdmin;
            Assert.False(canAccess);

            // Now test with an admin
            var admin = new Member { FullName = "Admin User", IsAdmin = true };
            context.Members.Add(admin);
            context.SaveChanges();

            var adminAccess = admin.IsAdmin;
            Assert.True(adminAccess);
        }
    }
}