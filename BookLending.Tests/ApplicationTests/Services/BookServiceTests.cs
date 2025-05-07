using AutoMapper;
using BookLending.Application.DTOs;
using BookLending.Application.Interfaces;
using BookLending.Application.Services;
using BookLending.Domain.Entities;
using BookLending.Domain.Interfaces;
using BookLending.Domain.Specifications;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Tests.ApplicationTests.Services
{
    public class BookServiceTests
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Book> _bookRepository;
        private readonly IBookService bookService;
        public BookServiceTests()
        {
            unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.Repository<Book>().Returns(Substitute.For<IGenericRepository<Book>>());
            _bookRepository = unitOfWork.Repository<Book>();
            _mapper = Substitute.For<IMapper>();
            bookService = new BookService(unitOfWork, _mapper);
        }

        [Fact]
        public async Task AddBookAsync_ShouldReturnAddBook_WhenCalled()
        {
            // Arrange
            var createBookDto = new CreateBookDto
            {
                Title = "Test Book",
                Author = "Test Author",
                ISBN = "1234567890",
                PublishedYear = 2023,
                Description = "Test Description",
                CoverImageUrl = "http://example.com/test.jpg"
            };
            var book = new Book
            {
                Id = 1,
                Title = createBookDto.Title,
                Author = createBookDto.Author,
                ISBN = createBookDto.ISBN,
                PublishedYear = createBookDto.PublishedYear,
                Description = createBookDto.Description,
                CoverImageUrl = createBookDto.CoverImageUrl
            };
            var bookDto = new BookDto
            {
                Id = 1,
                Title = createBookDto.Title,
                Author = createBookDto.Author,
                ISBN = createBookDto.ISBN,
                PublishedYear = createBookDto.PublishedYear,
                Description = createBookDto.Description,
                CoverImageUrl = createBookDto.CoverImageUrl
            };

            _mapper.Map<BookDto>(book).Returns(bookDto);
            _mapper.Map<Book>(createBookDto).Returns(book);

            //act
            var result = await bookService.AddBookAsync(createBookDto);

            //Assert
            await _bookRepository.Received(1).AddAsync(book);
            await unitOfWork.Received(1).CompleteAsync();
            Assert.IsType<BookDto>(result);
        }

        [Fact]
        public async Task GetAllBooksAsync_ShouldReturnAllBooks_WhenCalled()
        {
            //arrange
            var books = new List<Book>
            {
                new Book { Id = 1, Title = "Test Book 1", Author = "Test Author 1" },
                new Book { Id = 2, Title = "Test Book 2", Author = "Test Author 2" }
            };
            var bookDtos = new List<BookDto>
            {
                new BookDto { Id = 1, Title = "Test Book 1", Author = "Test Author 1" },
                new BookDto { Id = 2, Title = "Test Book 2", Author = "Test Author 2" }
            };

            _bookRepository.GetAllWithSpecAsync(Arg.Any<BookSpecification>()).Returns(books);
            _mapper.Map<IEnumerable<BookDto>>(books).Returns(bookDtos);

            //act
            var result = await bookService.GetAllBooksAsync(new BookParameters());

            //assert
            await _bookRepository.Received(1).GetAllWithSpecAsync(Arg.Any<BookSpecification>());
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetBookByIdAsync_ShouldReturnBook_WhenBookExists()
        {
            // Arrange
            var bookId = 1;
            var book = new Book { Id = bookId, Title = "Test Book", Author = "Test Author" };
            var bookDto = new BookDto { Id = bookId, Title = "Test Book", Author = "Test Author" };
            _bookRepository.GetByIdAsync(bookId).Returns(book);
            _mapper.Map<BookDto>(book).Returns(bookDto);
            // Act
            var result = await bookService.GetBookByIdAsync(bookId);
            // Assert
            await _bookRepository.Received(1).GetByIdAsync(bookId);
            _mapper.Received(1).Map<BookDto>(book);
        }

        [Fact]
        public async Task GetBookByIdAsync_ShouldReturnNull_WhenBookDoesNotExist()
        {
            // Arrange
            var bookId = 1;
            _bookRepository.GetByIdAsync(bookId).Returns((Book)null);
            // Act
            var result = await bookService.GetBookByIdAsync(bookId);
            // Assert
            await _bookRepository.Received(1).GetByIdAsync(bookId);
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateBookAsync_ShouldUpdateBook_WhenCalled()
        {
            // Arrange
            var bookId = 1;
            var createBookDto = new CreateBookDto
            {
                Title = "Updated Book",
                Author = "Updated Author",
                ISBN = "0987654321",
                PublishedYear = 2024,
                Description = "Updated Description",
                CoverImageUrl = "http://example.com/updated.jpg"
            };
            var book = new Book
            {
                Id = bookId,
                Title = createBookDto.Title,
                Author = createBookDto.Author,
                ISBN = createBookDto.ISBN,
                PublishedYear = createBookDto.PublishedYear,
                Description = createBookDto.Description,
                CoverImageUrl = createBookDto.CoverImageUrl
            };
            _mapper.Map<Book>(createBookDto).Returns(book);
            _bookRepository.GetByIdAsync(bookId).Returns(book);
            unitOfWork.CompleteAsync().Returns(Task.FromResult(1));
            // Act
            await bookService.UpdateBookAsync(bookId, createBookDto);
            // Assert
            await unitOfWork.Received(1).CompleteAsync();
        }

        [Fact]
        public async Task DeleteBookAsync_ShouldDeleteBook_WhenCalled()
        {
            // Arrange
            var bookService = new BookService(unitOfWork, _mapper);
            var bookId = 1;
            var book = new Book { Id = bookId, Title = "Test Book", Author = "Test Author" };
            _bookRepository.GetByIdAsync(bookId).Returns(book);
            // Act
            await bookService.DeleteBookAsync(bookId);
            // Assert
            await _bookRepository.Received(1).GetByIdAsync(bookId);
            _bookRepository.Received(1).Delete(book);
            await unitOfWork.Received(1).CompleteAsync();
        }

        [Fact]
        public async Task GetAvailableBooksAsync_ShouldReturnAvailableBooks_WhenCalled()
        {
            // Arrange
            var availableBooks = new List<Book>
            {
                new Book { Id = 1, Title = "Available Book 1", Author = "Author 1", IsAvailable = true },
                new Book { Id = 2, Title = "Available Book 2", Author = "Author 2", IsAvailable = true }
            };
            var availableBookDtos = new List<BookDto>
            {
                new BookDto { Id = 1, Title = "Available Book 1", Author = "Author 1" },
                new BookDto { Id = 2, Title = "Available Book 2", Author = "Author 2" }
            };
            _bookRepository.GetAllWithSpecAsync(Arg.Any<AvailableBooksSpecification>()).Returns(availableBooks);
            _mapper.Map<IEnumerable<BookDto>>(availableBooks).Returns(availableBookDtos);
            // Act
            var result = await bookService.GetAvailableBooksAsync();
            // Assert
            await _bookRepository.Received(1).GetAllWithSpecAsync(Arg.Any<AvailableBooksSpecification>());
            _mapper.Received(1).Map<IEnumerable<BookDto>>(availableBooks);
        }

        [Fact]
        public async Task GetAvailableBooksAsync_ShouldReturnEmptyList_WhenNoAvailableBooks()
        {
            // Arrange
            var availableBooks = new List<Book>();
            var availableBookDtos = new List<BookDto>();
            _bookRepository.GetAllWithSpecAsync(Arg.Any<AvailableBooksSpecification>()).Returns(availableBooks);
            _mapper.Map<IEnumerable<BookDto>>(availableBooks).Returns(availableBookDtos);
            // Act
            var result = await bookService.GetAvailableBooksAsync();
            // Assert
            await _bookRepository.Received(1).GetAllWithSpecAsync(Arg.Any<AvailableBooksSpecification>());
            _mapper.Received(1).Map<IEnumerable<BookDto>>(availableBooks);
        }


    }
}
