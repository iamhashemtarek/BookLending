using AutoMapper;
using BookLending.Application.Features.Books.Commands.UpdateBook;
using BookLending.Application.Features.Borrows.DTOs;
using BookLending.Application.Interfaces;
using BookLending.Domain.Entities;
using BookLending.Domain.Enums;
using BookLending.Domain.Interfaces;
using BookLending.Infrastructure.Configuration;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Features.Borrows.Commands.ReturnBook
{
    public class ReturnBookHandler : IRequestHandler<ReturnBookCommand, BorrowDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<IBorrowService> _logger;
        private readonly IOptions<BorrowSettings> options;
        private readonly IWriteRepository<Borrow> _borrowRepository;
        public ReturnBookHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<IBorrowService> logger, IOptions<BorrowSettings> options)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            this.options = options;
            _borrowRepository = unitOfWork.WriteRepository<Borrow>();
        }
        public async Task<BorrowDto> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
        {
            var borrow = await _borrowRepository.GetByIdAsyncTracked(request.BorrowId);
            if (borrow == null)
                return null;
            if (borrow.Status != BorrowStatus.Returned)
            {
                borrow.ReturnDate = DateTime.UtcNow;
                borrow.Status = BorrowStatus.Returned;
                _borrowRepository.Update(borrow);
                var book = await _unitOfWork.Repository<Book>().GetByIdAsync(borrow.BookId);
                if (book != null)
                {
                    book.IsAvailable = true;
                    _unitOfWork.Repository<Book>().Update(book);
                }
                await _unitOfWork.CompleteAsync();
            }
            return _mapper.Map<BorrowDto>(borrow);
        }
    }
}
