using CoreFinance.Application.Services;
using CoreFinance.Domain;
using CoreFinance.Domain.BaseRepositories;
using CoreFinance.Domain.Enums;
using CoreFinance.Domain.UnitOfWorks;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace CoreFinance.Application.Tests.RecurringTransactionTemplateServiceTests;

// Tests for the CalculateNextExecutionDateAsync method of RecurringTransactionTemplateService
public partial class RecurringTransactionTemplateServiceTests
{
    [Fact]
    public async Task CalculateNextExecutionDateAsync_ShouldReturnCorrectDate_WhenTemplateExists()
    {
        // Arrange
        var templateId = Guid.NewGuid();
        var nextExecutionDate = DateTime.UtcNow.AddDays(7);
        var frequency = RecurrenceFrequency.Weekly;
        var customIntervalDays = 7;

        var template = new RecurringTransactionTemplate
        {
            Id = templateId,
            NextExecutionDate = nextExecutionDate,
            Frequency = frequency,
            CustomIntervalDays = customIntervalDays
        };

        var templateRepoMock = new Mock<IBaseRepository<RecurringTransactionTemplate, Guid>>();
        templateRepoMock.Setup(r => r.GetByIdAsync(templateId))
            .ReturnsAsync(template);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<RecurringTransactionTemplate, Guid>())
            .Returns(templateRepoMock.Object);

        var loggerMock = new Mock<ILogger<RecurringTransactionTemplateService>>();
        var service = new RecurringTransactionTemplateService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        var expectedNextDate = nextExecutionDate.AddDays(7); // Weekly frequency

        // Act
        var result = await service.CalculateNextExecutionDateAsync(templateId);

        // Assert
        result.Should().Be(expectedNextDate);
        templateRepoMock.Verify(r => r.GetByIdAsync(templateId), Times.Once);
    }

    [Fact]
    public async Task CalculateNextExecutionDateAsync_ShouldThrowArgumentException_WhenTemplateNotFound()
    {
        // Arrange
        var templateId = Guid.NewGuid();

        var templateRepoMock = new Mock<IBaseRepository<RecurringTransactionTemplate, Guid>>();
        templateRepoMock.Setup(r => r.GetByIdAsync(templateId))
            .ReturnsAsync((RecurringTransactionTemplate?)null);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<RecurringTransactionTemplate, Guid>())
            .Returns(templateRepoMock.Object);

        var loggerMock = new Mock<ILogger<RecurringTransactionTemplateService>>();
        var service = new RecurringTransactionTemplateService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        // Act & Assert
        var act = async () => await service.CalculateNextExecutionDateAsync(templateId);
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Template not found*")
            .WithParameterName("templateId");

        templateRepoMock.Verify(r => r.GetByIdAsync(templateId), Times.Once);
    }

    [Theory]
    [InlineData(RecurrenceFrequency.Daily)]
    [InlineData(RecurrenceFrequency.Weekly)]
    [InlineData(RecurrenceFrequency.Biweekly)]
    [InlineData(RecurrenceFrequency.Monthly)]
    [InlineData(RecurrenceFrequency.Quarterly)]
    [InlineData(RecurrenceFrequency.SemiAnnually)]
    [InlineData(RecurrenceFrequency.Annually)]
    public async Task CalculateNextExecutionDateAsync_ShouldCalculateCorrectly_ForDifferentFrequencies(
        RecurrenceFrequency frequency)
    {
        // Arrange
        var templateId = Guid.NewGuid();
        var baseDate = new DateTime(2024, 1, 1); // Fixed date for consistent testing

        var template = new RecurringTransactionTemplate
        {
            Id = templateId,
            NextExecutionDate = baseDate,
            Frequency = frequency,
            CustomIntervalDays = null
        };

        var templateRepoMock = new Mock<IBaseRepository<RecurringTransactionTemplate, Guid>>();
        templateRepoMock.Setup(r => r.GetByIdAsync(templateId))
            .ReturnsAsync(template);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<RecurringTransactionTemplate, Guid>())
            .Returns(templateRepoMock.Object);

        var loggerMock = new Mock<ILogger<RecurringTransactionTemplateService>>();
        var service = new RecurringTransactionTemplateService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        DateTime expectedDate;
        switch (frequency)
        {
            case RecurrenceFrequency.Daily:
                expectedDate = baseDate.AddDays(1);
                break;
            case RecurrenceFrequency.Weekly:
                expectedDate = baseDate.AddDays(7);
                break;
            case RecurrenceFrequency.Biweekly:
                expectedDate = baseDate.AddDays(14);
                break;
            case RecurrenceFrequency.Monthly:
                expectedDate = baseDate.AddMonths(1);
                break;
            case RecurrenceFrequency.Quarterly:
                expectedDate = baseDate.AddMonths(3);
                break;
            case RecurrenceFrequency.SemiAnnually:
                expectedDate = baseDate.AddMonths(6);
                break;
            case RecurrenceFrequency.Annually:
                expectedDate = baseDate.AddYears(1);
                break;
            default:
                expectedDate = baseDate.AddDays(1);
                break;
        }

        // Act
        var result = await service.CalculateNextExecutionDateAsync(templateId);

        // Assert
        result.Should().Be(expectedDate);
    }

    [Fact]
    public async Task CalculateNextExecutionDateAsync_ShouldUseCustomInterval_WhenFrequencyIsCustom()
    {
        // Arrange
        var templateId = Guid.NewGuid();
        var baseDate = new DateTime(2024, 1, 1);
        var customInterval = 15;

        var template = new RecurringTransactionTemplate
        {
            Id = templateId,
            NextExecutionDate = baseDate,
            Frequency = RecurrenceFrequency.Custom,
            CustomIntervalDays = customInterval
        };

        var templateRepoMock = new Mock<IBaseRepository<RecurringTransactionTemplate, Guid>>();
        templateRepoMock.Setup(r => r.GetByIdAsync(templateId))
            .ReturnsAsync(template);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<RecurringTransactionTemplate, Guid>())
            .Returns(templateRepoMock.Object);

        var loggerMock = new Mock<ILogger<RecurringTransactionTemplateService>>();
        var service = new RecurringTransactionTemplateService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        var expectedDate = baseDate.AddDays(customInterval);

        // Act
        var result = await service.CalculateNextExecutionDateAsync(templateId);

        // Assert
        result.Should().Be(expectedDate);
    }

    [Fact]
    public async Task CalculateNextExecutionDateAsync_ShouldDefaultToOneDay_WhenCustomIntervalIsNull()
    {
        // Arrange
        var templateId = Guid.NewGuid();
        var baseDate = new DateTime(2024, 1, 1);

        var template = new RecurringTransactionTemplate
        {
            Id = templateId,
            NextExecutionDate = baseDate,
            Frequency = RecurrenceFrequency.Custom,
            CustomIntervalDays = null
        };

        var templateRepoMock = new Mock<IBaseRepository<RecurringTransactionTemplate, Guid>>();
        templateRepoMock.Setup(r => r.GetByIdAsync(templateId))
            .ReturnsAsync(template);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<RecurringTransactionTemplate, Guid>())
            .Returns(templateRepoMock.Object);

        var loggerMock = new Mock<ILogger<RecurringTransactionTemplateService>>();
        var service = new RecurringTransactionTemplateService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        var expectedDate = baseDate.AddDays(1); // Default to 1 day when custom interval is null

        // Act
        var result = await service.CalculateNextExecutionDateAsync(templateId);

        // Assert
        result.Should().Be(expectedDate);
    }

    [Fact]
    public async Task CalculateNextExecutionDateAsync_ShouldHandleMonthlyFrequency_WithDifferentMonthLengths()
    {
        // Arrange
        var templateId = Guid.NewGuid();
        var baseDate = new DateTime(2024, 1, 31); // January 31st

        var template = new RecurringTransactionTemplate
        {
            Id = templateId,
            NextExecutionDate = baseDate,
            Frequency = RecurrenceFrequency.Monthly,
            CustomIntervalDays = null
        };

        var templateRepoMock = new Mock<IBaseRepository<RecurringTransactionTemplate, Guid>>();
        templateRepoMock.Setup(r => r.GetByIdAsync(templateId))
            .ReturnsAsync(template);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Repository<RecurringTransactionTemplate, Guid>())
            .Returns(templateRepoMock.Object);

        var loggerMock = new Mock<ILogger<RecurringTransactionTemplateService>>();
        var service = new RecurringTransactionTemplateService(_mapper, unitOfWorkMock.Object, loggerMock.Object);

        var expectedDate = new DateTime(2024, 2, 29); // February 29th (2024 is leap year)

        // Act
        var result = await service.CalculateNextExecutionDateAsync(templateId);

        // Assert
        result.Should().Be(expectedDate);
    }
}