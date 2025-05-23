using Bogus;
using CoreFinance.Application.DTOs;
using CoreFinance.Application.Services;
using CoreFinance.Domain;
using CoreFinance.Domain.BaseRepositories;
using CoreFinance.Domain.Exceptions;
using CoreFinance.Domain.UnitOfWorks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;

namespace CoreFinance.Application.Tests.AccountServiceTests;

public partial class AccountServiceTests
{
    [Fact]
    public async Task CreateAsync_List_ValidRequest_ReturnsViewModels()
    {
        // Arrange
        var numberOfAccounts = 5;
        var createRequests = new Faker<AccountCreateRequest>()
            .RuleFor(r => r.Name, f => f.Finance.AccountName())
            .RuleFor(r => r.Type, f => f.PickRandom<AccountType>())
            .RuleFor(r => r.Currency, f => f.Finance.Currency().Code)
            .RuleFor(r => r.UserId, Guid.NewGuid())
            .Generate(numberOfAccounts);

        var createdEntities = _mapper.Map<List<Account>>(createRequests);
        createdEntities.ForEach(e => e.Id = Guid.NewGuid());

        var expectedViewModels = _mapper.Map<IEnumerable<AccountViewModel>>(createdEntities);

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.CreateAsync(It.IsAny<List<Account>>()))
            .ReturnsAsync(numberOfAccounts);
        
        var transactionMock = new Mock<IDbContextTransaction>();
        transactionMock.Setup(t => t.DisposeAsync()).Returns(ValueTask.CompletedTask);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);
        unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.FromResult(transactionMock.Object));

        var loggerMock = new Mock<ILogger<AccountService>>();
        var service = new AccountService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.CreateAsync(createRequests);

        // Assert
        var accountViewModels = result!.ToList();
        accountViewModels.Should().NotBeNullOrEmpty();
        accountViewModels.Should().HaveCount(numberOfAccounts);
        accountViewModels.Should().BeEquivalentTo(expectedViewModels, options => options.ExcludingMissingMembers().Excluding(e => e.Id));

        repoMock.Verify(r => r.CreateAsync(It.Is<List<Account>>(list => list.Count == numberOfAccounts)), Times.Once);
        unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
        transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        transactionMock.Verify(t => t.DisposeAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_List_EmptyRequestList_ReturnsEmptyList()
    {
        // Arrange
        var createRequests = new List<AccountCreateRequest>();

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        // BeginTransactionAsync should not be called for empty list as per BaseService logic
        
        var loggerMock = new Mock<ILogger<AccountService>>();
        var service = new AccountService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.CreateAsync(createRequests);

        // Assert
        var accountViewModels = result!.ToList();
        accountViewModels.Should().NotBeNull();
        accountViewModels.Should().BeEmpty();

        repoMock.Verify(r => r.CreateAsync(It.IsAny<List<Account>>()), Times.Never);
        unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_List_RepositoryReturnsZeroAffected_ThrowsNullReferenceException()
    {
        // Arrange
        var numberOfAccounts = 5;
        var createRequests = new Faker<AccountCreateRequest>()
            .RuleFor(r => r.Name, f => f.Finance.AccountName())
            .Generate(numberOfAccounts);

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.CreateAsync(It.IsAny<List<Account>>()))
            .ReturnsAsync(0);
        
        var transactionMock = new Mock<IDbContextTransaction>();
        transactionMock.Setup(t => t.DisposeAsync()).Returns(ValueTask.CompletedTask);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);
        unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.FromResult(transactionMock.Object));

        var loggerMock = new Mock<ILogger<AccountService>>();
        var service = new AccountService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        Func<Task> act = async () => await service.CreateAsync(createRequests);

        // Assert
        await act.Should().ThrowAsync<CreateFailedException>();

        repoMock.Verify(r => r.CreateAsync(It.Is<List<Account>>(list => list.Count == numberOfAccounts)), Times.Once);
        unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
        transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        transactionMock.Verify(t => t.DisposeAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_List_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var numberOfAccounts = 5;
        var createRequests = new Faker<AccountCreateRequest>()
            .RuleFor(r => r.Name, f => f.Finance.AccountName())
            .Generate(numberOfAccounts);
        
        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.CreateAsync(It.IsAny<List<Account>>()))
            .ThrowsAsync(new InvalidOperationException("Simulated DB error"));

        var transactionMock = new Mock<IDbContextTransaction>();
        transactionMock.Setup(t => t.DisposeAsync()).Returns(ValueTask.CompletedTask);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);
        unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.FromResult(transactionMock.Object));

        var loggerMock = new Mock<ILogger<AccountService>>();
        var service = new AccountService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        Func<Task> act = async () => await service.CreateAsync(createRequests);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Simulated DB error");

        repoMock.Verify(r => r.CreateAsync(It.Is<List<Account>>(list => list.Count == numberOfAccounts)), Times.Once);
        unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
        transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        transactionMock.Verify(t => t.DisposeAsync(), Times.Once);
    }
} 