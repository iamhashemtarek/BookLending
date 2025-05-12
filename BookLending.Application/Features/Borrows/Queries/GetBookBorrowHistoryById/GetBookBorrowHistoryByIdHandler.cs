using AutoMapper;
using BookLending.Application.Features.Books.Commands.UpdateBook;
using BookLending.Application.Features.Borrows.DTOs;
using BookLending.Application.Interfaces;
using BookLending.Domain.Entities;
using BookLending.Domain.Interfaces;
using BookLending.Domain.Specifications;
using BookLending.Infrastructure.Configuration;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Features.Borrows.Queries.GetBookBorrowHistoryById
{
    public class GetBookBorrowHistoryByIdHandler : IRequestHandler<GetBookBorrowHistoryByIdQuery, IEnumerable<BorrowDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<IBorrowService> _logger;
        private readonly IOptions<BorrowSettings> options;
        private readonly IReadRepository<Borrow> _borrowRepository;
        public GetBookBorrowHistoryByIdHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<IBorrowService> logger, IOptions<BorrowSettings> options)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            this.options = options;
            _borrowRepository = unitOfWork.ReadRepository<Borrow>();
        }
        public async Task<IEnumerable<BorrowDto>> Handle(GetBookBorrowHistoryByIdQuery request, CancellationToken cancellationToken)
        {
            var spec = new BookBorrowSpecification(request.BookId);
            var borrows = await _borrowRepository.GetAllWithSpecAsync(spec);
            return _mapper.Map<IEnumerable<BorrowDto>>(borrows);
        }
    }
}
