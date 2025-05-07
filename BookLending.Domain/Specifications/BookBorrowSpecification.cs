using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Domain.Specifications
{
    public class BookBorrowSpecification : BorrowSpecification
    {
        public BookBorrowSpecification(int bookId)
        {   
            AddCriteria(x => x.BookId == bookId);
            AddCriteria(x => x.IsDeleted == false);
            _AddIncludes();
        }

    }
}
