using CoreFinance.Application.DTOs;
using CoreFinance.Application.Services;
using CoreFinance.Domain;
using CoreFinance.Domain.BaseRepositories;
using CoreFinance.Domain.UnitOfWorks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;

namespace CoreFinance.Application.Tests.AccountServiceTests;

// Tests for the GetAllDtoAsync method of AccountService
public partial class AccountServiceTests
{
    [Fact]
    public async Task GetAllDtoAsync_ShouldReturnAllAccounts_WhenAccountsExist()
    {
        // Arrange
        var accounts = new List<Account>
        {
            new() { Id = Guid.NewGuid(), Name = "Account 1", Type = AccountType.Bank, Currency = "USD", InitialBalance = 100 },
            new() { Id = Guid.NewGuid(), Name = "Account 2", Type = AccountType.Cash, Currency = "EUR", InitialBalance = 50 },
            new() { Id = Guid.NewGuid(), Name = "Account 3", Type = AccountType.CreditCard, Currency = "VND", InitialBalance = 0 }
        };

        var accountsMock = accounts.AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        // Assuming GetAllDtoAsync uses GetNoTrackingEntities()
        repoMock.Setup(r => r.GetNoTrackingEntities()).Returns(accountsMock);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<AccountService>>();

        var service = new AccountService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetAllDtoAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(accounts.Count);
        
        // Map the expected accounts using the real mapper to compare with the result
        var expectedViewModels = accounts.Select(_mapper.Map<AccountViewModel>).ToList();
        result.Should().BeEquivalentTo(expectedViewModels);

        // Verify that the repository method was called
        repoMock.Verify(r => r.GetNoTrackingEntities(), Times.Once);
    }

    [Fact]
    public async Task GetAllDtoAsync_ShouldReturnEmptyList_WhenNoAccountsExist()
    {
        // Arrange
        var emptyAccounts = new List<Account>().AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        // Assuming GetAllDtoAsync uses GetNoTrackingEntities()
        repoMock.Setup(r => r.GetNoTrackingEntities()).Returns(emptyAccounts);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<AccountService>>();

        var service = new AccountService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetAllDtoAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        // Verify that the repository method was called
        repoMock.Verify(r => r.GetNoTrackingEntities(), Times.Once);
    }
} 
