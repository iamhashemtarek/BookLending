using AutoMapper;
using BookLending.Application.DTOs;
using BookLending.Application.Interfaces;
using BookLending.Domain.Entities;
using BookLending.Domain.Enums;
using BookLending.Domain.Interfaces;
using BookLending.Domain.Specifications;
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
        private readonly IGenericRepository<Borrow> _borrowRepository;
        public BorrowService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _borrowRepository = unitOfWork.Repository<Borrow>();

        }
        public async Task<BorrowDto?> BorrowBookAsync(string userId, BorrowBookDto borrowDto)
        {
            var book = await _unitOfWork.Repository<Book>().GetByIdAsync(borrowDto.BookId);
            if (book == null) //|| book.IsAvailable == false
                return null;

            var borrow = new Borrow
            {
                BookId = borrowDto.BookId,
                UserId = userId,
                BorrowDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7), 
                Status = BorrowStatus.Borrowed,
                RemindersSent = 0,
            };
            await _borrowRepository.AddAsync(borrow);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<BorrowDto>(borrow);
        }

        public async Task CheckOverdueBorrowsAsync()
        {
            var spec = new OverdueBorrowsSpecification();
            var overdueBorrows = await _borrowRepository.GetAllWithSpecAsync(spec);
            foreach (var borrow in overdueBorrows)
            {
                borrow.Status = BorrowStatus.Overdue;
                borrow.RemindersSent++;
                _borrowRepository.Update(borrow);
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
                throw new Exception("Borrow record not found.");
            borrow.ReturnDate = DateTime.UtcNow;
            borrow.Status = BorrowStatus.Returned;
            _borrowRepository.Update(borrow);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<BorrowDto>(borrow);
        }
    }
}
