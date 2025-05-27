using AutoMapper;
using Bogus;
using CoreFinance.Application.Mapper;
using CoreFinance.Domain;
using CoreFinance.Domain.Enums;
using MockQueryable;

namespace CoreFinance.Application.Tests.Helpers;

public static class TestHelpers
{
    public static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => { cfg.AddProfile<AutoMapperProfile>(); });
        return config.CreateMapper();
    }

    public static IQueryable<Account> GenerateFakeAccounts(int count)
    {
        var faker = new Faker<Account>()
            .RuleFor(a => a.Id, _ => Guid.NewGuid())
            .RuleFor(a => a.UserId, _ => Guid.NewGuid())
            .RuleFor(a => a.Name, f => f.Finance.AccountName())
            .RuleFor(a => a.Type, f => f.PickRandom<AccountType>())
            .RuleFor(a => a.Currency, f => f.Finance.Currency().Code)
            .RuleFor(a => a.InitialBalance, f => f.Finance.Amount())
            .RuleFor(a => a.CurrentBalance, f => f.Finance.Amount())
            .RuleFor(a => a.IsActive, f => f.Random.Bool());
        return faker.Generate(count).AsQueryable().BuildMock();
    }

    public static IQueryable<ExpectedTransaction> GenerateFakeExpectedTransactions(int count)
    {
        var faker = new Faker<ExpectedTransaction>()
            .RuleFor(et => et.Id, _ => Guid.NewGuid())
            .RuleFor(et => et.UserId, _ => Guid.NewGuid())
            .RuleFor(et => et.AccountId, _ => Guid.NewGuid())
            .RuleFor(et => et.RecurringTransactionTemplateId, _ => Guid.NewGuid())
            .RuleFor(et => et.ExpectedDate, f => f.Date.Future())
            .RuleFor(et => et.ExpectedAmount, f => f.Finance.Amount(10))
            .RuleFor(et => et.OriginalAmount, (f, et) => f.Random.Bool() ? et.ExpectedAmount : null)
            .RuleFor(et => et.Description, f => f.Lorem.Sentence())
            .RuleFor(et => et.TransactionType, f => f.PickRandom<RecurringTransactionType>())
            .RuleFor(et => et.Category, f => f.Commerce.Categories(1).First())
            .RuleFor(et => et.Status, f => f.PickRandom<ExpectedTransactionStatus>())
            .RuleFor(et => et.IsAdjusted, f => f.Random.Bool())
            .RuleFor(et => et.AdjustmentReason, f => f.Random.Bool() ? f.Lorem.Sentence() : null)
            .RuleFor(et => et.ActualTransactionId, f => f.Random.Bool() ? Guid.NewGuid() : null)
            .RuleFor(et => et.GeneratedAt, f => f.Date.Past())
            .RuleFor(et => et.ProcessedAt, f => f.Random.Bool() ? f.Date.Recent() : null)
            .RuleFor(et => et.CreatedAt, f => f.Date.Past(2))
            .RuleFor(et => et.UpdatedAt, f => f.Date.Recent());
        var result = faker.Generate(count);
        return result.AsQueryable().BuildMock();
    }

    public static IQueryable<RecurringTransactionTemplate> GenerateFakeRecurringTransactionTemplates(int count)
    {
        var faker = new Faker<RecurringTransactionTemplate>()
            .RuleFor(rt => rt.Id, _ => Guid.NewGuid())
            .RuleFor(rt => rt.UserId, _ => Guid.NewGuid())
            .RuleFor(rt => rt.AccountId, _ => Guid.NewGuid())
            .RuleFor(rt => rt.Name, f => f.Commerce.ProductName())
            .RuleFor(rt => rt.Description, f => f.Lorem.Sentence())
            .RuleFor(rt => rt.Amount, f => f.Finance.Amount(10))
            .RuleFor(rt => rt.TransactionType, f => f.PickRandom<RecurringTransactionType>())
            .RuleFor(rt => rt.Category, f => f.Commerce.Categories(1).First())
            .RuleFor(rt => rt.Frequency, f => f.PickRandom<RecurrenceFrequency>())
            .RuleFor(rt => rt.CustomIntervalDays, f => f.Random.Bool() ? f.Random.Int(1, 30) : null)
            .RuleFor(rt => rt.StartDate, f => f.Date.Past())
            .RuleFor(rt => rt.EndDate, f => f.Random.Bool() ? f.Date.Future() : null)
            .RuleFor(rt => rt.NextExecutionDate, f => f.Date.Future())
            .RuleFor(rt => rt.IsActive, f => f.Random.Bool())
            .RuleFor(rt => rt.AutoGenerate, f => f.Random.Bool())
            .RuleFor(rt => rt.DaysInAdvance, f => f.Random.Int(7, 90))
            .RuleFor(rt => rt.CreatedAt, f => f.Date.Past(2))
            .RuleFor(rt => rt.UpdatedAt, f => f.Date.Recent());
        return faker.Generate(count).AsQueryable().BuildMock();
    }

    public static IQueryable<Transaction> GenerateFakeTransactions(int count)
    {
        var faker = new Faker<Transaction>()
            .RuleFor(t => t.Id, _ => Guid.NewGuid())
            .RuleFor(t => t.UserId, _ => Guid.NewGuid())
            .RuleFor(t => t.AccountId, _ => Guid.NewGuid())
            .RuleFor(t => t.TransactionDate, f => f.Date.Past())
            .RuleFor(t => t.RevenueAmount, f => f.Finance.Amount(0, 5000))
            .RuleFor(t => t.SpentAmount, f => f.Finance.Amount(0, 3000))
            .RuleFor(t => t.Description, f => f.Lorem.Sentence())
            .RuleFor(t => t.Balance, f => f.Finance.Amount(100, 10000))
            .RuleFor(t => t.BalanceCompare, f => f.Random.Bool() ? f.Finance.Amount(100, 10000) : null)
            .RuleFor(t => t.AvailableLimit, f => f.Random.Bool() ? f.Finance.Amount(1000, 50000) : null)
            .RuleFor(t => t.AvailableLimitCompare, f => f.Random.Bool() ? f.Finance.Amount(1000, 50000) : null)
            .RuleFor(t => t.TransactionCode, f => f.Random.AlphaNumeric(10))
            .RuleFor(t => t.SyncMisa, f => f.Random.Bool())
            .RuleFor(t => t.SyncSms, f => f.Random.Bool())
            .RuleFor(t => t.Vn, f => f.Random.Bool())
            .RuleFor(t => t.CategorySummary, f => f.Commerce.Categories(1).First())
            .RuleFor(t => t.Note, f => f.Random.Bool() ? f.Lorem.Sentence() : null)
            .RuleFor(t => t.ImportFrom, f => f.Random.Bool() ? f.Company.CompanyName() : null)
            .RuleFor(t => t.IncreaseCreditLimit, f => f.Random.Bool() ? f.Finance.Amount() : null)
            .RuleFor(t => t.UsedPercent, f => f.Random.Bool() ? f.Random.Decimal(0, 100) : null)
            .RuleFor(t => t.CategoryType, f => f.PickRandom<CategoryType>())
            .RuleFor(t => t.Group, f => f.Random.Bool() ? f.Commerce.Department() : null)
            .RuleFor(t => t.CreatedAt, f => f.Date.Past(2))
            .RuleFor(t => t.UpdatedAt, f => f.Date.Recent())
            .RuleFor(t => t.CreateAt, f => f.Date.Past(2))
            .RuleFor(t => t.UpdateAt, f => f.Date.Recent())
            .RuleFor(t => t.CreateBy, f => f.Person.UserName)
            .RuleFor(t => t.UpdateBy, f => f.Person.UserName);
        return faker.Generate(count).AsQueryable().BuildMock();
    }
}