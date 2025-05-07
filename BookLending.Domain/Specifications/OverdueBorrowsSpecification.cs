using BookLending.Domain.Entities;
using BookLending.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Domain.Specifications
{
    public class OverdueBorrowsSpecification : Specification<Borrow>
    {
        public OverdueBorrowsSpecification()
            : base(b => b.DueDate < DateTime.UtcNow && b.Status == BorrowStatus.Borrowed)
        {
        }
    }
}
