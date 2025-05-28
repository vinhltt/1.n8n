using Microsoft.EntityFrameworkCore;
using CoreFinance.Domain;
using Microsoft.Extensions.Configuration;

#pragma warning disable CS8618, CS9264

namespace CoreFinance.Infrastructure;

public class CoreFinanceDbContext : DbContext
{
    // ReSharper disable once NotAccessedField.Local
    private readonly IConfiguration _configuration;
    public const string DEFAULT_CONNECTION_STRING = "CoreFinanceDb";

    public CoreFinanceDbContext()
    {
    }

    public CoreFinanceDbContext(DbContextOptions<CoreFinanceDbContext> options,
        IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<ExpectedTransaction> ExpectedTransactions { get; set; }
    public DbSet<RecurringTransactionTemplate> RecurringTransactionTemplates { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }
}