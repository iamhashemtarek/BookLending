using BookLending.Application.Features.Borrows.Commands.BorrowBook;
using BookLending.Application.Features.Borrows.Commands.ReturnBook;
using BookLending.Application.Features.Borrows.DTOs;
using BookLending.Application.Features.Borrows.Queries.GetAllBorrows;
using BookLending.Application.Features.Borrows.Queries.GetBookBorrowHistoryById;
using BookLending.Application.Features.Borrows.Queries.GetBorrowById;
using BookLending.Application.Features.Borrows.Queries.GetUserBorrowHistoryById;
using BookLending.Application.Interfaces;
using BookLending.Common.Constants;
using BookLending.Common.Errors;
using BookLending.Domain.Specifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookLending.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BorrowsController : ControllerBase
    {
        private readonly IBorrowService _borrowService;
        private readonly IMediator _mediator;

        public BorrowsController(IBorrowService borrowService, IMediator mediator)
        {
            _borrowService = borrowService;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BorrowDto>>> GetBorrows([FromQuery] BorrowParameters borrowParameters)
        {
            if (User.IsInRole(AppRoles.Admin))
            {
                var borrows = await _mediator.Send(new GetAllBorrowsQuery(borrowParameters));
                return Ok(borrows);
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var borrows = await _mediator.Send(new GetUserBorrowHistoryByIdQuery(userId));
                return Ok(borrows);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowDto>> GetBorrow(int id)
        {
            var borrow = await _mediator.Send(new GetBorrowByIdQuery(id));
            if (borrow == null)
                return NotFound();

            if (!User.IsInRole(AppRoles.Admin) && borrow.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return BadRequest(new ApiErrorResponse(403));

            return Ok(borrow);

        }

        [HttpPost]
        public async Task<ActionResult<BorrowDto>> BorrowBook(BorrowBookDto borrowDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var borrow = await _mediator.Send(new BorrowBookCommand(userId, borrowDto));    
            return Ok(borrow);
        }

        [HttpPut("{id}/return")]
        public async Task<ActionResult<BorrowDto>> ReturnBook(int id)
        {
            var borrow = await _mediator.Send(new GetBorrowByIdQuery(id));
            if (borrow == null)
                return NotFound();

            if (!User.IsInRole(AppRoles.Admin) && borrow.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return BadRequest(new ApiErrorResponse(403));

            var returnedBorrow = await _mediator.Send(new ReturnBookCommand(id));
            return Ok(returnedBorrow);
        }

        [HttpGet("users/{userId}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<ActionResult<IEnumerable<BorrowDto>>> GetUserBorrows(string userId)
        {
            var borrows = await _mediator.Send(new GetUserBorrowHistoryByIdQuery(userId));
            return Ok(borrows);
        }

        [HttpGet("books/{bookId}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<ActionResult<IEnumerable<BorrowDto>>> GetBookBorrows(int bookId)
        {
            var borrows = await _mediator.Send(new GetBookBorrowHistoryByIdQuery(bookId));
            return Ok(borrows);
        }
    }
}
