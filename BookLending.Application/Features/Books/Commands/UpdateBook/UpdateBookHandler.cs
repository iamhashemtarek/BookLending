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

namespace BookLending.Application.Features.Books.Commands.UpdateBook
{
    public class UpdateBookHandler : IRequestHandler<UpdateBookCommand>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWriteRepository<Book> _bookRepository;
        public UpdateBookHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
            _bookRepository = unitOfWork.WriteRepository<Book>();
        }

        public async Task Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            var book = _mapper.Map<Book>(request.CreateBookDto);
            book.Id = request.Id;
            _bookRepository.Update(book);
            await unitOfWork.CompleteAsync();
            return;
        }
    }
}
