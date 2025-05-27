using CoreFinance.Application.Services;
using CoreFinance.Domain;
using CoreFinance.Domain.BaseRepositories;
using CoreFinance.Domain.UnitOfWorks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;

namespace CoreFinance.Application.Tests.RecurringTransactionTemplateServiceTests;

public partial class RecurringTransactionTemplateServiceTests
{
    [Fact]
    public async Task ToggleActiveStatusAsync_ShouldReturnTrue_WhenTemplateIsToggledSuccessfully()
    {
        // Arrange
        var templateId = Guid.NewGuid();
        var template = new RecurringTransactionTemplate
        {
            Id = templateId,
            UserId = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            Name = "Test Template",
            IsActive = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var repoMock = new Mock<IBaseRepository<RecurringTransactionTemplate, Guid>>();
        repoMock.Setup(r => r.GetByIdAsync(templateId))
            .ReturnsAsync(template);
        repoMock.Setup(r => r.UpdateAsync(It.IsAny<RecurringTransactionTemplate>()))
            .ReturnsAsync(1);

        var transactionMock = new Mock<IDbContextTransaction>();
        transactionMock.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        transactionMock.Setup(t => t.DisposeAsync()).Returns(ValueTask.CompletedTask);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<RecurringTransactionTemplate, Guid>()).Returns(repoMock.Object);
        unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(transactionMock.Object);
        unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var loggerMock = new Mock<ILogger<RecurringTransactionTemplateService>>();
        var service = new RecurringTransactionTemplateService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.ToggleActiveStatusAsync(templateId, true);

        // Assert
        result.Should().BeTrue();
        template.IsActive.Should().BeTrue();
        template.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        repoMock.Verify(r => r.GetByIdAsync(templateId), Times.Once);
        repoMock.Verify(r => r.UpdateAsync(It.IsAny<RecurringTransactionTemplate>()), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ToggleActiveStatusAsync_ShouldReturnFalse_WhenTemplateNotFound()
    {
        // Arrange
        var templateId = Guid.NewGuid();

        var repoMock = new Mock<IBaseRepository<RecurringTransactionTemplate, Guid>>();
        repoMock.Setup(r => r.GetByIdAsync(templateId))
            .ReturnsAsync((RecurringTransactionTemplate?)null);

        var transactionMock = new Mock<IDbContextTransaction>();
        transactionMock.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        transactionMock.Setup(t => t.DisposeAsync()).Returns(ValueTask.CompletedTask);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<RecurringTransactionTemplate, Guid>()).Returns(repoMock.Object);
        unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(transactionMock.Object);

        var loggerMock = new Mock<ILogger<RecurringTransactionTemplateService>>();
        var service = new RecurringTransactionTemplateService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.ToggleActiveStatusAsync(templateId, true);

        // Assert
        result.Should().BeFalse();
        repoMock.Verify(r => r.GetByIdAsync(templateId), Times.Once);
        repoMock.Verify(r => r.UpdateAsync(It.IsAny<RecurringTransactionTemplate>()), Times.Never);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task ToggleActiveStatusAsync_ShouldToggleFromTrueToFalse_WhenTemplateIsCurrentlyActive()
    {
        // Arrange
        var templateId = Guid.NewGuid();
        var template = new RecurringTransactionTemplate
        {
            Id = templateId,
            UserId = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            Name = "Active Template",
            IsActive = true, // Currently active
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var repoMock = new Mock<IBaseRepository<RecurringTransactionTemplate, Guid>>();
        repoMock.Setup(r => r.GetByIdAsync(templateId))
            .ReturnsAsync(template);
        repoMock.Setup(r => r.UpdateAsync(It.IsAny<RecurringTransactionTemplate>()))
            .ReturnsAsync(1);

        var transactionMock = new Mock<IDbContextTransaction>();
        transactionMock.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        transactionMock.Setup(t => t.DisposeAsync()).Returns(ValueTask.CompletedTask);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<RecurringTransactionTemplate, Guid>()).Returns(repoMock.Object);
        unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(transactionMock.Object);
        unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var loggerMock = new Mock<ILogger<RecurringTransactionTemplateService>>();
        var service = new RecurringTransactionTemplateService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.ToggleActiveStatusAsync(templateId, false);

        // Assert
        result.Should().BeTrue();
        template.IsActive.Should().BeFalse(); // Should be toggled to false
        template.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        repoMock.Verify(r => r.GetByIdAsync(templateId), Times.Once);
        repoMock.Verify(r => r.UpdateAsync(It.IsAny<RecurringTransactionTemplate>()), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ToggleActiveStatusAsync_ShouldReturnFalse_WhenExceptionOccurs()
    {
        // Arrange
        var templateId = Guid.NewGuid();

        var repoMock = new Mock<IBaseRepository<RecurringTransactionTemplate, Guid>>();
        repoMock.Setup(r => r.GetByIdAsync(templateId))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        var transactionMock = new Mock<IDbContextTransaction>();
        transactionMock.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        transactionMock.Setup(t => t.DisposeAsync()).Returns(ValueTask.CompletedTask);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<RecurringTransactionTemplate, Guid>()).Returns(repoMock.Object);
        unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(transactionMock.Object);

        var loggerMock = new Mock<ILogger<RecurringTransactionTemplateService>>();
        var service = new RecurringTransactionTemplateService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.ToggleActiveStatusAsync(templateId, true);

        // Assert
        result.Should().BeFalse();
        repoMock.Verify(r => r.GetByIdAsync(templateId), Times.Once);
        transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);

        // Verify that error was logged
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error toggling active status for template")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}