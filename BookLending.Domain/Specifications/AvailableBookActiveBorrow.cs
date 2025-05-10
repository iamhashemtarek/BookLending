using BookLending.Domain.Entities;
using BookLending.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Domain.Specifications
{
    public class AvailableBookActiveBorrow : Specification<Borrow>
    {
        public AvailableBookActiveBorrow(int bookId)
            : base(b => b.BookId == bookId && b.Status != BorrowStatus.Returned && b.Book.IsAvailable == true && b.IsDeleted == false)
        {
            AddInclude(b => b.Book);
        }
    }
}
