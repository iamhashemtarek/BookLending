using BookLending.Application.Features.Borrows.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Features.Borrows.Queries.GetUserBorrowHistoryById
{
    public class GetUserBorrowHistoryByIdQuery : IRequest<IEnumerable<BorrowDto>>
    {
        public string UserId { get; set; }
        public GetUserBorrowHistoryByIdQuery(string userId)
        {
            UserId = userId;
        }
    }
}
