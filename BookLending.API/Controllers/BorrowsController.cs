using BookLending.Application.DTOs;
using BookLending.Application.Interfaces;
using BookLending.Common.Errors;
using BookLending.Domain.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookLending.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowsController : ControllerBase
    {
        private readonly IBorrowService _borrowService;

        public BorrowsController(IBorrowService borrowService)
        {
            _borrowService = borrowService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BorrowDto>>> GetBorrows([FromQuery] BorrowParameters borrowParameters)
        {
            if (User.IsInRole("Admin"))
            {
                var borrows = await _borrowService.GetAllBorrowsAsync(borrowParameters);
                return Ok(borrows);
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var borrows = await _borrowService.GetUserBorrowsAsync(userId);
                return Ok(borrows);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowDto>> GetBorrow(int id)
        {
            var borrow = await _borrowService.GetBorrowByIdAsync(id);
            if (borrow == null)
                return NotFound();

            if (!User.IsInRole("Admin") && borrow.UserId.ToString() != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return BadRequest(new ApiErrorResponse(403));

            return Ok(borrow);

        }

        [HttpPost]
        public async Task<ActionResult<BorrowDto>> BorrowBook(BorrowBookDto borrowDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var borrow = await _borrowService.BorrowBookAsync(userId, borrowDto);
            return Ok(borrow);
        }

        [HttpPut("{id}/return")]
        public async Task<ActionResult<BorrowDto>> ReturnBook(int id)
        {
            var borrow = await _borrowService.GetBorrowByIdAsync(id);
            if (borrow == null)
                return NotFound();

            if (!User.IsInRole("Admin") && borrow.UserId.ToString() != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return BadRequest(new ApiErrorResponse(403));

            var returnedBorrow = await _borrowService.ReturnBookAsync(id);
            return Ok(returnedBorrow);
        }

        [HttpGet("users/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<BorrowDto>>> GetUserBorrows(string userId)
        {
            var borrows = await _borrowService.GetUserBorrowsAsync(userId);
            return Ok(borrows);
        }

        [HttpGet("books/{bookId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<BorrowDto>>> GetBookBorrows(int bookId)
        {
            var borrows = await _borrowService.GetBookBorrowHistoryAsync(bookId);
            return Ok(borrows);
        }
    }
}
