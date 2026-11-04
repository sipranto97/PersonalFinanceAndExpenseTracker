using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceAndExpenseTracker.Domain.Entities;

namespace PersonalFinanceAndExpenseTracker.Domain.Configs
{
    public class SavingsGoalConfig : IEntityTypeConfiguration<SavingsGoal>
    {
        public void Configure(EntityTypeBuilder<SavingsGoal> entity)
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(t => t.TargetAmount)
                .IsRequired();

            entity.Property(t => t.TargetDate)
                .IsRequired();

            entity.Property(t => t.Status)
                .IsRequired();

            entity.Property(t => t.CreatedAt)
                .IsRequired();

            entity.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId);

            entity.HasOne(t => t.SettlementIncome)
                .WithMany()
                .HasForeignKey(t => t.SettlementIncomeId)
                .IsRequired(false);
        }
    }
}
