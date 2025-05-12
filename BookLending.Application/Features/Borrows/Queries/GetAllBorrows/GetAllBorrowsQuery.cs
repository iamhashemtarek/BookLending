using BookLending.Application.Features.Borrows.DTOs;
using BookLending.Domain.Specifications;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Features.Borrows.Queries.GetAllBorrows
{
    public class GetAllBorrowsQuery : IRequest<IEnumerable<BorrowDto>>
    {
        public BorrowParameters BorrowParameters { get; set; }
        public GetAllBorrowsQuery(BorrowParameters borrowParameters)
        {
            BorrowParameters = borrowParameters;
        }
    }
}
