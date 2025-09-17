using AutoMapper;
using WebApplication1.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Mapping;

public class BookProfile : Profile
{
    public BookProfile()
    {
        CreateMap<Book, BookResultDto>();
    }
}
