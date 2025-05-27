using CoreFinance.Application.DTOs.Transaction;
using CoreFinance.Application.Services;
using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Contracts.Enums;
using CoreFinance.Domain;
using CoreFinance.Domain.BaseRepositories;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using FluentAssertions;
using CoreFinance.Application.Tests.Helpers;
using CoreFinance.Domain.UnitOfWorks;

namespace CoreFinance.Application.Tests.TransactionServiceTests;

// Tests for the GetPagingAsync method of TransactionService
public partial class TransactionServiceTests
{
    [Fact]
    public async Task GetPagingAsync_ShouldReturnPagedResult()
    {
        // Arrange
        var transactions = TestHelpers.GenerateFakeTransactions(3).ToList();
        var pageSize = 2;
        var pageIndex = 1;
        var orderedTransactions = transactions.OrderBy(t => t.TransactionDate).ToList();

        var transactionsMock = transactions.AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities())
            .Returns(transactionsMock);
        repoMock.Setup(x => x.GetQueryableTable()).Returns(transactionsMock);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<TransactionService>>();

        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        var request = new FilterBodyRequest
        {
            SearchValue = "", // Empty search value
            Pagination = new Pagination { PageIndex = pageIndex, PageSize = pageSize },
            Orders = new List<SortDescriptor> { new() { Field = "TransactionDate", Direction = SortDirection.Asc } }
        };

        // Act
        var result = await service.GetPagingAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Count().Should().Be(pageSize); // PageSize = 2

        var expectedViewModels = orderedTransactions.Skip((pageIndex - 1)! * pageSize).Take(pageSize)
            .Select(t => _mapper.Map<TransactionViewModel>(t)).ToList();

        result.Data.Should().BeEquivalentTo(expectedViewModels, options => options.WithStrictOrdering());

        result.Pagination.TotalRow.Should().Be(transactions.Count); // Total records
        result.Pagination.PageSize.Should().Be(pageSize);
        result.Pagination.PageIndex.Should().Be(pageIndex);
    }

    [Fact]
    public async Task GetPagingAsync_ShouldFilterByDescription()
    {
        // Arrange
        var transactions = TestHelpers.GenerateFakeTransactions(3).ToList();
        var expectedDescription = transactions[0].Description;
        var expectedTransaction = transactions.First(t => t.Description == expectedDescription);

        var transactionsMock = transactions.AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities())
            .Returns(transactionsMock);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<TransactionService>>();

        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        var request = new FilterBodyRequest
        {
            SearchValue = expectedDescription // Use the exact description
        };

        // Act
        var result = await service.GetPagingAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Should().ContainSingle();
        var expectedViewModel = _mapper.Map<TransactionViewModel>(expectedTransaction);
        result.Data.First().Should().BeEquivalentTo(expectedViewModel);
    }

    [Fact]
    public async Task GetPagingAsync_ShouldFilterByCategorySummary()
    {
        // Arrange
        var transactions = TestHelpers.GenerateFakeTransactions(3).ToList();
        var expectedCategorySummary = transactions[0].CategorySummary;
        var expectedTransaction = transactions.First(t => t.CategorySummary == expectedCategorySummary);

        var transactionsMock = transactions.AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities())
            .Returns(transactionsMock);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<TransactionService>>();

        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        var request = new FilterBodyRequest
        {
            SearchValue = expectedCategorySummary // Use the exact category summary
        };

        // Act
        var result = await service.GetPagingAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Should().ContainSingle();
        var expectedViewModel = _mapper.Map<TransactionViewModel>(expectedTransaction);
        result.Data.First().Should().BeEquivalentTo(expectedViewModel);
    }

    [Fact]
    public async Task GetPagingAsync_ShouldFilterBySearchValue_CaseInsensitive()
    {
        // Arrange
        var transactions = new List<Transaction>
        {
            new() { Description = "Test Transaction One", CategorySummary = "Food" },
            new() { Description = "test transaction two", CategorySummary = "Entertainment" },
            new() { Description = "Another Transaction", CategorySummary = "TEST Category" }
        }.AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities())
            .Returns(transactions);
        repoMock.Setup(x => x.GetQueryableTable()).Returns(transactions);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<TransactionService>>();
        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        var request = new FilterBodyRequest
        {
            SearchValue = "test"
        };

        // Act
        var result = await service.GetPagingAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Count().Should().Be(3); // All 3 transactions contain "test" (case insensitive)

        var expectedViewModels = transactions.Where(t =>
            (t.Description != null && t.Description.ToLower().Contains("test")) ||
            (t.CategorySummary != null && t.CategorySummary.ToLower().Contains("test"))
        ).ToList().Select(_mapper.Map<TransactionViewModel>).ToList();

        result.Data.Should().BeEquivalentTo(expectedViewModels);
    }

    [Fact]
    public async Task GetPagingAsync_ShouldReturnEmpty_WhenSearchValueHasNoMatch()
    {
        // Arrange
        var transactionsData = TestHelpers.GenerateFakeTransactions(3).ToList();
        var transactionsMock = transactionsData.AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities())
            .Returns(transactionsMock);
        repoMock.Setup(x => x.GetQueryableTable()).Returns(transactionsMock);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<TransactionService>>();
        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        var request = new FilterBodyRequest
        {
            SearchValue = "NonExistentSearchTermWhichWillNotMatchAnyFakeTransactionDescription"
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
        var emptyTransactions = new List<Transaction>().AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities())
            .Returns(emptyTransactions);
        repoMock.Setup(x => x.GetQueryableTable()).Returns(emptyTransactions);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<TransactionService>>();
        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

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
    public async Task GetPagingAsync_ShouldHandlePaginationEdgeCases(int pageIndex, int pageSize, int totalItems,
        int expectedPageIndex, int expectedDataCount)
    {
        // Arrange
        var transactions = TestHelpers.GenerateFakeTransactions(totalItems).ToList();
        var orderedTransactions = transactions.OrderBy(t => t.TransactionDate).ToList();

        var transactionsMock = transactions.AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities())
            .Returns(transactionsMock);
        repoMock.Setup(x => x.GetQueryableTable()).Returns(transactionsMock);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<TransactionService>>();
        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        var request = new FilterBodyRequest
        {
            SearchValue = "",
            Pagination = new Pagination { PageIndex = pageIndex, PageSize = pageSize },
            Orders = new List<SortDescriptor> { new() { Field = "TransactionDate", Direction = SortDirection.Asc } }
        };

        // Act
        var result = await service.GetPagingAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Count().Should().Be(expectedDataCount);
        result.Pagination.PageIndex.Should().Be(expectedPageIndex);
        result.Pagination.TotalRow.Should().Be(totalItems);

        var expectedViewModels = orderedTransactions.Skip((pageIndex - 1) * pageSize).Take(pageSize)
            .Select(t => _mapper.Map<TransactionViewModel>(t)).ToList();

        result.Data.Should().BeEquivalentTo(expectedViewModels, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetPagingAsync_ShouldHandleNullSearchValue()
    {
        // Arrange
        var transactions = TestHelpers.GenerateFakeTransactions(2).ToList();
        var transactionsMock = transactions.AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities())
            .Returns(transactionsMock);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<TransactionService>>();
        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        var request = new FilterBodyRequest
        {
            SearchValue = null, // Null search value
            Pagination = new Pagination { PageIndex = 1, PageSize = 10 }
        };

        // Act
        var result = await service.GetPagingAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Count().Should().Be(2); // Should return all transactions when search is null
        result.Pagination.TotalRow.Should().Be(2);
    }

    [Fact]
    public async Task GetPagingAsync_ShouldHandleEmptyStringSearchValue()
    {
        // Arrange
        var transactions = TestHelpers.GenerateFakeTransactions(2).ToList();
        var transactionsMock = transactions.AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities())
            .Returns(transactionsMock);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<TransactionService>>();
        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        var request = new FilterBodyRequest
        {
            SearchValue = "", // Empty string search value
            Pagination = new Pagination { PageIndex = 1, PageSize = 10 }
        };

        // Act
        var result = await service.GetPagingAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Count().Should().Be(2); // Should return all transactions when search is empty
        result.Pagination.TotalRow.Should().Be(2);
    }
}