using BookLending.Application.Features.Books.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Features.Books.Queries.GetBookById
{
    public class GetBookByIdQuery : IRequest<BookDto>
    {
        public int Id { get; set; }
        public GetBookByIdQuery(int id)
        {
            Id = id;
        }
    }
}
