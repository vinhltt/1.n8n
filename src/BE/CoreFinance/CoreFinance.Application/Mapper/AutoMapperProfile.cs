using AutoMapper;
using CoreFinance.Application.DTOs;
using CoreFinance.Domain;

namespace CoreFinance.Application.Mapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Account, AccountCreateRequest>().ReverseMap();
        CreateMap<Account, AccountUpdateRequest>().ReverseMap();
        CreateMap<Account, AccountViewModel>().ReverseMap();

    }
}