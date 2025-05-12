using AutoMapper;
using BookLending.Application.Features.Books.DTOs;
using BookLending.Domain.Entities;
using BookLending.Domain.Interfaces;
using BookLending.Domain.Specifications;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Features.Books.Queries.GetAllBooks
{
    public class GetAllBooksHandler : IRequestHandler<GetAllBooksQuery, IEnumerable<BookDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IReadRepository<Book> _bookRepository;
        public GetAllBooksHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _bookRepository = unitOfWork.ReadRepository<Book>();
        }
        public async Task<IEnumerable<BookDto>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
        {
            var spec = new BookSpecification(request.BookParameters);
            var books = await _bookRepository.GetAllWithSpecAsync(spec);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }
    }
}
