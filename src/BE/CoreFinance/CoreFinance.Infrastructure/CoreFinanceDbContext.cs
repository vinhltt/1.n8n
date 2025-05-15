using Microsoft.EntityFrameworkCore;
using CoreFinance.Domain;
using Microsoft.Extensions.Configuration;
#pragma warning disable CS8618, CS9264

namespace CoreFinance.Infrastructure;

public class CoreFinanceDbContext: DbContext
{
    private readonly IConfiguration _configuration;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }
}