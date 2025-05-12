using AutoMapper;
using BookLending.Domain.Entities;
using BookLending.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Features.Books.Commands.DeleteBook
{
    public class DeleteBookHandler : IRequestHandler<DeleteBookCommand>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWriteRepository<Book> _bookRepository;
        public DeleteBookHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
            _bookRepository = unitOfWork.WriteRepository<Book>();
        }
        public async Task Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsyncTracked(request.Id);
            _bookRepository.Delete(book);
            await unitOfWork.CompleteAsync();
            return;
        }
    }
}
