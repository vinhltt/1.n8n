using CoreFinance.Application.DTOs.ExpectedTransaction;
using CoreFinance.Application.Services;
using CoreFinance.Domain;
using CoreFinance.Domain.BaseRepositories;
using CoreFinance.Domain.Enums;
using CoreFinance.Domain.UnitOfWorks;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using FluentAssertions;

namespace CoreFinance.Application.Tests.ExpectedTransactionServiceTests;

// Tests for the GetPendingTransactionsAsync method of ExpectedTransactionService
public partial class ExpectedTransactionServiceTests
{
    [Fact]
    public async Task GetPendingTransactionsAsync_ShouldReturnOnlyPendingTransactions_ForSpecificUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var expectedTransactions = new List<ExpectedTransaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = "Pending Transaction 1",
                ExpectedAmount = 1000m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = DateTime.UtcNow.AddDays(5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = "Confirmed Transaction",
                ExpectedAmount = 2000m,
                Status = ExpectedTransactionStatus.Confirmed, // Should be ignored
                ExpectedDate = DateTime.UtcNow.AddDays(10)
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = "Pending Transaction 2",
                ExpectedAmount = 1500m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = DateTime.UtcNow.AddDays(15)
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = otherUserId,
                Description = "Other User Pending",
                ExpectedAmount = 3000m,
                Status = ExpectedTransactionStatus.Pending, // Should be ignored (different user)
                ExpectedDate = DateTime.UtcNow.AddDays(20)
            }
        };

        var expectedTransactionsMock = expectedTransactions.AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<ExpectedTransaction, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities())
            .Returns(expectedTransactionsMock);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<ExpectedTransaction, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<ExpectedTransactionService>>();
        var service = new ExpectedTransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetPendingTransactionsAsync(userId);

        // Assert
        var expectedTransactionViewModels = result.ToList();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().HaveCount(2);
        expectedTransactionViewModels.Should().OnlyContain(t => t.UserId == userId);
        expectedTransactionViewModels.Should().OnlyContain(t => t.Status == ExpectedTransactionStatus.Pending);
        expectedTransactionViewModels.Select(t => t.Description).Should().Contain(new List<string>
            { "Pending Transaction 1", "Pending Transaction 2" }.AsReadOnly());
    }

    [Fact]
    public async Task GetPendingTransactionsAsync_ShouldReturnEmptyList_WhenNoPendingTransactionsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var expectedTransactions = new List<ExpectedTransaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = "Confirmed Transaction",
                ExpectedAmount = 1000m,
                Status = ExpectedTransactionStatus.Confirmed,
                ExpectedDate = DateTime.UtcNow.AddDays(5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = "Cancelled Transaction",
                ExpectedAmount = 2000m,
                Status = ExpectedTransactionStatus.Cancelled,
                ExpectedDate = DateTime.UtcNow.AddDays(10)
            }
        };

        var expectedTransactionsMock = expectedTransactions.AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<ExpectedTransaction, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities())
            .Returns(expectedTransactionsMock);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<ExpectedTransaction, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<ExpectedTransactionService>>();
        var service = new ExpectedTransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetPendingTransactionsAsync(userId);

        // Assert
        var expectedTransactionViewModels = result.ToList();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPendingTransactionsAsync_ShouldReturnEmptyList_WhenUserHasNoTransactions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var expectedTransactions = new List<ExpectedTransaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = otherUserId,
                Description = "Other User Transaction",
                ExpectedAmount = 1000m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = DateTime.UtcNow.AddDays(5)
            }
        };

        var expectedTransactionsMock = expectedTransactions.AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<ExpectedTransaction, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities())
            .Returns(expectedTransactionsMock);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<ExpectedTransaction, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<ExpectedTransactionService>>();
        var service = new ExpectedTransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetPendingTransactionsAsync(userId);

        // Assert
        var expectedTransactionViewModels = result as ExpectedTransactionViewModel[] ?? result.ToArray();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPendingTransactionsAsync_ShouldReturnEmptyList_WhenNoTransactionsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedTransactionsMock = new List<ExpectedTransaction>().AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<ExpectedTransaction, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities())
            .Returns(expectedTransactionsMock);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<ExpectedTransaction, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<ExpectedTransactionService>>();
        var service = new ExpectedTransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetPendingTransactionsAsync(userId);

        // Assert
        var expectedTransactionViewModels = result as ExpectedTransactionViewModel[] ?? result.ToArray();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPendingTransactionsAsync_ShouldReturnCorrectTransactionProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var templateId = Guid.NewGuid();

        var expectedTransactions = new List<ExpectedTransaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                AccountId = accountId,
                RecurringTransactionTemplateId = templateId,
                Description = "Monthly Rent",
                ExpectedAmount = 1500m,
                TransactionType = RecurringTransactionType.Expense,
                Category = "Housing",
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = DateTime.UtcNow.AddDays(5),
                GeneratedAt = DateTime.UtcNow.AddDays(-1),
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        var expectedTransactionsMock = expectedTransactions.AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<ExpectedTransaction, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities())
            .Returns(expectedTransactionsMock);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<ExpectedTransaction, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<ExpectedTransactionService>>();
        var service = new ExpectedTransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetPendingTransactionsAsync(userId);

        // Assert
        var expectedTransactionViewModels = result as ExpectedTransactionViewModel[] ?? result.ToArray();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().HaveCount(1);

        var transaction = expectedTransactionViewModels.First();
        transaction.UserId.Should().Be(userId);
        transaction.AccountId.Should().Be(accountId);
        transaction.RecurringTransactionTemplateId.Should().Be(templateId);
        transaction.Description.Should().Be("Monthly Rent");
        transaction.ExpectedAmount.Should().Be(1500m);
        transaction.TransactionType.Should().Be(RecurringTransactionType.Expense);
        transaction.Category.Should().Be("Housing");
        transaction.Status.Should().Be(ExpectedTransactionStatus.Pending);
    }
}