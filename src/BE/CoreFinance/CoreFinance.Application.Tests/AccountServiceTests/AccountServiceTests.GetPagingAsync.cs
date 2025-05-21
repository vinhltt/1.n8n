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
using FluentAssertions;
using CoreFinance.Application.Tests.Helpers;

namespace CoreFinance.Application.Tests.AccountServiceTests;

// Tests for the GetPagingAsync method of AccountService
public partial class AccountServiceTests
{
    // Helper method has been moved to TestHelpers.cs
    // private static IQueryable<Account> GenerateFakeAccounts(int count) { ... }

    [Fact]
    public async Task GetPagingAsync_ShouldReturnPagedResult()
    {
        // Arrange
        var accounts = TestHelpers.GenerateFakeAccounts(3).ToList();
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
        var accounts = TestHelpers.GenerateFakeAccounts(3).ToList();
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

    [Fact]
    public async Task GetPagingAsync_ShouldFilterBySearchValue_CaseInsensitive()
    {
        // Arrange
        var accounts = new List<Account>
        {
            new Account { Name = "Test Account One" },
            new Account { Name = "test account two" },
            new Account { Name = "Another Account" }
        }.AsQueryable().BuildMock();

        var accountViewModels = new List<AccountViewModel>
        {
            new AccountViewModel { Name = "Test Account One" },
            new AccountViewModel { Name = "test account two" },
            new AccountViewModel { Name = "Another Account" }
        }.AsQueryable().BuildMock();
        
        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities(It.IsAny<Expression<Func<Account, object>>>()))
            .Returns(accounts);
        repoMock.Setup(x => x.GetQueryableTable()).Returns(accounts);

        var unitOfWorkMock = new Mock<IUnitOffWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.ProjectTo<AccountViewModel>(It.IsAny<IQueryable<Account>>(), It.IsAny<object?>()))
            .Returns(accountViewModels);
        
        var loggerMock = new Mock<ILogger<AccountService>>();
        var service = new AccountService(mapperMock.Object, unitOfWorkMock.Object, loggerMock.Object);

        var request = new FilterBodyRequest
        {
            SearchValue = "test account" 
        };

        // Act
        var result = await service.GetPagingAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Count().Should().Be(2); // "Test Account One" and "test account two"
        result.Data.Should().Contain(vm => vm.Name == "Test Account One");
        result.Data.Should().Contain(vm => vm.Name == "test account two");
    }

    [Fact]
    public async Task GetPagingAsync_ShouldReturnEmpty_WhenSearchValueHasNoMatch()
    {
        // Arrange
        var accountsData = TestHelpers.GenerateFakeAccounts(3).ToList(); // Generate some accounts
        var accountsMock = accountsData.AsQueryable().BuildMock();

        // These are the AccountViewModels that ProjectTo should return *before* the service's Where clause
        var projectedViewModels = accountsData.Select(a => new AccountViewModel { 
            Id = a.Id, 
            Name = a.Name, 
            Currency = a.Currency 
            // Map other fields if they are used by the service's Where clause or subsequent logic
        }).AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities(It.IsAny<Expression<Func<Account, object>>>()))
            .Returns(accountsMock);
        repoMock.Setup(x => x.GetQueryableTable()).Returns(accountsMock);

        var unitOfWorkMock = new Mock<IUnitOffWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);

        var mapperMock = new Mock<IMapper>();
        // Correctly setup ProjectTo to match the 3-parameter overload used by the service
        // Mapper.ProjectTo<T>(source) resolves to ProjectTo(source, null, new Expression<...>[0])
        mapperMock.Setup(m => m.ProjectTo(
                        It.IsAny<IQueryable<Account>>(),  // source
                        null,                             // parameters argument in ProjectTo
                        It.IsAny<Expression<Func<AccountViewModel, object>>[]>() // membersToExpand argument
                    ))
            .Returns(projectedViewModels); // Return the unfiltered projected view models

        var loggerMock = new Mock<ILogger<AccountService>>();
        var service = new AccountService(mapperMock.Object, unitOfWorkMock.Object, loggerMock.Object);

        var request = new FilterBodyRequest
        {
            SearchValue = "NonExistentSearchTermWhichWillNotMatchAnyFakeAccountName"
        };

        // Act
        var result = await service.GetPagingAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty(); // The service's Where clause should result in an empty list
        result.Pagination.TotalRow.Should().Be(0);
    }

    [Fact]
    public async Task GetPagingAsync_ShouldHandleRepositoryReturningNoData()
    {
        // Arrange
        var emptyAccounts = new List<Account>().AsQueryable().BuildMock();
        var emptyAccountViewModels = new List<AccountViewModel>().AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities(It.IsAny<Expression<Func<Account, object>>>()))
            .Returns(emptyAccounts);
        repoMock.Setup(x => x.GetQueryableTable()).Returns(emptyAccounts);

        var unitOfWorkMock = new Mock<IUnitOffWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.ProjectTo<AccountViewModel>(It.IsAny<IQueryable<Account>>(), It.IsAny<object?>()))
            .Returns(emptyAccountViewModels); // Mapper projects an empty list to an empty list

        var loggerMock = new Mock<ILogger<AccountService>>();
        var service = new AccountService(mapperMock.Object, unitOfWorkMock.Object, loggerMock.Object);

        var request = new FilterBodyRequest
        {
            Pagination = new Pagination { PageIndex = 1, PageSize = 10 }
        };

        // Act
        var result = await service.GetPagingAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.Pagination.TotalRow.Should().Be(0);
        result.Pagination.PageIndex.Should().Be(1);
        result.Pagination.PageSize.Should().Be(10);
    }

    [Theory]
    [InlineData(1, 5, 3, 1, 3)] // Total items < PageSize
    [InlineData(2, 2, 5, 2, 2)] // Requesting page that has fewer items than PageSize
    public async Task GetPagingAsync_ShouldHandlePaginationEdgeCases(int pageIndex, int pageSize, int totalItems, int expectedPageIndex, int expectedDataCount)
    {
        // Arrange
        var accounts = TestHelpers.GenerateFakeAccounts(totalItems).ToList();
        var orderedAccounts = accounts.OrderBy(a => a.Name).ToList();
        var expectedPageData = orderedAccounts.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

        var accountsMock = accounts.AsQueryable().BuildMock();
        var accountViewModels = accounts.Select(a => new AccountViewModel { Id = a.Id, Name = a.Name, Currency = a.Currency })
            .AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities(It.IsAny<Expression<Func<Account, object>>>()))
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
            Pagination = new Pagination { PageIndex = pageIndex, PageSize = pageSize },
            Orders = new List<SortDescriptor> { new() { Field = "Name", Direction = SortDirection.Asc } }
        };

        // Act
        var result = await service.GetPagingAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Count().Should().Be(expectedDataCount);
        result.Pagination.TotalRow.Should().Be(totalItems);
        result.Pagination.PageIndex.Should().Be(expectedPageIndex); // Assuming ToPagingAsync caps PageIndex or handles it appropriately
        result.Pagination.PageSize.Should().Be(pageSize);

        // Verify that the correct items are returned for the page
        // This part of the assertion depends on how ToPagingAsync behaves with IQueryable mocks and sorting.
        // If ToPagingAsync does client-side evaluation due to mocking, the order might not be guaranteed
        // without careful setup of the mock provider for ToPagingAsync or testing ToPagingAsync itself.
        // For this test, we assume ToPagingAsync correctly applies pagination after sorting.
        if (expectedDataCount > 0)
        {
            var resultNames = result.Data.Select(d => d.Name).ToList();
            var expectedNames = expectedPageData.Select(e => e.Name).ToList();
            // The order might not be strictly guaranteed here with BuildMock() and ProjectTo if ToPagingAsync relies on database ordering that's not perfectly mimicked.
            // A more robust check might be to ensure all expected items are present, regardless of order, if strict order is hard to mock.
            resultNames.Should().BeEquivalentTo(expectedNames, options => options.WithStrictOrdering());
        }
    }
} 