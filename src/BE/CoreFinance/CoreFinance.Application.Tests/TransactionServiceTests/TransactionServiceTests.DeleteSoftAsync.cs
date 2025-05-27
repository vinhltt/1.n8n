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
    public async Task DeleteSoftAsync_ShouldReturnOne_WhenTransactionIsDeleted()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.DeleteSoftAsync(transactionId)).ReturnsAsync(1);
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.DeleteSoftAsync(transactionId);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task DeleteSoftAsync_ShouldReturnAffectedCount_WhenDeletionIsSuccessful()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var expectedAffectedCount = 1;
        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.DeleteSoftAsync(transactionId)).ReturnsAsync(expectedAffectedCount);
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.DeleteSoftAsync(transactionId);

        // Assert
        result.Should().Be(expectedAffectedCount);
        repoMock.Verify(r => r.DeleteSoftAsync(transactionId), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteSoftAsync_ShouldReturnZero_WhenEntityDoesNotExist()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var expectedAffectedCount = 0;
        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.DeleteSoftAsync(transactionId)).ReturnsAsync(expectedAffectedCount);
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.DeleteSoftAsync(transactionId);

        // Assert
        result.Should().Be(expectedAffectedCount);
        repoMock.Verify(r => r.DeleteSoftAsync(transactionId), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteSoftAsync_ShouldThrowException_WhenRepositoryThrowsException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var expectedException = new InvalidOperationException("Database error during soft delete");
        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.DeleteSoftAsync(transactionId)).ThrowsAsync(expectedException);
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        Func<Task> act = async () => await service.DeleteSoftAsync(transactionId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Database error during soft delete");
        repoMock.Verify(r => r.DeleteSoftAsync(transactionId), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task DeleteSoftAsync_ShouldHandleDifferentAffectedCounts(int affectedCount)
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.DeleteSoftAsync(transactionId)).ReturnsAsync(affectedCount);
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.DeleteSoftAsync(transactionId);

        // Assert
        result.Should().Be(affectedCount);
        repoMock.Verify(r => r.DeleteSoftAsync(transactionId), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}