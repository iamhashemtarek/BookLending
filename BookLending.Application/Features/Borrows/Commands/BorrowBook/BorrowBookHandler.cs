using AutoMapper;
using BookLending.Application.Features.Books.Commands.UpdateBook;
using BookLending.Application.Features.Borrows.DTOs;
using BookLending.Application.Interfaces;
using BookLending.Domain.Entities;
using BookLending.Domain.Enums;
using BookLending.Domain.Interfaces;
using BookLending.Domain.Specifications;
using BookLending.Infrastructure.Configuration;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Features.Borrows.Commands.BorrowBook
{
    public class BorrowBookHandler : IRequestHandler<BorrowBookCommand, BorrowDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<IBorrowService> _logger;
        private readonly IOptions<BorrowSettings> options;
        private readonly IWriteRepository<Borrow> _borrowRepository;
        public BorrowBookHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<IBorrowService> logger, IOptions<BorrowSettings> options)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            this.options = options;
            _borrowRepository = unitOfWork.WriteRepository<Borrow>();
        }
        public async Task<BorrowDto> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
        {
            var availableBookSpec = new AvailableBooksSpecification(request.BorrowBookDto.BookId);
            var availableBook = await _unitOfWork.WriteRepository<Book>().GetWithSpecAsyncTracked(availableBookSpec);

            var userActiveBorrowSpec = new UserActiveBorrow(request.UserId);
            var userActiveBorrows = await _unitOfWork.ReadRepository<Borrow>().CountAsync(userActiveBorrowSpec);

            if (availableBook == null || userActiveBorrows >= options.Value.MaxBorrowedBooks)
                return null;

            var borrow = new Borrow
            {
                BookId = request.BorrowBookDto.BookId,
                UserId = request.UserId,
                BorrowDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(options.Value.MaxBorrowDurationDays),
                Status = BorrowStatus.Borrowed,
                Book = availableBook,
                RemindersSent = 0,
            };
            availableBook.IsAvailable = false;
            await _borrowRepository.AddAsync(borrow);
            _unitOfWork.Repository<Book>().Update(availableBook);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<BorrowDto>(borrow);
        }
    }
}
