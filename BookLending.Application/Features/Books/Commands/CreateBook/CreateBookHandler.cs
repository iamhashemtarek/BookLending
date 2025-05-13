using AutoMapper;
using BookLending.Application.Features.Books.DTOs;
using BookLending.Domain.Entities;
using BookLending.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Features.Books.Commands.CreateBook
{
    public class CreateBookHandler : IRequestHandler<CreateBookCommand, BookDto>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWriteRepository<Book> _bookRepository;
        public CreateBookHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
            _bookRepository = unitOfWork.WriteRepository<Book>();
        }
        public async Task<BookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            var book = _mapper.Map<Book>(request.CreateBookDto);
            await _bookRepository.AddAsync(book);
            await unitOfWork.CompleteAsync();
            return _mapper.Map<BookDto>(book);
        }
    }

}
