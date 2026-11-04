using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceAndExpenseTracker.Domain.Entities;

namespace PersonalFinanceAndExpenseTracker.Domain.Configs
{
    public class SavingsGoalContributionConfig : IEntityTypeConfiguration<SavingsGoalContribution>
    {
        public void Configure(EntityTypeBuilder<SavingsGoalContribution> entity)
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Amount)
                .IsRequired();

            entity.Property(t => t.ContributedOn)
                .IsRequired();

            entity.HasOne(t => t.SavingsGoal)
                .WithMany(g => g.Contributions)
                .HasForeignKey(t => t.SavingsGoalId);

            entity.HasOne(t => t.Expense)
                .WithMany()
                .HasForeignKey(t => t.ExpenseId);
        }
    }
}
