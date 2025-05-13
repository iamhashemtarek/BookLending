using BookLending.Application.Features.Books.DTOs;
using BookLending.Domain.Specifications;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Features.Books.Queries.GetAllBooks
{
    public class GetAllBooksQuery : IRequest<IEnumerable<BookDto>>
    {
        public BookParameters BookParameters { get; set; }
        public GetAllBooksQuery(BookParameters bookParameters)
        {
            BookParameters = bookParameters;
        }
    }
}
