using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Domain.Specifications
{
    public  class UserBorrowSpecification : BorrowSpecification
    {
        public UserBorrowSpecification(string userId)
        {
            AddCriteria(x => x.UserId == userId);
            _AddIncludes();
        }
    }
}
