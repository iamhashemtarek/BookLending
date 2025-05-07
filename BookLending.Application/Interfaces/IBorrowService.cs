using BookLending.Application.DTOs;
using BookLending.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Interfaces
{
    public interface IBorrowService
    {
        Task<BorrowDto> BorrowBookAsync(string userId, BorrowBookDto borrowDto);
        Task<BorrowDto> ReturnBookAsync(int borrowId);
        Task<IEnumerable<BorrowDto>> GetUserBorrowsAsync(string userId);
        Task<IEnumerable<BorrowDto>> GetAllBorrowsAsync(BorrowParameters borrowParameters);
        Task<BorrowDto> GetBorrowByIdAsync(int id);
        Task<IEnumerable<BorrowDto>> GetBookBorrowHistoryAsync(int bookId);
        Task CheckOverdueBorrowsAsync();
    }
}
