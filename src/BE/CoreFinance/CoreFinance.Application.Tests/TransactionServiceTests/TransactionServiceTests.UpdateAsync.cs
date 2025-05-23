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
using CoreFinance.Domain.Exceptions;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;

namespace CoreFinance.Application.Tests.TransactionServiceTests;

public partial class TransactionServiceTests
{
    [Fact]
    public async Task UpdateAsync_ShouldReturnUpdatedViewModel_WhenTransactionIsUpdated()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var updateRequest = new TransactionUpdateRequest
        {
            Id = transactionId,
            AccountId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Description = "Updated Description",
            RevenueAmount = 200,
            SpentAmount = 50
        };
        var transaction = new Transaction { Id = transactionId, Description = "Old Description", RevenueAmount = 100, SpentAmount = 0 };
        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.GetByIdAsync(transactionId, It.IsAny<System.Linq.Expressions.Expression<Func<Transaction, object>>[]>())).ReturnsAsync(transaction);
        repoMock.Setup(r => r.UpdateAsync(It.IsAny<Transaction>())).ReturnsAsync(1);
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);
        unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(Mock.Of<IDbContextTransaction>());
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.UpdateAsync(transactionId, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(transactionId);
        result.Description.Should().Be(updateRequest.Description);
        result.RevenueAmount.Should().Be(updateRequest.RevenueAmount);
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_ReturnsUpdatedViewModel()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var updateRequest = new TransactionUpdateRequest
        {
            Id = transactionId,
            AccountId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Description = "Updated Transaction",
            RevenueAmount = 200,
            SpentAmount = 50
        };
        var existingTransaction = new Transaction { Id = transactionId, Description = "Old Transaction", RevenueAmount = 100, SpentAmount = 0 };
        var transactionAfterMap = _mapper.Map(updateRequest, new Transaction());
        transactionAfterMap.Id = existingTransaction.Id;
        var expectedViewModel = _mapper.Map<TransactionViewModel>(transactionAfterMap);
        expectedViewModel.Description = updateRequest.Description;
        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.GetByIdAsync(transactionId)).ReturnsAsync(existingTransaction);
        repoMock.Setup(r => r.UpdateAsync(It.Is<Transaction>(t => t.Id == transactionId && t.Description == updateRequest.Description))).ReturnsAsync(1);
        var transactionMock = new Mock<IDbContextTransaction>();
        transactionMock.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        transactionMock.Setup(t => t.DisposeAsync()).Returns(ValueTask.CompletedTask);
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);
        unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.FromResult(transactionMock.Object));
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);
        // Act
        var result = await service.UpdateAsync(transactionId, updateRequest);
        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedViewModel, options => options.ExcludingMissingMembers());
        repoMock.Verify(r => r.GetByIdAsync(transactionId), Times.Once);
        repoMock.Verify(r => r.UpdateAsync(It.Is<Transaction>(t => t.Id == transactionId && t.Description == updateRequest.Description)), Times.Once);
        unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
        transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        transactionMock.Verify(t => t.DisposeAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_IdMismatch_ThrowsKeyNotFoundException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var requestWithDifferentId = new TransactionUpdateRequest { Id = Guid.NewGuid() };
        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        var transactionMock = new Mock<IDbContextTransaction>();
        transactionMock.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        transactionMock.Setup(t => t.DisposeAsync()).Returns(ValueTask.CompletedTask);
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);
        unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.FromResult(transactionMock.Object));
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);
        // Act
        Func<Task> act = async () => await service.UpdateAsync(transactionId, requestWithDifferentId);
        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
        repoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        repoMock.Verify(r => r.UpdateAsync(It.IsAny<Transaction>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_EntityNotFound_ThrowsNullReferenceException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var updateRequest = new TransactionUpdateRequest { Id = transactionId };
        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.GetByIdAsync(transactionId)).ReturnsAsync((Transaction?)null);
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);
        // Act
        Func<Task> act = async () => await service.UpdateAsync(transactionId, updateRequest);
        // Assert
        await act.Should().ThrowAsync<NullReferenceException>();
        repoMock.Verify(r => r.GetByIdAsync(transactionId), Times.Once);
        repoMock.Verify(r => r.UpdateAsync(It.IsAny<Transaction>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_UpdateFails_ThrowsUpdateFailedException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var updateRequest = new TransactionUpdateRequest { Id = transactionId, Description = "Fail Update" };
        var existingTransaction = new Transaction { Id = transactionId };
        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.GetByIdAsync(transactionId)).ReturnsAsync(existingTransaction);
        repoMock.Setup(r => r.UpdateAsync(It.Is<Transaction>(t => t.Id == transactionId && t.Description == updateRequest.Description))).ReturnsAsync(0);
        var transactionMock = new Mock<IDbContextTransaction>();
        transactionMock.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        transactionMock.Setup(t => t.DisposeAsync()).Returns(ValueTask.CompletedTask);
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);
        unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.FromResult(transactionMock.Object));
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);
        // Act
        Func<Task> act = async () => await service.UpdateAsync(transactionId, updateRequest);
        // Assert
        await act.Should().ThrowAsync<UpdateFailedException>();
        repoMock.Verify(r => r.GetByIdAsync(transactionId), Times.Once);
        repoMock.Verify(r => r.UpdateAsync(It.Is<Transaction>(t => t.Id == transactionId && t.Description == updateRequest.Description)), Times.Once);
        unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
        transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        transactionMock.Verify(t => t.DisposeAsync(), Times.Once);
    }
}
