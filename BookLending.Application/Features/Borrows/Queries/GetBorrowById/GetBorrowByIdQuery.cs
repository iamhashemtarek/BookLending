using BookLending.Application.Features.Borrows.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Features.Borrows.Queries.GetBorrowById
{
    public class GetBorrowByIdQuery : IRequest<BorrowDto>
    {
        public int Id { get; set; }
        public GetBorrowByIdQuery(int id)
        {
            Id = id;
        }
    }
}
