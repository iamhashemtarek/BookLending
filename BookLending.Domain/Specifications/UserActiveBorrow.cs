using BookLending.Domain.Entities;
using BookLending.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Domain.Specifications
{
    public class UserActiveBorrow : Specification<Borrow>
    {
        public UserActiveBorrow(string userId)
            : base(b => b.UserId == userId && b.Status != BorrowStatus.Returned && b.IsDeleted == false)
        {
            AddInclude(b => b.User);
        }
    }
}
