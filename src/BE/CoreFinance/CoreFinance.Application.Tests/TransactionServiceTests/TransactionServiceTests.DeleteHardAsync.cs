using CoreFinance.Application.DTOs;
using CoreFinance.Application.Services;
using CoreFinance.Domain;
using CoreFinance.Domain.BaseRepositories;
using CoreFinance.Domain.UnitOfWorks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CoreFinance.Application.Tests.TransactionServiceTests;

public partial class TransactionServiceTests
{
    [Fact]
    public async Task DeleteHardAsync_ShouldReturnOne_WhenTransactionIsDeleted()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.DeleteHardAsync(transactionId)).ReturnsAsync(1);
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.DeleteHardAsync(transactionId);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task DeleteHardAsync_ShouldReturnAffectedCount_WhenDeletionIsSuccessful()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var expectedAffectedCount = 1;
        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.DeleteHardAsync(transactionId)).ReturnsAsync(expectedAffectedCount);
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);
        unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(expectedAffectedCount);
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.DeleteHardAsync(transactionId);

        // Assert
        result.Should().Be(expectedAffectedCount);
        repoMock.Verify(r => r.DeleteHardAsync(transactionId), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteHardAsync_ShouldReturnZero_WhenEntityDoesNotExist()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var expectedAffectedCount = 0;
        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.DeleteHardAsync(transactionId)).ReturnsAsync(expectedAffectedCount);
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);
        unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(expectedAffectedCount);
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.DeleteHardAsync(transactionId);

        // Assert
        result.Should().Be(expectedAffectedCount);
        repoMock.Verify(r => r.DeleteHardAsync(transactionId), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteHardAsync_ShouldThrowException_WhenRepositoryThrowsException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var expectedException = new InvalidOperationException("Database error");
        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.DeleteHardAsync(transactionId)).ThrowsAsync(expectedException);
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        Func<Task> act = async () => await service.DeleteHardAsync(transactionId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Database error");
        repoMock.Verify(r => r.DeleteHardAsync(transactionId), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteHardAsync_ShouldThrowException_WhenSaveChangesThrowsException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var expectedException = new InvalidOperationException("Save changes failed");
        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.DeleteHardAsync(transactionId)).ReturnsAsync(1);
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);
        unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ThrowsAsync(expectedException);
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        Func<Task> act = async () => await service.DeleteHardAsync(transactionId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Save changes failed");
        repoMock.Verify(r => r.DeleteHardAsync(transactionId), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
