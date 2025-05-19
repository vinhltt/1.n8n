using System.Linq.Expressions;
using AutoMapper;
using CoreFinance.Application.DTOs;
using CoreFinance.Application.Services;
using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Contracts.Enums;
using CoreFinance.Domain;
using CoreFinance.Domain.BaseRepositories;
using CoreFinance.Domain.UnitOffWorks;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using Bogus;

namespace CoreFinance.Application.Tests;

public class AccountServiceTests
{
    private static IQueryable<Account> GenerateFakeAccounts(int count)
    {
        var faker = new Faker<Account>()
            .RuleFor(a => a.Id, f => Guid.NewGuid())
            .RuleFor(a => a.UserId, f => Guid.NewGuid())
            .RuleFor(a => a.Name, f => f.Company.CompanyName())
            .RuleFor(a => a.Type, f => f.PickRandom<AccountType>())
            .RuleFor(a => a.CardNumber, f => f.Finance.CreditCardNumber())
            .RuleFor(a => a.Currency, f => f.Finance.Currency().Code)
            .RuleFor(a => a.InitialBalance, f => f.Finance.Amount(0, 10000))
            .RuleFor(a => a.CurrentBalance, (f, a) => a.InitialBalance + f.Finance.Amount(-1000, 1000))
            .RuleFor(a => a.AvailableLimit, f => f.Random.Bool() ? f.Finance.Amount(0, 5000) : null)
            .RuleFor(a => a.CreatedAt, f => f.Date.Past(2))
            .RuleFor(a => a.UpdatedAt, f => f.Date.Recent())
            .RuleFor(a => a.IsActive, f => f.Random.Bool());
        return faker.Generate(count).AsQueryable().BuildMock();
    }

    [Fact]
    public async Task GetPagingAsync_ShouldReturnPagedResult()
    {
        // Arrange
        var accounts = GenerateFakeAccounts(3);
        var accountViewModels = accounts.Select(a => new AccountViewModel
        {
            Id = a.Id,
            Name = a.Name,
            Currency = a.Currency,
        }).AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities(It.IsAny<Expression<Func<Account, object>>>()))
            .Returns(accounts);
        repoMock.Setup(x => x.GetQueryableTable()).Returns(accounts);

        var unitOfWorkMock = new Mock<IUnitOffWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);

        var mapperMock = new Mock<IMapper>();
        // ProjectTo trả về IQueryable<AccountViewModel> hỗ trợ async
        mapperMock.Setup(m => m.ProjectTo<AccountViewModel>(It.IsAny<IQueryable<Account>>(), It.IsAny<object?>()))
            .Returns(accountViewModels);

        var loggerMock = new Mock<ILogger<AccountService>>();

        var service = new AccountService(mapperMock.Object, unitOfWorkMock.Object, loggerMock.Object);

        var request = new FilterBodyRequest
        {
            SearchValue = "Account",
            Pagination = new Pagination { PageIndex = 1, PageSize = 2 },
            Orders = new List<SortDescriptor> { new() { Field = "Name", Direction = SortDirection.Asc } }
        };

        // Act
        var result = await service.GetPagingAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count()); // PageSize = 2
        Assert.Contains(result.Data, x => x.Name == "Account 1");
        Assert.Contains(result.Data, x => x.Name == "Account 2");
        Assert.Equal(3, result.Pagination.TotalRow); // Tổng số bản ghi
        Assert.Equal(2, result.Pagination.PageSize);
        Assert.Equal(1, result.Pagination.PageIndex);
    }

    [Fact]
    public async Task GetPagingAsync_ShouldFilterBySearchValue()
    {
        // Arrange
        var accounts = GenerateFakeAccounts(3);
        var accountViewModels = accounts.Select(a => new AccountViewModel
        {
            Id = a.Id,
            Name = a.Name,
            Currency = a.Currency,
        }).AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities(It.IsAny<Expression<Func<Account, object>>>()))
            .Returns(accounts);

        var unitOfWorkMock = new Mock<IUnitOffWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.ProjectTo<AccountViewModel>(It.IsAny<IQueryable<Account>>(), It.IsAny<object?>()))
            .Returns(accountViewModels);
        var loggerMock = new Mock<ILogger<AccountService>>();

        var service = new AccountService(mapperMock.Object, unitOfWorkMock.Object, loggerMock.Object);

        var request = new FilterBodyRequest
        {
            SearchValue = "Account 1"
        };

        // Act
        var result = await service.GetPagingAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        Assert.Equal("Account 1", result.Data.First().Name);
    }
}