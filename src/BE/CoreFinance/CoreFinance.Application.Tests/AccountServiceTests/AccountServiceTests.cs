using AutoMapper;
using CoreFinance.Application.Mapper;

namespace CoreFinance.Application.Tests.AccountServiceTests
{
    public partial class AccountServiceTests
    {
        private readonly IMapper _mapper;

        public AccountServiceTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });
            _mapper = config.CreateMapper();
        }

    }
}
