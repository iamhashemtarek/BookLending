using BookLending.Application.Features.Borrows.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Features.Borrows.Queries.GetBookBorrowHistoryById
{
    public class GetBookBorrowHistoryByIdQuery : IRequest<IEnumerable<BorrowDto>>
    {
        public int BookId { get; set; }
        public GetBookBorrowHistoryByIdQuery(int bookId)
        {
            BookId = bookId;
        }
    }
}
