using AutoMapper;
using PlanningInvestment.Application.DTOs.ExpectedTransaction;
using PlanningInvestment.Application.DTOs.RecurringTransactionTemplate;
using PlanningInvestment.Domain.Entities;
using PlanningInvestment.Domain.Enums;

namespace CoreFinance.Application.Mapper;

/// <summary>
    /// (EN) AutoMapper profile for mapping between domain entities and DTOs.<br/>
    /// (VI) Cấu hình AutoMapper để ánh xạ giữa các entity miền (domain) và DTO.
/// </summary>
public class AutoMapperProfile : Profile
{
    /// <summary>
    /// (EN) Initializes a new instance of the <see cref="AutoMapperProfile"/> class.<br/>
    /// (VI) Khởi tạo một phiên bản mới của lớp <see cref="AutoMapperProfile"/>.
/// </summary>
    public AutoMapperProfile()
    {
        CreateMap<ExpectedTransaction, ExpectedTransactionCreateRequest>().ReverseMap();
        CreateMap<ExpectedTransaction, ExpectedTransactionUpdateRequest>().ReverseMap();
        CreateMap<ExpectedTransaction, ExpectedTransactionViewModel>()
            .ForMember(dest => dest.TemplateName,
                opt => opt.MapFrom(src =>
                    src.RecurringTransactionTemplate != null ? src.RecurringTransactionTemplate.Name : string.Empty))
            .ForMember(dest => dest.AccountName,
                opt => opt.MapFrom(src => src.Account != null ? src.Account.Name : string.Empty))
            .ForMember(dest => dest.AccountType,
                opt => opt.MapFrom(src => src.Account != null ? (AccountType?)src.Account.Type : null))
            .ForMember(dest => dest.HasActualTransaction, opt => opt.MapFrom(src => src.ActualTransactionId.HasValue))
            .ForMember(dest => dest.DaysUntilDue,
                opt => opt.MapFrom(src => (int)(src.ExpectedDate - DateTime.UtcNow).TotalDays))
            .ReverseMap()
            .ForMember(dest => dest.RecurringTransactionTemplate, opt => opt.Ignore())
            .ForMember(dest => dest.Account, opt => opt.Ignore())
            .ForMember(dest => dest.ActualTransaction, opt => opt.Ignore());
        CreateMap<RecurringTransactionTemplate, RecurringTransactionTemplateCreateRequest>().ReverseMap();
        CreateMap<RecurringTransactionTemplate, RecurringTransactionTemplateUpdateRequest>().ReverseMap();
        CreateMap<RecurringTransactionTemplate, RecurringTransactionTemplateViewModel>()
            .ForMember(dest => dest.AccountName,
                opt => opt.MapFrom(src => src.Account != null ? src.Account.Name : string.Empty))
            .ForMember(dest => dest.AccountType,
                opt => opt.MapFrom(src => src.Account != null ? (AccountType?)src.Account.Type : null))
            .ForMember(dest => dest.ExpectedTransactionCount, opt => opt.MapFrom(src => 0)) // Default value for test
            .ReverseMap()
            .ForMember(dest => dest.Account, opt => opt.Ignore());
    }
}