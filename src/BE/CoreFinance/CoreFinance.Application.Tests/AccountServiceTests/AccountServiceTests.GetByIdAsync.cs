using CoreFinance.Application.Services;
using CoreFinance.Domain;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using CoreFinance.Domain.BaseRepositories;
using CoreFinance.Domain.UnitOfWorks;

namespace CoreFinance.Application.Tests.AccountServiceTests;

public partial class AccountServiceTests
{
    [Fact]
    public async Task GetByIdAsync_ShouldReturnAccount_WhenAccountExists()
    {
        // Arrange
        var unitOffWorkMock = new Mock<IUnitOfWork>();
        var loggerMock = new Mock<ILogger<AccountService>>();
        var repositoryMock = new Mock<IBaseRepository<Account, Guid>>();

        var accountId = Guid.NewGuid();
        var account = new Account { Id = accountId, Name = "Test Account" };

        unitOffWorkMock.Setup(uow => uow.Repository<Account, Guid>()).Returns(repositoryMock.Object);
        repositoryMock.Setup(repo => repo.GetByIdNoTrackingAsync(accountId)).ReturnsAsync(account);

        var accountService = new AccountService(_mapper, unitOffWorkMock.Object, loggerMock.Object);

        // Act
        var result = await accountService.GetByIdAsync(accountId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(accountId);
        result.Name.Should().Be("Test Account");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenAccountDoesNotExist()
    {
        // Arrange
        var unitOffWorkMock = new Mock<IUnitOfWork>();
        var loggerMock = new Mock<ILogger<AccountService>>();
        var repositoryMock = new Mock<IBaseRepository<Account, Guid>>();

        var accountId = Guid.NewGuid();

        unitOffWorkMock.Setup(uow => uow.Repository<Account, Guid>()).Returns(repositoryMock.Object);
        repositoryMock.Setup(repo => repo.GetByIdAsync(accountId)).ReturnsAsync((Account?)null);

        var accountService = new AccountService(_mapper, unitOffWorkMock.Object, loggerMock.Object);

        // Act
        var result = await accountService.GetByIdAsync(accountId);

        // Assert
        result.Should().BeNull();
    }
} 