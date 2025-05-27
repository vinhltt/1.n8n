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

// Tests for the GetTransactionsByTemplateAsync method of ExpectedTransactionService
public partial class ExpectedTransactionServiceTests
{
    [Fact]
    public async Task GetTransactionsByTemplateAsync_ShouldReturnTransactionsForSpecificTemplate()
    {
        // Arrange
        var templateId = Guid.NewGuid();
        var otherTemplateId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var expectedTransactions = new List<ExpectedTransaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                RecurringTransactionTemplateId = templateId,
                UserId = userId,
                Description = "Template Transaction 1",
                ExpectedAmount = 1000m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = DateTime.UtcNow.AddDays(5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                RecurringTransactionTemplateId = templateId,
                UserId = userId,
                Description = "Template Transaction 2",
                ExpectedAmount = 1500m,
                Status = ExpectedTransactionStatus.Confirmed,
                ExpectedDate = DateTime.UtcNow.AddDays(10)
            },
            new()
            {
                Id = Guid.NewGuid(),
                RecurringTransactionTemplateId = otherTemplateId, // Different template
                UserId = userId,
                Description = "Other Template Transaction",
                ExpectedAmount = 2000m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = DateTime.UtcNow.AddDays(15)
            },
            new()
            {
                Id = Guid.NewGuid(),
                RecurringTransactionTemplateId = templateId,
                UserId = userId,
                Description = "Template Transaction 3",
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
        var result = await service.GetTransactionsByTemplateAsync(templateId);

        // Assert
        var expectedTransactionViewModels = result.ToList();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().HaveCount(3);
        expectedTransactionViewModels.Should().OnlyContain(t => t.RecurringTransactionTemplateId == templateId);
        expectedTransactionViewModels.Select(t => t.Description).Should().Contain(new[]
            { "Template Transaction 1", "Template Transaction 2", "Template Transaction 3" });

        // Should be ordered by ExpectedDate
        var resultList = expectedTransactionViewModels.ToList();
        resultList[0].Description.Should().Be("Template Transaction 1");
        resultList[1].Description.Should().Be("Template Transaction 2");
        resultList[2].Description.Should().Be("Template Transaction 3");
    }

    [Fact]
    public async Task GetTransactionsByTemplateAsync_ShouldReturnEmptyList_WhenTemplateHasNoTransactions()
    {
        // Arrange
        var templateId = Guid.NewGuid();
        var otherTemplateId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var expectedTransactions = new List<ExpectedTransaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                RecurringTransactionTemplateId = otherTemplateId, // Different template
                UserId = userId,
                Description = "Other Template Transaction",
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
        var result = await service.GetTransactionsByTemplateAsync(templateId);

        // Assert
        var expectedTransactionViewModels = result.ToList();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTransactionsByTemplateAsync_ShouldReturnEmptyList_WhenNoTransactionsExist()
    {
        // Arrange
        var templateId = Guid.NewGuid();
        var expectedTransactionsMock = new List<ExpectedTransaction>().AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<ExpectedTransaction, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities())
            .Returns(expectedTransactionsMock);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<ExpectedTransaction, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<ExpectedTransactionService>>();
        var service = new ExpectedTransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetTransactionsByTemplateAsync(templateId);

        // Assert
        var expectedTransactionViewModels = result.ToList();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTransactionsByTemplateAsync_ShouldReturnTransactionsOrderedByExpectedDate()
    {
        // Arrange
        var templateId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var expectedTransactions = new List<ExpectedTransaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                RecurringTransactionTemplateId = templateId,
                UserId = userId,
                Description = "Transaction C",
                ExpectedAmount = 1000m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = DateTime.UtcNow.AddDays(20)
            },
            new()
            {
                Id = Guid.NewGuid(),
                RecurringTransactionTemplateId = templateId,
                UserId = userId,
                Description = "Transaction A",
                ExpectedAmount = 1500m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = DateTime.UtcNow.AddDays(5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                RecurringTransactionTemplateId = templateId,
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
        var result = await service.GetTransactionsByTemplateAsync(templateId);

        // Assert
        var resultList = result.ToList();
        resultList.Should().NotBeNull();
        resultList.Should().HaveCount(3);

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
    public async Task GetTransactionsByTemplateAsync_ShouldReturnAllStatusTypes()
    {
        // Arrange
        var templateId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var expectedTransactions = new List<ExpectedTransaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                RecurringTransactionTemplateId = templateId,
                UserId = userId,
                Description = "Pending Transaction",
                ExpectedAmount = 1000m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = DateTime.UtcNow.AddDays(5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                RecurringTransactionTemplateId = templateId,
                UserId = userId,
                Description = "Confirmed Transaction",
                ExpectedAmount = 1500m,
                Status = ExpectedTransactionStatus.Confirmed,
                ExpectedDate = DateTime.UtcNow.AddDays(10)
            },
            new()
            {
                Id = Guid.NewGuid(),
                RecurringTransactionTemplateId = templateId,
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
        var result = await service.GetTransactionsByTemplateAsync(templateId);

        // Assert
        var expectedTransactionViewModels = result.ToList();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().HaveCount(3);
        expectedTransactionViewModels.Should().Contain(t => t.Status == ExpectedTransactionStatus.Pending);
        expectedTransactionViewModels.Should().Contain(t => t.Status == ExpectedTransactionStatus.Confirmed);
        expectedTransactionViewModels.Should().Contain(t => t.Status == ExpectedTransactionStatus.Cancelled);
    }

    [Fact]
    public async Task GetTransactionsByTemplateAsync_ShouldReturnCorrectTransactionProperties()
    {
        // Arrange
        var templateId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        var expectedTransactions = new List<ExpectedTransaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                RecurringTransactionTemplateId = templateId,
                UserId = userId,
                AccountId = accountId,
                Description = "Monthly Salary",
                ExpectedAmount = 5000m,
                TransactionType = RecurringTransactionType.Income,
                Category = "Salary",
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
        var result = await service.GetTransactionsByTemplateAsync(templateId);

        // Assert
        var expectedTransactionViewModels = result.ToList();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().HaveCount(1);
        var transaction = expectedTransactionViewModels.First();

        transaction.RecurringTransactionTemplateId.Should().Be(templateId);
        transaction.UserId.Should().Be(userId);
        transaction.AccountId.Should().Be(accountId);
        transaction.Description.Should().Be("Monthly Salary");
        transaction.ExpectedAmount.Should().Be(5000m);
        transaction.TransactionType.Should().Be(RecurringTransactionType.Income);
        transaction.Category.Should().Be("Salary");
        transaction.Status.Should().Be(ExpectedTransactionStatus.Pending);
    }

    [Fact]
    public async Task GetTransactionsByTemplateAsync_ShouldHandlePastAndFutureTransactions()
    {
        // Arrange
        var templateId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var today = DateTime.UtcNow.Date;

        var expectedTransactions = new List<ExpectedTransaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                RecurringTransactionTemplateId = templateId,
                UserId = userId,
                Description = "Past Transaction",
                ExpectedAmount = 1000m,
                Status = ExpectedTransactionStatus.Confirmed,
                ExpectedDate = today.AddDays(-10)
            },
            new()
            {
                Id = Guid.NewGuid(),
                RecurringTransactionTemplateId = templateId,
                UserId = userId,
                Description = "Today Transaction",
                ExpectedAmount = 1500m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = today
            },
            new()
            {
                Id = Guid.NewGuid(),
                RecurringTransactionTemplateId = templateId,
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
        var result = await service.GetTransactionsByTemplateAsync(templateId);

        // Assert
        var expectedTransactionViewModels = result.ToList();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().HaveCount(3);

        expectedTransactionViewModels[0].Description.Should().Be("Past Transaction"); // Earliest date
        expectedTransactionViewModels[1].Description.Should().Be("Today Transaction"); // Middle date
        expectedTransactionViewModels[2].Description.Should().Be("Future Transaction"); // Latest date
    }
}