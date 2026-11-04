using Microsoft.EntityFrameworkCore;
using PersonalFinanceAndExpenseTracker.Domain.Configs;
using PersonalFinanceAndExpenseTracker.Domain.Entities;

namespace PersonalFinanceAndExpenseTracker.Domain
{
    public class ExpenseTrackerDbContext : DbContext
    {
        public ExpenseTrackerDbContext()
        {

        }
        public ExpenseTrackerDbContext(DbContextOptions<ExpenseTrackerDbContext> options) : base(options)
        {
        }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<RecurringIncome> RecurringIncomes { get; set; }
        public DbSet<IncomeShareLink> IncomeShareLinks { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<RecurringExpense> RecurringExpenses { get; set; }
        public DbSet<ExpenseShareLink> ExpenseShareLinks { get; set; }
        public DbSet<SavingsGoal> SavingsGoals { get; set; }
        public DbSet<SavingsGoalContribution> SavingsGoalContributions { get; set; }
        public DbSet<ChartShareLink> ChartShareLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.ApplyConfiguration(new UserAccountConfig());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfig());
            modelBuilder.ApplyConfiguration(new CategoryConfig());
            modelBuilder.ApplyConfiguration(new IncomeConfig());
            modelBuilder.ApplyConfiguration(new RecurringIncomeConfig());
            modelBuilder.ApplyConfiguration(new IncomeShareLinkConfig());
            modelBuilder.ApplyConfiguration(new ExpenseConfig());
            modelBuilder.ApplyConfiguration(new RecurringExpenseConfig());
            modelBuilder.ApplyConfiguration(new ExpenseShareLinkConfig());
            modelBuilder.ApplyConfiguration(new SavingsGoalConfig());
            modelBuilder.ApplyConfiguration(new SavingsGoalContributionConfig());
            modelBuilder.ApplyConfiguration(new ChartShareLinkConfig());
        }
    }
}
