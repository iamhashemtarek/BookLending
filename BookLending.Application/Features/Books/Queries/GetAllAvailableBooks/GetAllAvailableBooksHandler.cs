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

namespace BookLending.Application.Features.Books.Queries.GetAllAvailableBooks
{
    public class GetAllAvailableBooksHandler : IRequestHandler<GetAllAvailableBooksQuery, IEnumerable<BookDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IReadRepository<Book> _bookRepository;
        public GetAllAvailableBooksHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _bookRepository = unitOfWork.ReadRepository<Book>();
        }
        public async Task<IEnumerable<BookDto>> Handle(GetAllAvailableBooksQuery request, CancellationToken cancellationToken)
        {
            var spec = new AvailableBooksSpecification();
            var books = await _bookRepository.GetAllWithSpecAsync(spec);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }
    }
}
