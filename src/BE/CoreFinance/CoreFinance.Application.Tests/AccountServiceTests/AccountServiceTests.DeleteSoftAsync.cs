using CoreFinance.Application.Services;
using CoreFinance.Domain;
using CoreFinance.Domain.BaseRepositories;
using CoreFinance.Domain.UnitOfWorks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CoreFinance.Application.Tests.AccountServiceTests;

// Tests for the DeleteSoftAsync methods of AccountService
public partial class AccountServiceTests
{
    [Fact]
    public async Task DeleteSoftAsync_ShouldReturnAffectedCount_WhenDeletionIsSuccessful()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var expectedAffectedCount = 1;

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.DeleteSoftAsync(accountId))
            .ReturnsAsync(expectedAffectedCount);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<AccountService>>();
        var service = new AccountService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.DeleteSoftAsync(accountId);

        // Assert
        result.Should().Be(expectedAffectedCount);

        repoMock.Verify(r => r.DeleteSoftAsync(accountId), Times.Once);
        // DeleteSoftAsync doesn't call SaveChangesAsync in BaseService, it's handled by repository
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteSoftAsync_ShouldReturnZero_WhenEntityDoesNotExist()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var expectedAffectedCount = 0;

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.DeleteSoftAsync(accountId))
            .ReturnsAsync(expectedAffectedCount);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<AccountService>>();
        var service = new AccountService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.DeleteSoftAsync(accountId);

        // Assert
        result.Should().Be(expectedAffectedCount);

        repoMock.Verify(r => r.DeleteSoftAsync(accountId), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteSoftAsync_ShouldThrowException_WhenRepositoryThrowsException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var expectedException = new InvalidOperationException("Database error during soft delete");

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.DeleteSoftAsync(accountId))
            .ThrowsAsync(expectedException);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<AccountService>>();
        var service = new AccountService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        Func<Task> act = async () => await service.DeleteSoftAsync(accountId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Database error during soft delete");

        repoMock.Verify(r => r.DeleteSoftAsync(accountId), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task DeleteHardAsync_ShouldHandleDifferentAffectedCounts(int affectedCount)
    {
        // Arrange
        var accountId = Guid.NewGuid();

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.DeleteHardAsync(accountId))
            .ReturnsAsync(affectedCount);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);
        unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(affectedCount);

        var loggerMock = new Mock<ILogger<AccountService>>();
        var service = new AccountService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.DeleteHardAsync(accountId);

        // Assert
        result.Should().Be(affectedCount);

        repoMock.Verify(r => r.DeleteHardAsync(accountId), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task DeleteSoftAsync_ShouldHandleDifferentAffectedCounts(int affectedCount)
    {
        // Arrange
        var accountId = Guid.NewGuid();

        var repoMock = new Mock<IBaseRepository<Account, Guid>>();
        repoMock.Setup(r => r.DeleteSoftAsync(accountId))
            .ReturnsAsync(affectedCount);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<Account, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<AccountService>>();
        var service = new AccountService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.DeleteSoftAsync(accountId);

        // Assert
        result.Should().Be(affectedCount);

        repoMock.Verify(r => r.DeleteSoftAsync(accountId), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
} 