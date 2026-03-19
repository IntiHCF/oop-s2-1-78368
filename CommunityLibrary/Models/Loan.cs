namespace CommunityLibrary.Models
{
    public class Loan
    {
        public int Id { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; } = new Book();

        public int MemberId { get; set; }
        public Member Member { get; set; } = new Member();

        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }

        public DateTime? ReturnedDate { get; set; } 

    }

}
