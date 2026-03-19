using Microsoft.EntityFrameworkCore;

namespace CommunityLibrary.Models
{
    [Index(nameof(Title), IsUnique = true)]
    public class Book
    {
        public int Id { get; set; } 

        public string Title { get; set; } = string.Empty; 
        public string Author { get; set; } = string.Empty; 
        public string Isbn { get; set; } = string.Empty; 
        public string Category { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true; 

        public List<Loan> Loans { get; set; } = new List<Loan>();
    }
}
