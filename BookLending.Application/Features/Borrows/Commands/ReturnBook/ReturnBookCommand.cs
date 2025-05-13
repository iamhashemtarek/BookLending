using BookLending.Application.Features.Borrows.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Features.Borrows.Commands.ReturnBook
{
    public class ReturnBookCommand : IRequest<BorrowDto>
    {
        public int BorrowId { get; set; }
        public ReturnBookCommand(int borrowId)
        {
            BorrowId = borrowId;
        }
    }
}
