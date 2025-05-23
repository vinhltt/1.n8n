using Bogus;
using CoreFinance.Domain;
using MockQueryable;

namespace CoreFinance.Application.Tests.Helpers;

public static class TestHelpers
{
    public static IQueryable<Account> GenerateFakeAccounts(int count)
    {
        var faker = new Faker<Account>()
            .RuleFor(a => a.Id, _ => Guid.NewGuid())
            .RuleFor(a => a.UserId, _ => Guid.NewGuid())
            .RuleFor(a => a.Name, f => f.Company.CompanyName())
            .RuleFor(a => a.Type, f => f.PickRandom<AccountType>())
            .RuleFor(a => a.CardNumber, f => f.Finance.CreditCardNumber())
            .RuleFor(a => a.Currency, f => f.Finance.Currency().Code)
            .RuleFor(a => a.InitialBalance, f => f.Finance.Amount(0, 10000))
            .RuleFor(a => a.CurrentBalance, (f, a) => a.InitialBalance + f.Finance.Amount(-1000, 1000))
            .RuleFor(a => a.AvailableLimit, f => f.Random.Bool() ? f.Finance.Amount(0, 5000) : null)
            .RuleFor(a => a.CreatedAt, f => f.Date.Past(2))
            .RuleFor(a => a.UpdatedAt, f => f.Date.Recent())
            .RuleFor(a => a.IsActive, f => f.Random.Bool());
        return faker.Generate(count).AsQueryable().BuildMock();
    }
}