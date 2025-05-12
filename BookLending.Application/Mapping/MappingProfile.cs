using AutoMapper;
using BookLending.Application.DTOs;
using BookLending.Application.Features.Books.DTOs;
using BookLending.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Book mappings
            CreateMap<Book, BookDto>();
            CreateMap<CreateBookDto, Book>();

            // Borrow mappings
            CreateMap<Borrow, BorrowDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
