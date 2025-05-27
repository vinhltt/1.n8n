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

// Tests for the GetUpcomingTransactionsAsync method of ExpectedTransactionService
public partial class ExpectedTransactionServiceTests
{
    [Fact]
    public async Task GetUpcomingTransactionsAsync_ShouldReturnUpcomingPendingTransactions_WithDefaultDays()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var today = DateTime.UtcNow.Date;

        var expectedTransactions = new List<ExpectedTransaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = "Transaction Today",
                ExpectedAmount = 1000m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = today
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = "Transaction in 15 days",
                ExpectedAmount = 1500m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = today.AddDays(15)
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = "Transaction in 29 days",
                ExpectedAmount = 2000m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = today.AddDays(29)
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = "Transaction in 31 days", // Should be excluded (beyond 30 days)
                ExpectedAmount = 2500m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = today.AddDays(31)
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = "Past Transaction", // Should be excluded (in the past)
                ExpectedAmount = 3000m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = today.AddDays(-1)
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
        var result = await service.GetUpcomingTransactionsAsync(userId); // Default 30 days

        // Assert
        var expectedTransactionViewModels = result.ToList();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().HaveCount(3);
        expectedTransactionViewModels.Should().OnlyContain(t => t.UserId == userId);
        expectedTransactionViewModels.Should().OnlyContain(t => t.Status == ExpectedTransactionStatus.Pending);
        expectedTransactionViewModels.Should()
            .OnlyContain(t => t.ExpectedDate >= today && t.ExpectedDate <= today.AddDays(30));
        expectedTransactionViewModels.Select(t => t.Description).Should().Contain(new[]
            { "Transaction Today", "Transaction in 15 days", "Transaction in 29 days" });

        // Should be ordered by ExpectedDate
        var resultList = expectedTransactionViewModels.ToList();
        resultList[0].Description.Should().Be("Transaction Today");
        resultList[1].Description.Should().Be("Transaction in 15 days");
        resultList[2].Description.Should().Be("Transaction in 29 days");
    }

    [Fact]
    public async Task GetUpcomingTransactionsAsync_ShouldReturnUpcomingPendingTransactions_WithCustomDays()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var today = DateTime.UtcNow.Date;
        var customDays = 7;

        var expectedTransactions = new List<ExpectedTransaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = "Transaction in 3 days",
                ExpectedAmount = 1000m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = today.AddDays(3)
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = "Transaction in 7 days",
                ExpectedAmount = 1500m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = today.AddDays(7)
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = "Transaction in 10 days", // Should be excluded (beyond 7 days)
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
        var result = await service.GetUpcomingTransactionsAsync(userId, customDays);

        // Assert
        var expectedTransactionViewModels = result.ToList();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().HaveCount(2);
        expectedTransactionViewModels.Should()
            .OnlyContain(t => t.ExpectedDate >= today && t.ExpectedDate <= today.AddDays(customDays));
        expectedTransactionViewModels.Select(t => t.Description).Should()
            .Contain(new[] { "Transaction in 3 days", "Transaction in 7 days" });
    }

    [Fact]
    public async Task GetUpcomingTransactionsAsync_ShouldIgnoreNonPendingTransactions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var today = DateTime.UtcNow.Date;

        var expectedTransactions = new List<ExpectedTransaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = "Pending Transaction",
                ExpectedAmount = 1000m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = today.AddDays(5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = "Confirmed Transaction", // Should be ignored
                ExpectedAmount = 1500m,
                Status = ExpectedTransactionStatus.Confirmed,
                ExpectedDate = today.AddDays(10)
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = "Cancelled Transaction", // Should be ignored
                ExpectedAmount = 2000m,
                Status = ExpectedTransactionStatus.Cancelled,
                ExpectedDate = today.AddDays(15)
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
        var result = await service.GetUpcomingTransactionsAsync(userId);

        // Assert
        var expectedTransactionViewModels = result.ToList();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().HaveCount(1);
        expectedTransactionViewModels.Should().OnlyContain(t => t.Status == ExpectedTransactionStatus.Pending);
        expectedTransactionViewModels.First().Description.Should().Be("Pending Transaction");
    }

    [Fact]
    public async Task GetUpcomingTransactionsAsync_ShouldFilterByUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var today = DateTime.UtcNow.Date;

        var expectedTransactions = new List<ExpectedTransaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = "User Transaction",
                ExpectedAmount = 1000m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = today.AddDays(5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = otherUserId,
                Description = "Other User Transaction", // Should be ignored
                ExpectedAmount = 1500m,
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
        var result = await service.GetUpcomingTransactionsAsync(userId);

        // Assert
        var expectedTransactionViewModels = result.ToList();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().HaveCount(1);
        expectedTransactionViewModels.Should().OnlyContain(t => t.UserId == userId);
        expectedTransactionViewModels.First().Description.Should().Be("User Transaction");
    }

    [Fact]
    public async Task GetUpcomingTransactionsAsync_ShouldReturnEmptyList_WhenNoUpcomingTransactions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var today = DateTime.UtcNow.Date;

        var expectedTransactions = new List<ExpectedTransaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = "Past Transaction",
                ExpectedAmount = 1000m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = today.AddDays(-5) // In the past
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = "Far Future Transaction",
                ExpectedAmount = 1500m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = today.AddDays(50) // Beyond 30 days
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
        var result = await service.GetUpcomingTransactionsAsync(userId);

        // Assert
        var expectedTransactionViewModels = result.ToList();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUpcomingTransactionsAsync_ShouldReturnEmptyList_WhenNoTransactionsExist()
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
        var result = await service.GetUpcomingTransactionsAsync(userId);

        // Assert
        var expectedTransactionViewModels = result.ToList();
        expectedTransactionViewModels.Should().NotBeNull();
        expectedTransactionViewModels.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUpcomingTransactionsAsync_ShouldReturnTransactionsOrderedByExpectedDate()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var today = DateTime.UtcNow.Date;

        var expectedTransactions = new List<ExpectedTransaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = "Transaction C",
                ExpectedAmount = 1000m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = today.AddDays(20)
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = "Transaction A",
                ExpectedAmount = 1500m,
                Status = ExpectedTransactionStatus.Pending,
                ExpectedDate = today.AddDays(5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = "Transaction B",
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
        var result = await service.GetUpcomingTransactionsAsync(userId);

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
}