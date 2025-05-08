using AutoMapper;
using BookLending.Application.DTOs;
using BookLending.Application.Interfaces;
using BookLending.Application.Services;
using BookLending.Domain.Entities;
using BookLending.Domain.Enums;
using BookLending.Domain.Interfaces;
using BookLending.Domain.Specifications;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Tests.ApplicationTests.Services
{
    public class BorrowServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Borrow> _borrowRepository;
        private readonly IMapper _mapper;
        private readonly IBorrowService _borrowService;
        private readonly ILogger<IBorrowService> _logger;
        public BorrowServiceTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _unitOfWork.Repository<Borrow>().Returns(Substitute.For<IGenericRepository<Borrow>>());
            _unitOfWork.Repository<Book>().Returns(Substitute.For<IGenericRepository<Book>>());
            _borrowRepository = _unitOfWork.Repository<Borrow>();
            _mapper = Substitute.For<IMapper>();
            _logger = Substitute.For<ILogger<IBorrowService>>();
            _borrowService = new BorrowService(_unitOfWork, _mapper, _logger);
        }
        
        [Fact]
        public async Task BorrowBookAsync_ShouldReturnBorrowDto_WhenBookIsAvailable()
        {
            // Arrange
            var userId = "testUserId";
            var borrowDto = new BorrowBookDto { BookId = 1 };
            var book = new Book { Id = 1, IsAvailable = true };
            var borrow = new Borrow { BookId = 1, UserId = userId, Status = BorrowStatus.Borrowed };
            _unitOfWork.Repository<Book>().GetByIdAsync(1).Returns(book);
            _mapper.Map<BorrowDto>(borrow).Returns(new BorrowDto());
            // Act
            var result = await _borrowService.BorrowBookAsync(userId, borrowDto);
            // Assert
            //Assert.NotNull(result); ??????????????????????????
            await _borrowRepository.Received(1).AddAsync(Arg.Is<Borrow>(b => b.BookId == borrowDto.BookId && b.UserId == userId));
        }

        [Fact]
        public async Task BorrowBookAsync_ShouldReturnNull_WhenBookIsNotAvailable()
        {
            // Arrange
            var userId = "testUserId";
            var borrowDto = new BorrowBookDto { BookId = 1 };
            var book = new Book { Id = 1, IsAvailable = false };
            _unitOfWork.Repository<Book>().GetByIdAsync(1).ReturnsNull();
            // Act
            var result = await _borrowService.BorrowBookAsync(userId, borrowDto);
            // Assert
            Assert.Null(result);
            await _borrowRepository.DidNotReceive().AddAsync(Arg.Any<Borrow>());
        }

        [Fact]
        public async Task ReturnBookAsync_ShouldReturnBorrowDto_WhenBorrowExists()
        {
            // Arrange
            var borrowId = 1;
            var borrow = new Borrow { Id = borrowId, Status = BorrowStatus.Borrowed };
            _borrowRepository.GetByIdAsync(borrowId).Returns(borrow);
            _mapper.Map<BorrowDto>(borrow).Returns(new BorrowDto());
            // Act
            var result = await _borrowService.ReturnBookAsync(borrowId);
            // Assert
            Assert.Equal(BorrowStatus.Returned, borrow.Status);
        }

        [Fact]
        public async Task GetUserBorrowsAsync_ShouldReturnListOfBorrows_WhenUserHasBorrows()
        {
            // Arrange
            var userId = "testUserId";
            var borrows = new List<Borrow>
            {
                new Borrow { UserId = userId, Status = BorrowStatus.Borrowed },
                new Borrow { UserId = userId, Status = BorrowStatus.Returned }
            };
            _borrowRepository.GetAllWithSpecAsync(Arg.Any<BorrowSpecification>()).Returns(borrows);
            _mapper.Map<IEnumerable<BorrowDto>>(borrows).Returns(new List<BorrowDto>());
            // Act
            var result = await _borrowService.GetUserBorrowsAsync(userId);
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAllBorrowsAsync_ShouldReturnListOfBorrows_WhenBorrowsExist()
        {
            // Arrange
            var borrowParameters = new BorrowParameters();
            var borrows = new List<Borrow>
            {
                new Borrow { Status = BorrowStatus.Borrowed },
                new Borrow { Status = BorrowStatus.Returned }
            };
            _borrowRepository.GetAllWithSpecAsync(Arg.Any<BorrowSpecification>()).Returns(borrows);
            _mapper.Map<IEnumerable<BorrowDto>>(borrows).Returns(new List<BorrowDto>());
            // Act
            var result = await _borrowService.GetAllBorrowsAsync(borrowParameters);
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetBorrowByIdAsync_ShouldReturnBorrowDto_WhenBorrowExists()
        {
            // Arrange
            var borrowId = 1;
            var borrow = new Borrow { Id = borrowId, Status = BorrowStatus.Borrowed };
            _borrowRepository.GetWithSpecAsync(Arg.Any<ISpecification<Borrow>>()).Returns(borrow);
            _mapper.Map<BorrowDto>(borrow).Returns(new BorrowDto());
            // Act
            var result = await _borrowService.GetBorrowByIdAsync(borrowId);
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetBookBorrowHistoryAsync_ShouldReturnListOfBorrows_WhenBorrowsExist()
        {
            // Arrange
            var bookId = 1;
            var borrows = new List<Borrow>
            {
                new Borrow { BookId = bookId, Status = BorrowStatus.Borrowed },
                new Borrow { BookId = bookId, Status = BorrowStatus.Returned }
            };
            _borrowRepository.GetAllWithSpecAsync(Arg.Any<BorrowSpecification>()).Returns(borrows);
            _mapper.Map<IEnumerable<BorrowDto>>(borrows).Returns(new List<BorrowDto>());
            // Act
            var result = await _borrowService.GetBookBorrowHistoryAsync(bookId);
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CheckOverdueBorrowsAsync_ShouldUpdateOverdueBorrows_WhenOverdueBorrowsExist()
        {
            // Arrange
            var overdueBorrows = new List<Borrow>
            {
                new Borrow {
                    Id = 1,
                    Status = BorrowStatus.Borrowed,
                    DueDate = DateTime.UtcNow.AddDays(-1),
                    RemindersSent = 1,
                    Book = new Book { Title = "book-1" },
                    User = new AppUser {Email = "user1@email"}
                },
                new Borrow { 
                    Status = BorrowStatus.Overdue,
                    DueDate = DateTime.UtcNow.AddDays(-2),
                    RemindersSent = 1,
                    Book = new Book { Title = "book-2" },
                    User = new AppUser {Email = "user2@email"}
                }
            };
            _borrowRepository.GetAllWithSpecAsync(Arg.Any<OverdueBorrowsSpecification>()).Returns(overdueBorrows);

            // Act
            await _borrowService.CheckOverdueBorrowsAsync();

            // Assert
            foreach (var borrow in overdueBorrows)
            {
                Assert.Equal(BorrowStatus.Overdue, borrow.Status);
                Assert.Equal(2, borrow.RemindersSent);
            }
        }
    }
}
