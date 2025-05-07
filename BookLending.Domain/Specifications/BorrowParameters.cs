using BookLending.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Domain.Specifications
{
    public class BorrowParameters
    {
        private const int MAX_PAGE_SIZE = 10;
        public int? BookId { get; set; }
        public string UserId { get; set; }
        public DateTime? BorrowDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string? Status { get; set; }
        public int? RemindersSent { get; set; } 
        public string? SortBy { get; set; }
        public bool IsSortAscending { get; set; } = true;
        public int PageNumber { get; set; } = 1;
        private int pageSize { get; set; } = 10;
        public int PageSize
        {
            get => pageSize;
            set => pageSize = (value > MAX_PAGE_SIZE) ? MAX_PAGE_SIZE : value;
        }
    }
}
