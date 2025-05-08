using BookLending.Application.DTOs;
using BookLending.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Interfaces
{
    public interface IAccountService
    {
        Task<string> RegisterAsync(CreateUserDto user);
        Task<string> LoginAsync(LoginDto user);
        Task<string> CreateTokenAsync(AppUser user);
    }
}
