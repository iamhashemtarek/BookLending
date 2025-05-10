using AutoMapper;
using BookLending.Application.DTOs;
using BookLending.Application.Interfaces;
using BookLending.Domain.Entities;
using BookLending.Domain.Enums;
using BookLending.Domain.Interfaces;
using BookLending.Domain.Specifications;
using BookLending.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Services
{
    public class BorrowService : IBorrowService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<IBorrowService> _logger;
        private readonly IOptions<BorrowSettings> options;
        private readonly IGenericRepository<Borrow> _borrowRepository;
        public BorrowService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<IBorrowService> logger, IOptions<BorrowSettings> options)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            this.options = options;
            _borrowRepository = unitOfWork.Repository<Borrow>();
        }
        public async Task<BorrowDto?> BorrowBookAsync(string userId, BorrowBookDto borrowDto)
        {
            var availableBookSpec = new AvailableBooksSpecification(borrowDto.BookId);
            var availableBook = await _unitOfWork.Repository<Book>().GetWithSpecAsync(availableBookSpec);

            var userActiveBorrowSpec = new UserActiveBorrow(userId);
            var userActiveBorrows = await _borrowRepository.CountAsync(userActiveBorrowSpec);
            
            if (availableBook == null || userActiveBorrows >= options.Value.MaxBorrowedBooks)
                return null;

            var borrow = new Borrow
            {
                BookId = borrowDto.BookId,
                UserId = userId,
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

        public async Task CheckOverdueBorrowsAsync()
        {
            var spec = new OverdueBorrowsSpecification();
            var overdueBorrows = await _borrowRepository.GetAllWithSpecAsync(spec);
            foreach (var borrow in overdueBorrows)
            {
                if (borrow.Status == BorrowStatus.Borrowed)
                    borrow.Status = BorrowStatus.Overdue;

                borrow.RemindersSent++;
                borrow.LastReminderDate = DateTime.UtcNow;
                _borrowRepository.Update(borrow);
                _logger.LogInformation($"Reminder sent for Borrow ID {borrow.Id}, Book '{borrow.Book.Title}', User '{borrow.User.Email}'");
            }
            await _unitOfWork.CompleteAsync();
            return;
        }

        public async Task<IEnumerable<BorrowDto>> GetAllBorrowsAsync(BorrowParameters borrowParameters)
        {
            var spec = new BorrowSpecification(borrowParameters);
            var borrows = await _borrowRepository.GetAllWithSpecAsync(spec);
            return _mapper.Map<IEnumerable<BorrowDto>>(borrows);
        }

        public async Task<BorrowDto?> GetBorrowByIdAsync(int id)
        {
            var spec = new BorrowSpecification(id);
            var borrow = await _borrowRepository.GetWithSpecAsync(spec);
            if (borrow == null)
                return null;
            return _mapper.Map<BorrowDto>(borrow);
        }

        public async Task<IEnumerable<BorrowDto>> GetBookBorrowHistoryAsync(int bookId)
        {
            var spec = new BookBorrowSpecification(bookId);
            var borrows = await _borrowRepository.GetAllWithSpecAsync(spec);
            return _mapper.Map<IEnumerable<BorrowDto>>(borrows);
        }

        public async Task<IEnumerable<BorrowDto>> GetUserBorrowsAsync(string userId)
        {
            var spec = new UserBorrowSpecification(userId);
            var borrows = await _borrowRepository.GetAllWithSpecAsync(spec);
            return _mapper.Map<IEnumerable<BorrowDto>>(borrows);
        }

        public async Task<BorrowDto> ReturnBookAsync(int borrowId)
        {
            var borrow = await _borrowRepository.GetByIdAsync(borrowId);
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
