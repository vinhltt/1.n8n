using CoreFinance.Application.DTOs;
using CoreFinance.Application.Services;
using CoreFinance.Domain;
using CoreFinance.Domain.BaseRepositories;
using CoreFinance.Domain.UnitOfWorks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CoreFinance.Application.Tests.TransactionServiceTests;

public partial class TransactionServiceTests
{
    [Fact]
    public async Task GetAllDtoAsync_ShouldReturnAllTransactions_WhenTransactionsExist()
    {
        // Arrange
        var transactions = new List<Transaction>
        {
            new() { Id = Guid.NewGuid(), Description = "Salary", RevenueAmount = 1000, SpentAmount = 0 },
            new() { Id = Guid.NewGuid(), Description = "Groceries", RevenueAmount = 0, SpentAmount = 200 },
            new() { Id = Guid.NewGuid(), Description = "Transfer", RevenueAmount = 0, SpentAmount = 500 }
        };

        var transactionsMock = transactions.AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities()).Returns(transactionsMock);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<TransactionService>>();

        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetAllDtoAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(transactions.Count);
        var expectedViewModels = transactions.Select(_mapper.Map<TransactionViewModel>).ToList();
        result.Should().BeEquivalentTo(expectedViewModels);
        repoMock.Verify(r => r.GetNoTrackingEntities(), Times.Once);
    }

    [Fact]
    public async Task GetAllDtoAsync_ShouldReturnEmptyList_WhenNoTransactionsExist()
    {
        // Arrange
        var emptyTransactions = new List<Transaction>().AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<Transaction, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities()).Returns(emptyTransactions);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Transaction, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<TransactionService>>();

        var service = new TransactionService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetAllDtoAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        repoMock.Verify(r => r.GetNoTrackingEntities(), Times.Once);
    }
}
