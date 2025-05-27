using CoreFinance.Application.Services;
using CoreFinance.Domain;
using CoreFinance.Domain.BaseRepositories;
using CoreFinance.Domain.UnitOfWorks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CoreFinance.Application.Tests.TransactionServiceTests;

public partial class TransactionServiceTests
{
    [Fact]
    public async Task GetByIdAsync_ShouldReturnTransaction_WhenTransactionExists()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var repositoryMock = new Mock<IBaseRepository<Transaction, Guid>>();

        var transactionId = Guid.NewGuid();
        var transaction = new Transaction { Id = transactionId, Description = "Test Transaction" };

        unitOfWorkMock.Setup(uow => uow.Repository<Transaction, Guid>()).Returns(repositoryMock.Object);
        repositoryMock.Setup(repo => repo.GetByIdNoTrackingAsync(transactionId,
            It.IsAny<System.Linq.Expressions.Expression<Func<Transaction, object>>[]>())).ReturnsAsync(transaction);

        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetByIdAsync(transactionId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(transactionId);
        result.Description.Should().Be("Test Transaction");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenTransactionDoesNotExist()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var repositoryMock = new Mock<IBaseRepository<Transaction, Guid>>();

        var transactionId = Guid.NewGuid();

        unitOfWorkMock.Setup(uow => uow.Repository<Transaction, Guid>()).Returns(repositoryMock.Object);
        repositoryMock
            .Setup(repo => repo.GetByIdNoTrackingAsync(transactionId,
                It.IsAny<System.Linq.Expressions.Expression<Func<Transaction, object>>[]>()))
            .ReturnsAsync((Transaction?)null);

        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetByIdAsync(transactionId);

        // Assert
        result.Should().BeNull();
    }
}