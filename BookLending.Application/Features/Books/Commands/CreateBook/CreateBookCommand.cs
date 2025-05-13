using BookLending.Application.Features.Books.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Features.Books.Commands.CreateBook
{
    public class CreateBookCommand : IRequest<BookDto>
    {
        public CreateBookDto CreateBookDto { get; set; }
        public CreateBookCommand(CreateBookDto createBookDto)
        {
            CreateBookDto = createBookDto;
        }
    }
}
