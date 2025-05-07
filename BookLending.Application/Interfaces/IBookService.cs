using BookLending.Application.DTOs;
using BookLending.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllBooksAsync(BookParameters bookParameters);
        Task<BookDto?> GetBookByIdAsync(int id);
        Task<BookDto> AddBookAsync(CreateBookDto bookDto);
        Task UpdateBookAsync(int id, CreateBookDto bookDto);
        Task DeleteBookAsync(int id);
        Task<IEnumerable<BookDto>> GetAvailableBooksAsync();
    }
}
