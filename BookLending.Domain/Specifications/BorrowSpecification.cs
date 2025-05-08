using BookLending.Domain.Entities;
using BookLending.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Domain.Specifications
{
    public class BorrowSpecification : Specification<Borrow>
    {
        public BorrowSpecification()
        {
            AddCriteria(x => x.IsDeleted == false);
        }
        public BorrowSpecification(BorrowParameters borrowParameters)
            : base(x =>
                (string.IsNullOrEmpty(borrowParameters.UserId) || x.UserId == borrowParameters.UserId) &&
                (borrowParameters.BookId == null || x.BookId == borrowParameters.BookId) &&
                (string.IsNullOrEmpty(borrowParameters.Status) || x.Status == Enum.Parse<BorrowStatus>(borrowParameters.Status)) &&
                (borrowParameters.BorrowDate == null || x.BorrowDate == borrowParameters.BorrowDate) &&
                (borrowParameters.DueDate == null || x.DueDate == borrowParameters.DueDate) &&
                (borrowParameters.ReturnDate == null || x.ReturnDate == borrowParameters.ReturnDate) &&
                (borrowParameters.RemindersSent == null || x.RemindersSent == borrowParameters.RemindersSent) &&
                (x.IsDeleted == false)
            )
        {
            _AddIncludes();

            if (!string.IsNullOrEmpty(borrowParameters.SortBy))
                AddSorting(borrowParameters.SortBy, borrowParameters.IsSortAscending);
            else
                AddOrderBy(b => b.DueDate);

            AddPaging(borrowParameters.PageNumber, borrowParameters.PageSize);
        }

        public BorrowSpecification(int id) : base(x => x.Id == id && x.IsDeleted == false)
        {
            _AddIncludes();
        }

        //// Renamed the constructor to avoid conflict with the existing one
        //public BorrowSpecification(int bookId, bool isByBookId) : base(x => x.BookId == bookId)
        //{
        //    _AddIncludes();
        //}

        protected void _AddIncludes()
        {
            AddInclude(b => b.Book);
            AddInclude(b => b.User);
        }
    }
}
