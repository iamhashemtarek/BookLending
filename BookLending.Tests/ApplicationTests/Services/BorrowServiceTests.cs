using AutoMapper;
using BookLending.Application.DTOs;
using BookLending.Application.Interfaces;
using BookLending.Application.Services;
using BookLending.Domain.Entities;
using BookLending.Domain.Enums;
using BookLending.Domain.Interfaces;
using BookLending.Domain.Specifications;
using BookLending.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.Extensions;
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
        private readonly IOptions<BorrowSettings> options;
        public BorrowServiceTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _unitOfWork.Repository<Borrow>().Returns(Substitute.For<IGenericRepository<Borrow>>());
            _unitOfWork.Repository<Book>().Returns(Substitute.For<IGenericRepository<Book>>());
            _borrowRepository = _unitOfWork.Repository<Borrow>();
            _mapper = Substitute.For<IMapper>();
            _logger = Substitute.For<ILogger<IBorrowService>>();
            options = Substitute.For<IOptions<BorrowSettings>>();
            options.Value.Returns(new BorrowSettings
            {
                MaxBorrowedBooks = 1,
                MaxBorrowDurationDays = 7
            });

            _borrowService = new BorrowService(_unitOfWork, _mapper, _logger, options);
        }

        [Fact]
        public async Task BorrowBookAsync_ShouldReturnBorrowDto_WhenBookIsAvailable_AndUserHasNoActiveBorrows()
        {
            // Arrange
            var userId = "user123";
            var borrowBookDto = new BorrowBookDto { BookId = 1 };
            var availableBook = new Book { Id = 1, IsAvailable = true };
            var borrow = new Borrow
            {
                BookId = 1,
                UserId = userId,
                BorrowDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7),
                Status = BorrowStatus.Borrowed,
                Book = availableBook
            };

            _unitOfWork.Repository<Book>().GetWithSpecAsync(Arg.Any<ISpecification<Book>>())
                .Returns(availableBook);

            _borrowRepository.CountAsync(Arg.Any<ISpecification<Borrow>>())
                .Returns(0);

            _mapper.Map<BorrowDto>(Arg.Any<Borrow>()).Returns(new BorrowDto
            {
                BookId = 1,
                UserId = userId,
            });

            // Act
            var result = await _borrowService.BorrowBookAsync(userId, borrowBookDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(1, result.BookId);

            await _borrowRepository.Received(1).AddAsync(Arg.Any<Borrow>());
            await _unitOfWork.Received(1).CompleteAsync();
            _unitOfWork.Repository<Book>().Received(1).Update(availableBook);
        }

        [Fact]
        public async Task BorrowBookAsync_ShouldReturnNull_WhenBookIsNotAvailable()
        {
            // Arrange
            _unitOfWork.Repository<Book>().GetWithSpecAsync(Arg.Any<ISpecification<Book>>())
                .Returns((Book?)null);

            _borrowRepository.CountAsync(Arg.Any<ISpecification<Borrow>>())
                .Returns(0);

            // Act
            var result = await _borrowService.BorrowBookAsync("user123", new BorrowBookDto { BookId = 1 });

            // Assert
            Assert.Null(result);
            await _borrowRepository.DidNotReceive().AddAsync(Arg.Any<Borrow>());
        }

        [Fact]
        public async Task BorrowBookAsync_ShouldReturnNull_WhenUserReachedMaxBorrowLimit()
        {
            // Arrange
            _unitOfWork.Repository<Book>().GetWithSpecAsync(Arg.Any<ISpecification<Book>>())
                .Returns(new Book { Id = 1, IsAvailable = true });

            _borrowRepository.CountAsync(Arg.Any<ISpecification<Borrow>>())
                .Returns(1); // == MaxBorrowedBooks

            // Act
            var result = await _borrowService.BorrowBookAsync("user123", new BorrowBookDto { BookId = 1 });

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
