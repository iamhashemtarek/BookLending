using BookLending.Application.Features.Books.Commands.CreateBook;
using BookLending.Application.Features.Books.Commands.DeleteBook;
using BookLending.Application.Features.Books.Commands.UpdateBook;
using BookLending.Application.Features.Books.DTOs;
using BookLending.Application.Features.Books.Queries.GetAllAvailableBooks;
using BookLending.Application.Features.Books.Queries.GetAllBooks;
using BookLending.Application.Features.Books.Queries.GetBookById;
using BookLending.Application.Interfaces;
using BookLending.Common.Constants;
using BookLending.Common.Errors;
using BookLending.Domain.Specifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookLending.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BooksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks([FromQuery] BookParameters bookParameters)
        {
            var books = await _mediator.Send(new GetAllBooksQuery(bookParameters));
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            var book = await _mediator.Send(new GetBookByIdQuery(id));
            return book is null ? NotFound() : Ok(book);
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetAvailableBooks()
        {
            var books = await _mediator.Send(new GetAllAvailableBooksQuery());
            return Ok(books);
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<ActionResult<BookDto>> CreateBook(CreateBookDto bookDto)
        {
            var book = await _mediator.Send(new CreateBookCommand(bookDto));
            return Ok(book);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> UpdateBook(int id, CreateBookDto bookDto)
        {
            await _mediator.Send(new UpdateBookCommand(id, bookDto));
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> DeleteBook(int id)
        {
            await _mediator.Send(new DeleteBookCommand(id)); 
            return NoContent();
        }
    }
}
