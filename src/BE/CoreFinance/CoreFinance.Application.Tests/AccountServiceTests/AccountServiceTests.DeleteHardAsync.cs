using CoreFinance.Application.Services;
using CoreFinance.Domain;
using CoreFinance.Domain.BaseRepositories;
using CoreFinance.Domain.UnitOfWorks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CoreFinance.Application.Tests.AccountServiceTests;

// Tests for the DeleteHardAsync methods of AccountService
public partial class AccountServiceTests
{

    [Fact]
    public async Task DeleteHardAsync_ShouldReturnAffectedCount_WhenDeletionIsSuccessful()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var expectedAffectedCount = 1;

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.DeleteHardAsync(accountId))
            .ReturnsAsync(expectedAffectedCount);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);
        unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(expectedAffectedCount);

        var loggerMock = new Mock<ILogger<AccountService>>();
        var service = new AccountService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.DeleteHardAsync(accountId);

        // Assert
        result.Should().Be(expectedAffectedCount);

        repoMock.Verify(r => r.DeleteHardAsync(accountId), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteHardAsync_ShouldReturnZero_WhenEntityDoesNotExist()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var expectedAffectedCount = 0;

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.DeleteHardAsync(accountId))
            .ReturnsAsync(expectedAffectedCount);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);
        unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(expectedAffectedCount);

        var loggerMock = new Mock<ILogger<AccountService>>();
        var service = new AccountService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.DeleteHardAsync(accountId);

        // Assert
        result.Should().Be(expectedAffectedCount);

        repoMock.Verify(r => r.DeleteHardAsync(accountId), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteHardAsync_ShouldThrowException_WhenRepositoryThrowsException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var expectedException = new InvalidOperationException("Database error");

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.DeleteHardAsync(accountId))
            .ThrowsAsync(expectedException);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<AccountService>>();
        var service = new AccountService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        Func<Task> act = async () => await service.DeleteHardAsync(accountId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Database error");

        repoMock.Verify(r => r.DeleteHardAsync(accountId), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteHardAsync_ShouldThrowException_WhenSaveChangesThrowsException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var expectedException = new InvalidOperationException("Save changes failed");

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.DeleteHardAsync(accountId))
            .ReturnsAsync(1);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);
        unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ThrowsAsync(expectedException);

        var loggerMock = new Mock<ILogger<AccountService>>();
        var service = new AccountService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        Func<Task> act = async () => await service.DeleteHardAsync(accountId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Save changes failed");

        repoMock.Verify(r => r.DeleteHardAsync(accountId), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
} 