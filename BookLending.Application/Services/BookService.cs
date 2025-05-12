using AutoMapper;
using BookLending.Application.Features.Books.DTOs;
using BookLending.Application.Interfaces;
using BookLending.Domain.Entities;
using BookLending.Domain.Interfaces;
using BookLending.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Book> _bookRepository;

        public BookService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
            _bookRepository = unitOfWork.Repository<Book>();
        }
        public async Task<BookDto> AddBookAsync(CreateBookDto bookDto)
        {
            var book = _mapper.Map<Book>(bookDto);
            await _bookRepository.AddAsync(book);
            await unitOfWork.CompleteAsync();
            return _mapper.Map<BookDto>(book);
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            _bookRepository.Delete(book);
            await unitOfWork.CompleteAsync();
            return;
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync(BookParameters bookParameters)
        {
            var spec = new BookSpecification(bookParameters);
            var books = await _bookRepository.GetAllWithSpecAsync(spec);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<IEnumerable<BookDto>> GetAvailableBooksAsync()
        {
            var spec = new AvailableBooksSpecification();
            var books = await _bookRepository.GetAllWithSpecAsync(spec);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
                return null;
            return _mapper.Map<BookDto>(book);
        }

        public async Task UpdateBookAsync(int id, CreateBookDto bookDto)
        {
            var book = _mapper.Map<Book>(bookDto);
            book.Id = id;
            _bookRepository.Update(book);
            await unitOfWork.CompleteAsync();
            return;
        }
    }
}
