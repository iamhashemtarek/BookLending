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
            : base(b => (b.DueDate < DateTime.UtcNow && b.Status == BorrowStatus.Borrowed && b.IsDeleted == false) || ( b.Status == BorrowStatus.Overdue && b.IsDeleted == false))
        {
            AddInclude(x => x.Book);
            AddInclude(x => x.User);
        }
    }
}
