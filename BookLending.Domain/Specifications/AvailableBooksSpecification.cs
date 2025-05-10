using BookLending.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Domain.Specifications
{
    public class AvailableBooksSpecification : Specification<Book>
    {
        public AvailableBooksSpecification()
            : base(b => b.IsAvailable == true && b.IsDeleted == false)
        {
            AddOrderBy(b => b.Title);
        }
        public AvailableBooksSpecification(int id)
            : base(b => b.IsAvailable == true && b.IsDeleted == false && b.Id == id)
        {
        }
    }
}
