namespace CommunityLibrary.Models
{
    public class Member
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public List<Loan> Loans { get; set; } = new List<Loan>();

        public bool IsAdmin { get; set; } = false;
    }
}
