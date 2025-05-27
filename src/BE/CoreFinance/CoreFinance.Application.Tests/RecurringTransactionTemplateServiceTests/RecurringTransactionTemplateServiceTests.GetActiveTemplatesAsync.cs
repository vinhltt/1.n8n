using CoreFinance.Application.Services;
using CoreFinance.Domain;
using CoreFinance.Domain.BaseRepositories;
using CoreFinance.Domain.UnitOfWorks;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using FluentAssertions;

namespace CoreFinance.Application.Tests.RecurringTransactionTemplateServiceTests;

// Tests for the GetActiveTemplatesAsync method of RecurringTransactionTemplateService
public partial class RecurringTransactionTemplateServiceTests
{
    [Fact]
    public async Task GetActiveTemplatesAsync_ShouldReturnOnlyActiveTemplates_ForSpecificUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var templates = new List<RecurringTransactionTemplate>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Active Template 1",
                IsActive = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Inactive Template",
                IsActive = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Active Template 2",
                IsActive = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = otherUserId,
                Name = "Other User Active Template",
                IsActive = true
            }
        };

        var templatesMock = templates.AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<RecurringTransactionTemplate, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities())
            .Returns(templatesMock);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<RecurringTransactionTemplate, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<RecurringTransactionTemplateService>>();
        var service = new RecurringTransactionTemplateService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetActiveTemplatesAsync(userId);

        // Assert
        var recurringTransactionTemplateViewModels = result.ToList();
        recurringTransactionTemplateViewModels.Should().NotBeNull();
        recurringTransactionTemplateViewModels.Should().HaveCount(2);
        recurringTransactionTemplateViewModels.Should().OnlyContain(t => t.IsActive);
        recurringTransactionTemplateViewModels.Should().OnlyContain(t => t.UserId == userId);
        recurringTransactionTemplateViewModels.Select(t => t.Name).Should()
            .Contain(new[] { "Active Template 1", "Active Template 2" });
    }

    [Fact]
    public async Task GetActiveTemplatesAsync_ShouldReturnEmptyList_WhenNoActiveTemplatesExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var templates = new List<RecurringTransactionTemplate>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Inactive Template 1",
                IsActive = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Inactive Template 2",
                IsActive = false
            }
        };

        var templatesMock = templates.AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<RecurringTransactionTemplate, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities())
            .Returns(templatesMock);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<RecurringTransactionTemplate, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<RecurringTransactionTemplateService>>();
        var service = new RecurringTransactionTemplateService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetActiveTemplatesAsync(userId);

        // Assert
        var recurringTransactionTemplateViewModels = result.ToList();
        recurringTransactionTemplateViewModels.Should().NotBeNull();
        recurringTransactionTemplateViewModels.Should().BeEmpty();
    }

    [Fact]
    public async Task GetActiveTemplatesAsync_ShouldReturnEmptyList_WhenUserHasNoTemplates()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var templates = new List<RecurringTransactionTemplate>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = otherUserId,
                Name = "Other User Template",
                IsActive = true
            }
        };

        var templatesMock = templates.AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<RecurringTransactionTemplate, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities())
            .Returns(templatesMock);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<RecurringTransactionTemplate, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<RecurringTransactionTemplateService>>();
        var service = new RecurringTransactionTemplateService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetActiveTemplatesAsync(userId);

        // Assert
        var recurringTransactionTemplateViewModels = result.ToList();
        recurringTransactionTemplateViewModels.Should().NotBeNull();
        recurringTransactionTemplateViewModels.Should().BeEmpty();
    }

    [Fact]
    public async Task GetActiveTemplatesAsync_ShouldReturnEmptyList_WhenNoTemplatesExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var templatesMock = new List<RecurringTransactionTemplate>().AsQueryable().BuildMock();

        var repoMock = new Mock<IBaseRepository<RecurringTransactionTemplate, Guid>>();
        repoMock.Setup(r => r.GetNoTrackingEntities())
            .Returns(templatesMock);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<RecurringTransactionTemplate, Guid>()).Returns(repoMock.Object);

        var loggerMock = new Mock<ILogger<RecurringTransactionTemplateService>>();
        var service = new RecurringTransactionTemplateService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetActiveTemplatesAsync(userId);

        // Assert
        var recurringTransactionTemplateViewModels = result.ToList();
        recurringTransactionTemplateViewModels.Should().NotBeNull();
        recurringTransactionTemplateViewModels.Should().BeEmpty();
    }
}