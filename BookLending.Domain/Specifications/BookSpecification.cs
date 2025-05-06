using BookLending.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Domain.Specifications
{
    public class BookSpecification : Specification<Book>
    {
        public BookSpecification(BookParameters bookParameters)
            : base(b => (string.IsNullOrEmpty(bookParameters.Title) || b.Title.Contains(bookParameters.Title)) &&
                        (string.IsNullOrEmpty(bookParameters.Author) || b.Author.Contains(bookParameters.Author)) &&
                        (string.IsNullOrEmpty(bookParameters.ISBN) || b.ISBN.Contains(bookParameters.ISBN)) &&
                        (bookParameters.PublishedYear == 0 || b.PublishedYear == bookParameters.PublishedYear) &&
                        (b.IsDeleted == true))
        {
            if(!string.IsNullOrEmpty(bookParameters.SortBy))
                AddSorting(bookParameters.SortBy, bookParameters.IsSortAscending);
            else
                AddOrderBy(b => b.Title);
            
            if(bookParameters.PageNumber > 0 && bookParameters.PageSize > 0)
                AddPaging(bookParameters.PageNumber, bookParameters.PageSize);
        }

        public BookSpecification(int bookId)
            : base(b => b.Id == bookId)
        {
        }

    }
}
