using CoreFinance.Application.DTOs;
using CoreFinance.Application.Services;
using CoreFinance.Domain;
using CoreFinance.Domain.BaseRepositories;
using CoreFinance.Domain.UnitOfWorks;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Storage;

namespace CoreFinance.Application.Tests.AccountServiceTests;

// Tests for the CreateAsync method of AccountService
public partial class AccountServiceTests
{
    // Helper method has been moved to TestHelpers.cs
    // private static IQueryable<Account> GenerateFakeAccounts(int count) { ... }

    // --- CreateAsync Tests ---

    [Fact]
    public async Task CreateAsync_ShouldReturnViewModel_WhenCreationIsSuccessful()
    {
        // Arrange
        var createRequest = new AccountCreateRequest 
        { 
            Name = "New Savings Account", 
            Type = AccountType.Bank, 
            Currency = "USD", 
            InitialBalance = 1000,
            UserId = Guid.NewGuid()
            // Add other required properties for AccountCreateRequest
        };

        // Expected ViewModel properties will be based on the mapping profile
        // No need to manually construct expectedViewModel with dynamic Id, Currency, etc.
        // We will assert against the properties of the result directly.

        // var mapperMock = new Mock<IMapper>(); // Using real _mapper now
        // mapperMock.Setup(m => m.Map(createRequest, It.IsAny<Account>())) // Not needed with real mapper
        //     .Callback<AccountCreateRequest, Account>((src, dest) => 
        //     { /* Simulate AutoMapper's behavior */ });
        
        // mapperMock.Setup(m => m.Map<AccountViewModel>(It.IsAny<Account>())) // Not needed with real mapper
        //     .Returns((Account src) => new AccountViewModel { /* Map manually */ });


        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.CreateAsync(It.IsAny<Account>()))
            .ReturnsAsync(1); // Simulate 1 record affected

        var transactionMock = new Mock<IDbContextTransaction>();
        transactionMock.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        transactionMock.Setup(t => t.DisposeAsync()).Returns(ValueTask.CompletedTask);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);
        unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.FromResult(transactionMock.Object));


        var loggerMock = new Mock<ILogger<AccountService>>();
        // Use the real _mapper instance available from the partial class constructor
        var service = new AccountService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.CreateAsync(createRequest);

        // Assert
        result.Should().NotBeNull();
        // Assert properties directly to verify mapping logic
        result.Name.Should().Be(createRequest.Name);
        result.Type.Should().Be(createRequest.Type);
        result.Currency.Should().Be(createRequest.Currency);
        result.InitialBalance.Should().Be(createRequest.InitialBalance); // Based on profile mapping

        // Verify that the repository method was called
        repoMock.Verify(r => r.CreateAsync(It.IsAny<Account>()), Times.Once);
        unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
        transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        transactionMock.Verify(t => t.DisposeAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowNullReferenceException_WhenRepositoryReturnsZeroAffectedCount()
    {
        // Arrange
        var createRequest = new AccountCreateRequest { Name = "Test Account", UserId = Guid.NewGuid() };
        
        // var mapperMock = new Mock<IMapper>(); // Using real _mapper now
        // mapperMock.Setup(m => m.Map(createRequest, It.IsAny<Account>())); // Not needed with real mapper
        // No need to setup the second Map call as it won't be reached if exception is thrown

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.CreateAsync(It.IsAny<Account>()))
            .ReturnsAsync(0); // Simulate 0 records affected

        var transactionMock = new Mock<IDbContextTransaction>();
        // According to BaseService.cs, CommitAsync is called in the catch block when effectedCount <= 0
        transactionMock.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask); 
        transactionMock.Setup(t => t.DisposeAsync()).Returns(ValueTask.CompletedTask);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);
        unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.FromResult(transactionMock.Object));

        var loggerMock = new Mock<ILogger<AccountService>>();
        // Use the real _mapper instance
        var service = new AccountService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        Func<Task> act = async () => await service.CreateAsync(createRequest);

        // Assert
        await act.Should().ThrowAsync<NullReferenceException>();
        
        repoMock.Verify(r => r.CreateAsync(It.IsAny<Account>()), Times.Once);
        unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
        transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once); // BaseService commits in catch
        transactionMock.Verify(t => t.DisposeAsync(), Times.Once);
    }
    
    [Fact]
    public async Task CreateAsync_ShouldRollbackTransaction_WhenRepositoryThrowsException()
    {
        // Arrange
        var createRequest = new AccountCreateRequest { Name = "Test Account", UserId = Guid.NewGuid() };

        // var mapperMock = new Mock<IMapper>(); // Using real _mapper now
        // mapperMock.Setup(m => m.Map(createRequest, It.IsAny<Account>())); // Not needed with real mapper

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.CreateAsync(It.IsAny<Account>()))
            .ThrowsAsync(new InvalidOperationException("DB error")); // Simulate a DB error

        var transactionMock = new Mock<IDbContextTransaction>();
        // According to BaseService.cs, CommitAsync is called in the catch block when an exception occurs
        transactionMock.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask); 
        transactionMock.Setup(t => t.DisposeAsync()).Returns(ValueTask.CompletedTask);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);
        unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.FromResult(transactionMock.Object));


        var loggerMock = new Mock<ILogger<AccountService>>();
        // Use the real _mapper instance
        var service = new AccountService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        Func<Task> act = async () => await service.CreateAsync(createRequest);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("DB error");
        
        repoMock.Verify(r => r.CreateAsync(It.IsAny<Account>()), Times.Once);
        unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
        transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once); // BaseService commits in catch
        transactionMock.Verify(t => t.DisposeAsync(), Times.Once);
    }
} 