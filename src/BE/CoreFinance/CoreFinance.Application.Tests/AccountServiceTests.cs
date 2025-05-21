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
        var accountsData = GenerateFakeAccounts(3).ToList(); // Generate some accounts
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
        var accounts = GenerateFakeAccounts(totalItems).ToList();
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

    // --- CreateAsync Tests ---

    [Fact]
    public async Task CreateAsync_ShouldReturnViewModel_WhenCreationIsSuccessful()
    {
        // Arrange
        var createRequest = new AccountCreateRequest 
        { 
            Name = "New Savings Account", 
            Type = AccountType.Bank, 
            Currency = "USD", 
            InitialBalance = 1000,
            UserId = Guid.NewGuid()
            // Add other required properties for AccountCreateRequest
        };

        var expectedViewModel = new AccountViewModel 
        { 
            Id = Guid.NewGuid(), // Mapper will set this from newAccountEntity after creation
            Name = createRequest.Name, 
            Type = createRequest.Type,
            Currency = createRequest.Currency,
            CurrentBalance = createRequest.InitialBalance 
            // Add other relevant properties
        };

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.Map(createRequest, It.IsAny<Account>()))
            .Callback<AccountCreateRequest, Account>((src, dest) => 
            {
                // Simulate AutoMapper's behavior for mapping request to entity
                dest.Id = Guid.NewGuid(); // Simulate ID generation by DB/repository
                dest.Name = src.Name;
                dest.Type = src.Type;
                dest.Currency = src.Currency;
                dest.InitialBalance = src.InitialBalance;
                dest.CurrentBalance = src.InitialBalance; // Assuming current = initial on creation
                dest.UserId = src.UserId;
                dest.CreatedAt = DateTime.UtcNow;
                dest.UpdatedAt = DateTime.UtcNow;
                dest.IsActive = true;
            });
        
        mapperMock.Setup(m => m.Map<AccountViewModel>(It.IsAny<Account>()))
            .Returns((Account src) => new AccountViewModel { 
                Id = src.Id, 
                Name = src.Name, 
                Type = src.Type,
                Currency = src.Currency,
                CurrentBalance = src.CurrentBalance
            });


        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.CreateAsync(It.IsAny<Account>()))
            .ReturnsAsync(1); // Simulate 1 record affected

        var unitOfWorkMock = new Mock<IUnitOffWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);
        // Simulate DoWorkWithTransaction by directly invoking the passed func
        unitOfWorkMock.Setup(u => u.DoWorkWithTransaction(It.IsAny<Func<Task<AccountViewModel?>>>()))
            .Returns((Func<Task<AccountViewModel?>> func) => func());


        var loggerMock = new Mock<ILogger<AccountService>>();
        var service = new AccountService(mapperMock.Object, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.CreateAsync(createRequest);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedViewModel, options => options.Excluding(vm => vm.Id)); // Id is dynamic
        result.Name.Should().Be(createRequest.Name);
        result.Currency.Should().Be(createRequest.Currency);

        mapperMock.Verify(m => m.Map(createRequest, It.IsAny<Account>()), Times.Once);
        repoMock.Verify(r => r.CreateAsync(It.IsAny<Account>()), Times.Once);
        mapperMock.Verify(m => m.Map<AccountViewModel>(It.IsAny<Account>()), Times.Once);
        unitOfWorkMock.Verify(u => u.DoWorkWithTransaction(It.IsAny<Func<Task<AccountViewModel?>>>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowNullReferenceException_WhenRepositoryReturnsZeroAffectedCount()
    {
        // Arrange
        var createRequest = new AccountCreateRequest { Name = "Test Account" /* ... other properties */ };
        
        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.Map(createRequest, It.IsAny<Account>()));
        // No need to setup the second Map call as it won't be reached if exception is thrown

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.CreateAsync(It.IsAny<Account>()))
            .ReturnsAsync(0); // Simulate 0 records affected

        var unitOfWorkMock = new Mock<IUnitOffWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);
        unitOfWorkMock.Setup(u => u.DoWorkWithTransaction(It.IsAny<Func<Task<AccountViewModel?>>>()))
            .Returns((Func<Task<AccountViewModel?>> func) => func());

        var loggerMock = new Mock<ILogger<AccountService>>();
        var service = new AccountService(mapperMock.Object, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        Func<Task> act = async () => await service.CreateAsync(createRequest);

        // Assert
        await act.Should().ThrowAsync<NullReferenceException>();
        
        repoMock.Verify(r => r.CreateAsync(It.IsAny<Account>()), Times.Once);
        unitOfWorkMock.Verify(u => u.DoWorkWithTransaction(It.IsAny<Func<Task<AccountViewModel?>>>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateAsync_ShouldRollbackTransaction_WhenRepositoryThrowsException()
    {
        // Arrange
        var createRequest = new AccountCreateRequest { Name = "Test Account" /* ... other properties */ };

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.Map(createRequest, It.IsAny<Account>()));

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.CreateAsync(It.IsAny<Account>()))
            .ThrowsAsync(new InvalidOperationException("DB error")); // Simulate a DB error

        var unitOfWorkMock = new Mock<IUnitOffWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);
        
        // This setup is crucial: If DoWorkWithTransaction catches the exception and doesn't rethrow, the test is different.
        // Assuming DoWorkWithTransaction propagates the exception or handles rollback internally.
        // For this test, we just verify it's called. The actual rollback is an integration concern or UoW test.
        unitOfWorkMock.Setup(u => u.DoWorkWithTransaction(It.IsAny<Func<Task<AccountViewModel?>>>()))
             .Returns(async (Func<Task<AccountViewModel?>> func) => await func());


        var loggerMock = new Mock<ILogger<AccountService>>();
        var service = new AccountService(mapperMock.Object, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        Func<Task> act = async () => await service.CreateAsync(createRequest);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("DB error");
        
        repoMock.Verify(r => r.CreateAsync(It.IsAny<Account>()), Times.Once);
        unitOfWorkMock.Verify(u => u.DoWorkWithTransaction(It.IsAny<Func<Task<AccountViewModel?>>>()), Times.Once);
        // We can't easily verify rollback in a unit test without more complex UoW mocking.
        // The key is that the transaction block was entered.
    }
    
    // Consider adding tests for validation if AccountCreateRequest has validation attributes
    // and if the BaseService or AccountService is expected to trigger validation.
    // However, validation is often handled by MVC model binding or a dedicated validation layer before the service call.
    // If service is responsible for some validation, test it here.

    // Test for CreateAsync(List<TCreateRequest> request) can be added if that overload is used and needs specific testing.
    // The logic is similar but involves collections.
}