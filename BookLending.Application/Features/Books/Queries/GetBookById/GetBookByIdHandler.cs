using AutoMapper;
using BookLending.Application.Features.Books.DTOs;
using BookLending.Domain.Entities;
using BookLending.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Features.Books.Queries.GetBookById
{
    public class GetBookByIdHandler : IRequestHandler<GetBookByIdQuery, BookDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IReadRepository<Book> _bookRepository;


        public GetBookByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _bookRepository = unitOfWork.ReadRepository<Book>();    
        }

        public async Task<BookDto> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.Id);
            if (book == null)
                return null;
            return _mapper.Map<BookDto>(book);
        }
    }
}
