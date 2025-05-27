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

// Tests for the GetTransactionsByAccountAsync method of ExpectedTransactionService
public partial class ExpectedTransactionServiceTests
{
    [Fact]
    public async Task GetTransactionsByAccountAsync_ShouldReturnTransactionsForSpecificAccount()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var otherAccountId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var expectedTransactions = new List<ExpectedTransaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                UserId = userId,
                Description = "Account Transaction 1",
                ExpectedAmount = 1000m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = DateTime.UtcNow.AddDays(5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                UserId = userId,
                Description = "Account Transaction 2",
                ExpectedAmount = 1500m,
                Status = ExpectedTransactionStatus.Confirmed,
                ExpectedDate = DateTime.UtcNow.AddDays(10)
            },
            new()
            {
                Id = Guid.NewGuid(),
                AccountId = otherAccountId, // Different account
                UserId = userId,
                Description = "Other Account Transaction",
                ExpectedAmount = 2000m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = DateTime.UtcNow.AddDays(15)
            },
            new()
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                UserId = userId,
                Description = "Account Transaction 3",
                ExpectedAmount = 2500m,
                Status = ExpectedTransactionStatus.Cancelled,
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
        var result = await service.GetTransactionsByAccountAsync(accountId);

        // Assert
        var expectedTransactionViewModels = result.ToList();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().HaveCount(3);
        expectedTransactionViewModels.Should().OnlyContain(t => t.AccountId == accountId);
        expectedTransactionViewModels.Select(t => t.Description).Should().Contain(new[]
            { "Account Transaction 1", "Account Transaction 2", "Account Transaction 3" });

        // Should be ordered by ExpectedDate
        var resultList = expectedTransactionViewModels.ToList();
        resultList[0].Description.Should().Be("Account Transaction 1");
        resultList[1].Description.Should().Be("Account Transaction 2");
        resultList[2].Description.Should().Be("Account Transaction 3");
    }

    [Fact]
    public async Task GetTransactionsByAccountAsync_ShouldReturnEmptyList_WhenAccountHasNoTransactions()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var otherAccountId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var expectedTransactions = new List<ExpectedTransaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AccountId = otherAccountId, // Different account
                UserId = userId,
                Description = "Other Account Transaction",
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
        var result = await service.GetTransactionsByAccountAsync(accountId);

        // Assert
        var expectedTransactionViewModels = result as ExpectedTransactionViewModel[] ?? result.ToArray();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTransactionsByAccountAsync_ShouldReturnEmptyList_WhenNoTransactionsExist()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var expectedTransactionsMock = new List<ExpectedTransaction>().AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<ExpectedTransaction, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities())
            .Returns(expectedTransactionsMock);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<ExpectedTransaction, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<ExpectedTransactionService>>();
        var service = new ExpectedTransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetTransactionsByAccountAsync(accountId);

        // Assert
        var expectedTransactionViewModels = result.ToList();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTransactionsByAccountAsync_ShouldReturnTransactionsOrderedByExpectedDate()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var expectedTransactions = new List<ExpectedTransaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                UserId = userId,
                Description = "Transaction C",
                ExpectedAmount = 1000m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = DateTime.UtcNow.AddDays(20)
            },
            new()
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                UserId = userId,
                Description = "Transaction A",
                ExpectedAmount = 1500m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = DateTime.UtcNow.AddDays(5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                UserId = userId,
                Description = "Transaction B",
                ExpectedAmount = 2000m,
                Status = ExpectedTransactionStatus.Pending,
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
        var result = await service.GetTransactionsByAccountAsync(accountId);

        // Assert
        var expectedTransactionViewModels = result.ToList();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().HaveCount(3);

        var resultList = expectedTransactionViewModels.ToList();
        resultList[0].Description.Should().Be("Transaction A"); // Earliest date
        resultList[1].Description.Should().Be("Transaction B"); // Middle date
        resultList[2].Description.Should().Be("Transaction C"); // Latest date

        // Verify ordering
        for (int i = 0; i < resultList.Count - 1; i++)
        {
            resultList[i].ExpectedDate.Should().BeOnOrBefore(resultList[i + 1].ExpectedDate);
        }
    }

    [Fact]
    public async Task GetTransactionsByAccountAsync_ShouldReturnAllStatusTypes()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var expectedTransactions = new List<ExpectedTransaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                UserId = userId,
                Description = "Pending Transaction",
                ExpectedAmount = 1000m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = DateTime.UtcNow.AddDays(5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                UserId = userId,
                Description = "Confirmed Transaction",
                ExpectedAmount = 1500m,
                Status = ExpectedTransactionStatus.Confirmed,
                ExpectedDate = DateTime.UtcNow.AddDays(10)
            },
            new()
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                UserId = userId,
                Description = "Cancelled Transaction",
                ExpectedAmount = 2000m,
                Status = ExpectedTransactionStatus.Cancelled,
                ExpectedDate = DateTime.UtcNow.AddDays(15)
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
        var result = await service.GetTransactionsByAccountAsync(accountId);

        // Assert
        var expectedTransactionViewModels = result.ToList();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().HaveCount(3);
        expectedTransactionViewModels.Should().Contain(t => t.Status == ExpectedTransactionStatus.Pending);
        expectedTransactionViewModels.Should().Contain(t => t.Status == ExpectedTransactionStatus.Confirmed);
        expectedTransactionViewModels.Should().Contain(t => t.Status == ExpectedTransactionStatus.Cancelled);
    }

    [Fact]
    public async Task GetTransactionsByAccountAsync_ShouldReturnCorrectTransactionProperties()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var templateId = Guid.NewGuid();

        var expectedTransactions = new List<ExpectedTransaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                UserId = userId,
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
        var result = await service.GetTransactionsByAccountAsync(accountId);

        // Assert
        var expectedTransactionViewModels = result.ToList();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().HaveCount(1);

        var transaction = expectedTransactionViewModels.First();
        transaction.AccountId.Should().Be(accountId);
        transaction.UserId.Should().Be(userId);
        transaction.RecurringTransactionTemplateId.Should().Be(templateId);
        transaction.Description.Should().Be("Monthly Rent");
        transaction.ExpectedAmount.Should().Be(1500m);
        transaction.TransactionType.Should().Be(RecurringTransactionType.Expense);
        transaction.Category.Should().Be("Housing");
        transaction.Status.Should().Be(ExpectedTransactionStatus.Pending);
    }

    [Fact]
    public async Task GetTransactionsByAccountAsync_ShouldHandlePastAndFutureTransactions()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var today = DateTime.UtcNow.Date;

        var expectedTransactions = new List<ExpectedTransaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                UserId = userId,
                Description = "Past Transaction",
                ExpectedAmount = 1000m,
                Status = ExpectedTransactionStatus.Confirmed,
                ExpectedDate = today.AddDays(-10)
            },
            new()
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                UserId = userId,
                Description = "Today Transaction",
                ExpectedAmount = 1500m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = today
            },
            new()
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                UserId = userId,
                Description = "Future Transaction",
                ExpectedAmount = 2000m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = today.AddDays(10)
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
        var result = await service.GetTransactionsByAccountAsync(accountId);

        // Assert
        var expectedTransactionViewModels = result.ToList();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().HaveCount(3);

        var resultList = expectedTransactionViewModels.ToList();
        resultList[0].Description.Should().Be("Past Transaction"); // Earliest date
        resultList[1].Description.Should().Be("Today Transaction"); // Middle date
        resultList[2].Description.Should().Be("Future Transaction"); // Latest date
    }

    [Fact]
    public async Task GetTransactionsByAccountAsync_ShouldHandleMultipleTransactionTypes()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var expectedTransactions = new List<ExpectedTransaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                UserId = userId,
                Description = "Salary Income",
                ExpectedAmount = 5000m,
                TransactionType = RecurringTransactionType.Income,
                Category = "Salary",
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = DateTime.UtcNow.AddDays(5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                UserId = userId,
                Description = "Rent Expense",
                ExpectedAmount = 1500m,
                TransactionType = RecurringTransactionType.Expense,
                Category = "Housing",
                Status = ExpectedTransactionStatus.Pending,
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
        var result = await service.GetTransactionsByAccountAsync(accountId);

        // Assert
        var expectedTransactionViewModels = result.ToList();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().HaveCount(2);
        expectedTransactionViewModels.Should().Contain(t => t.TransactionType == RecurringTransactionType.Income);
        expectedTransactionViewModels.Should().Contain(t => t.TransactionType == RecurringTransactionType.Expense);

        var incomeTransaction =
            expectedTransactionViewModels.First(t => t.TransactionType == RecurringTransactionType.Income);
        incomeTransaction.Description.Should().Be("Salary Income");
        incomeTransaction.ExpectedAmount.Should().Be(5000m);
        incomeTransaction.Category.Should().Be("Salary");

        var expenseTransaction =
            expectedTransactionViewModels.First(t => t.TransactionType == RecurringTransactionType.Expense);
        expenseTransaction.Description.Should().Be("Rent Expense");
        expenseTransaction.ExpectedAmount.Should().Be(1500m);
        expenseTransaction.Category.Should().Be("Housing");
    }
}