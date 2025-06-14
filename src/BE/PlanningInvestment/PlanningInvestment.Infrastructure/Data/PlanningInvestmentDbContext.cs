using Microsoft.EntityFrameworkCore;
using PlanningInvestment.Domain.Entities;
using PlanningInvestment.Infrastructure.Data;
using Shared.Contracts.BaseEfModels;

namespace PlanningInvestment.Infrastructure.Data;

/// <summary>
/// Database context for Planning & Investment bounded context (EN)<br/>
/// Context cơ sở dữ liệu cho bounded context Lập kế hoạch & Đầu tư (VI)
/// </summary>
public class PlanningInvestmentDbContext : DbContext
{
    public PlanningInvestmentDbContext(DbContextOptions<PlanningInvestmentDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Goals table (EN)<br/>
    /// Bảng mục tiêu (VI)
    /// </summary>
    public DbSet<Goal> Goals { get; set; } = null!;

    /// <summary>
    /// Goal transactions table (EN)<br/>
    /// Bảng giao dịch mục tiêu (VI)
    /// </summary>
    public DbSet<GoalTransaction> GoalTransactions { get; set; } = null!;

    /// <summary>
    /// Investments table (EN)<br/>
    /// Bảng đầu tư (VI)
    /// </summary>
    public DbSet<Investment> Investments { get; set; } = null!;

    /// <summary>
    /// Investment transactions table (EN)<br/>
    /// Bảng giao dịch đầu tư (VI)
    /// </summary>
    public DbSet<InvestmentTransaction> InvestmentTransactions { get; set; } = null!;

    /// <summary>
    /// Debts table (EN)<br/>
    /// Bảng nợ (VI)
    /// </summary>
    public DbSet<Debt> Debts { get; set; } = null!;

    /// <summary>
    /// Debt payments table (EN)<br/>
    /// Bảng thanh toán nợ (VI)
    /// </summary>
    public DbSet<DebtPayment> DebtPayments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Goal entity
        modelBuilder.Entity<Goal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.TargetAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CurrentAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Priority).HasColumnType("decimal(5,2)");
            entity.Property(e => e.MonthlyContribution).HasColumnType("decimal(18,2)");
            
            // Configure relationships
            entity.HasMany(e => e.GoalTransactions)
                  .WithOne(gt => gt.Goal)
                  .HasForeignKey(gt => gt.GoalId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure GoalTransaction entity
        modelBuilder.Entity<GoalTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // Configure Investment entity
        modelBuilder.Entity<Investment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Symbol).HasMaxLength(20);
            entity.Property(e => e.InitialAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CurrentValue).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ReturnRate).HasColumnType("decimal(5,2)");
            entity.Property(e => e.Platform).HasMaxLength(100);

            // Configure relationships
            entity.HasMany(e => e.InvestmentTransactions)
                  .WithOne(it => it.Investment)
                  .HasForeignKey(it => it.InvestmentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure InvestmentTransaction entity
        modelBuilder.Entity<InvestmentTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18,4)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // Configure Debt entity
        modelBuilder.Entity<Debt>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.OriginalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CurrentBalance).HasColumnType("decimal(18,2)");
            entity.Property(e => e.InterestRate).HasColumnType("decimal(5,2)");
            entity.Property(e => e.MinimumPayment).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Creditor).HasMaxLength(200);
            entity.Property(e => e.AccountNumber).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            // Configure relationships
            entity.HasMany(e => e.DebtPayments)
                  .WithOne(dp => dp.Debt)
                  .HasForeignKey(dp => dp.DebtId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure DebtPayment entity
        modelBuilder.Entity<DebtPayment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PrincipalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.InterestAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Description).HasMaxLength(500);
        });        // Apply audit fields configuration for all BaseEntity types
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity<Guid>).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType, builder =>
                {
                    builder.Property("CreateAt").IsRequired();
                    builder.Property("UpdateAt");
                    builder.Property("CreateBy").HasMaxLength(100);
                    builder.Property("UpdateBy").HasMaxLength(100);
                    builder.Property("Deleted").HasDefaultValue("false").HasMaxLength(10);
                });
            }
        }
    }
}
