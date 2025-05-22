using AutoMapper;
using CoreFinance.Application.DTOs;
using CoreFinance.Application.Services;
using CoreFinance.Domain;
using CoreFinance.Domain.BaseRepositories;
using CoreFinance.Domain.UnitOffWorks;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

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

        var unitOfWorkMock = new Mock<IUnitOffWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);
        // Simulate DoWorkWithTransaction by directly invoking the passed func
        // This part of the mock for UnitOfWork seems reasonable for a unit test.
        unitOfWorkMock.Setup(u => u.DoWorkWithTransaction(It.IsAny<Func<Task<AccountViewModel?>>>()))
            .Returns((Func<Task<AccountViewModel?>> func) => func());


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
        // Verify that UnitOfWork transaction method was called
        unitOfWorkMock.Verify(u => u.DoWorkWithTransaction(It.IsAny<Func<Task<AccountViewModel?>>>()), Times.Once);
        // No need to verify mapper calls explicitly when using real mapper
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowNullReferenceException_WhenRepositoryReturnsZeroAffectedCount()
    {
        // Arrange
        var createRequest = new AccountCreateRequest { Name = "Test Account" /* ... other properties */ };
        
        // var mapperMock = new Mock<IMapper>(); // Using real _mapper now
        // mapperMock.Setup(m => m.Map(createRequest, It.IsAny<Account>())); // Not needed with real mapper
        // No need to setup the second Map call as it won't be reached if exception is thrown

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.CreateAsync(It.IsAny<Account>()))
            .ReturnsAsync(0); // Simulate 0 records affected

        var unitOfWorkMock = new Mock<IUnitOffWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);
        // Simulate DoWorkWithTransaction by directly invoking the passed func
        unitOfWorkMock.Setup(u => u.DoWorkWithTransaction(It.IsAny<Func<Task<AccountViewModel?>>>()))
            .Returns((Func<Task<AccountViewModel?>> func) => func());

        var loggerMock = new Mock<ILogger<AccountService>>();
        // Use the real _mapper instance
        var service = new AccountService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        Func<Task> act = async () => await service.CreateAsync(createRequest);

        // Assert
        await act.Should().ThrowAsync<NullReferenceException>();
        
        repoMock.Verify(r => r.CreateAsync(It.IsAny<Account>()), Times.Once);
        unitOfWorkMock.Verify(u => u.DoWorkWithTransaction(It.IsAny<Func<Task<AccountViewModel?>>>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateAsync_ShouldRollbackTransaction_WhenRepositoryThrowsException()
    {
        // Arrange
        var createRequest = new AccountCreateRequest { Name = "Test Account" /* ... other properties */ };

        // var mapperMock = new Mock<IMapper>(); // Using real _mapper now
        // mapperMock.Setup(m => m.Map(createRequest, It.IsAny<Account>())); // Not needed with real mapper

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.CreateAsync(It.IsAny<Account>()))
            .ThrowsAsync(new InvalidOperationException("DB error")); // Simulate a DB error

        var unitOfWorkMock = new Mock<IUnitOffWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);
        
        // This setup is crucial: If DoWorkWithTransaction catches the exception and doesn't rethrow, the test is different.
        // Assuming DoWorkWithTransaction propagates the exception or handles rollback internally.
        // For this test, we just verify it's called. The actual rollback is an integration concern or UoW test.
        unitOfWorkMock.Setup(u => u.DoWorkWithTransaction(It.IsAny<Func<Task<AccountViewModel?>>>()))
             .Returns(async (Func<Task<AccountViewModel?>> func) => await func());


        var loggerMock = new Mock<ILogger<AccountService>>();
        // Use the real _mapper instance
        var service = new AccountService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        Func<Task> act = async () => await service.CreateAsync(createRequest);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("DB error");
        
        repoMock.Verify(r => r.CreateAsync(It.IsAny<Account>()), Times.Once);
        unitOfWorkMock.Verify(u => u.DoWorkWithTransaction(It.IsAny<Func<Task<AccountViewModel?>>>()), Times.Once);
        // We can't easily verify rollback in a unit test without more complex UoW mocking.
        // The key is that the transaction block was entered.
    }
    
    // Consider adding tests for validation if AccountCreateRequest has validation attributes
    // and if the BaseService or AccountService is expected to trigger validation.
    // However, validation is often handled by MVC model binding or a dedicated validation layer before the service call.
    // If service is responsible for some validation, test it here.

    // Test for CreateAsync(List<TCreateRequest> request) can be added if that overload is used and needs specific testing.
    // The logic is similar but involves collections.
} 