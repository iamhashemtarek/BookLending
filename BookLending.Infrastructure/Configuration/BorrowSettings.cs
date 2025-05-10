using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Infrastructure.Configuration
{
    public class BorrowSettings
    {
        public int MaxBorrowedBooks { get; set; }
        public int MaxBorrowDurationDays { get; set; }
    }
}
