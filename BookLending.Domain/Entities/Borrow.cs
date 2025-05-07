using BookLending.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Domain.Entities
{
    public class Borrow : BaseEntity
    {
        public int BookId { get; set; }
        public string UserId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public BorrowStatus Status { get; set; } = BorrowStatus.Borrowed;
        public int RemindersSent { get; set; }
        public DateTime? LastReminderDate { get; set; }
        public Book Book { get; set; }
        public AppUser User { get; set; }
    }
}
