using BookLending.Application.Features.Borrows.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Features.Borrows.Commands.BorrowBook
{
    public class BorrowBookCommand : IRequest<BorrowDto>
    {
        public string UserId { get; set; }
        public BorrowBookDto BorrowBookDto { get; set; }
        public BorrowBookCommand(string userId, BorrowBookDto borrowBookDto)
        {
            UserId = userId;
            BorrowBookDto = borrowBookDto;
        }
    }
}
