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
using FluentAssertions;
// ReSharper disable RedundantArgumentDefaultValue

namespace CoreFinance.Application.Tests;

public class AccountServiceTests
{
    private static IQueryable<Account> GenerateFakeAccounts(int count)
    {
        var faker = new Faker<Account>()
            .RuleFor(a => a.Id, _ => Guid.NewGuid())
            .RuleFor(a => a.UserId, _ => Guid.NewGuid())
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
        var accounts = GenerateFakeAccounts(3).ToList();
        var pageSize = 2;
        var pageIndex = 1;
        var orderedAccounts = accounts.OrderBy(a => a.Name).ToList();
        // ReSharper disable once UselessBinaryOperation
        var expectedNames = orderedAccounts.Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(a => a.Name)
            .ToList();

        var accountsMock = accounts.AsQueryable().BuildMock();

        var accountViewModels = accounts.Select(a => new AccountViewModel
        {
            Id = a.Id,
            Name = a.Name,
            Currency = a.Currency,
        }).AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities(It.IsAny<Expression<Func<Account, object>>>() ))
            .Returns(accountsMock);
        repoMock.Setup(x => x.GetQueryableTable()).Returns(accountsMock);

        var unitOfWorkMock = new Mock<IUnitOffWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.ProjectTo<AccountViewModel>(It.IsAny<IQueryable<Account>>(), It.IsAny<object?>()))
            .Returns(accountViewModels);

        var loggerMock = new Mock<ILogger<AccountService>>();

        var service = new AccountService(mapperMock.Object, unitOfWorkMock.Object, loggerMock.Object);

        var request = new FilterBodyRequest
        {
            SearchValue = "", // Có thể để trống hoặc lấy ký tự chung, vì không filter cụ thể
            Pagination = new Pagination { PageIndex = pageIndex, PageSize = pageSize },
            Orders = new List<SortDescriptor> { new() { Field = "Name", Direction = SortDirection.Asc } }
        };

        // Act
        var result = await service.GetPagingAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Count().Should().Be(pageSize); // PageSize = 2
        result.Data.Select(x => x.Name).Should().Contain(expectedNames);
        result.Pagination.TotalRow.Should().Be(accounts.Count); // Tổng số bản ghi
        result.Pagination.PageSize.Should().Be(pageSize);
        result.Pagination.PageIndex.Should().Be(pageIndex);
    }

    [Fact]
    public async Task GetPagingAsync_ShouldFilterBySearchValue()
    {
        // Arrange
        var accounts = GenerateFakeAccounts(3).ToList();
        var expectedName = accounts[0].Name; // Lấy tên bất kỳ từ dữ liệu fake

        var accountsMock = accounts.AsQueryable().BuildMock();

        var accountViewModels = accounts.Select(a => new AccountViewModel
        {
            Id = a.Id,
            Name = a.Name,
            Currency = a.Currency,
        }).AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities(It.IsAny<Expression<Func<Account, object>>>() ))
            .Returns(accountsMock);

        var unitOfWorkMock = new Mock<IUnitOffWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.ProjectTo<AccountViewModel>(It.IsAny<IQueryable<Account>>(), It.IsAny<object?>()))
            .Returns(accountViewModels);
        var loggerMock = new Mock<ILogger<AccountService>>();

        var service = new AccountService(mapperMock.Object, unitOfWorkMock.Object, loggerMock.Object);

        var request = new FilterBodyRequest
        {
            SearchValue = expectedName // Dùng đúng tên vừa lấy
        };

        // Act
        var result = await service.GetPagingAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Should().ContainSingle();
        result.Data.First().Name.Should().Be(expectedName);
    }
}