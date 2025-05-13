using BookLending.Application.Features.Books.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Features.Books.Commands.UpdateBook
{
    public class UpdateBookCommand : IRequest
    {
        public int Id { get; set; }
        public CreateBookDto CreateBookDto { get; set; }    
        public UpdateBookCommand(int id, CreateBookDto createBookDto)
        {
            Id = id;
            CreateBookDto = createBookDto;
        }
    }
}
