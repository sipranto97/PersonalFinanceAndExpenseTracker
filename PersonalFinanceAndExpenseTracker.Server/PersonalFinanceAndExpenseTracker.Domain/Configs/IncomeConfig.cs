using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceAndExpenseTracker.Domain.Entities;

namespace PersonalFinanceAndExpenseTracker.Domain.Configs
{
    public class IncomeConfig : IEntityTypeConfiguration<Income>
    {
        public void Configure(EntityTypeBuilder<Income> entity)
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.UserId);
                    

            entity.Property(t => t.Amount)
                .IsRequired();

            entity.Property(t => t.CategoryId)
                .IsRequired();

            entity.Property(t => t.TrxDate)
                .IsRequired();

            entity.HasOne(t => t.Category)
                .WithMany(u => u.Incomes)
                .HasForeignKey(t => t.CategoryId);

            entity.HasOne(t => t.RecurringIncome)
                .WithMany(r => r.GeneratedIncomes)
                .HasForeignKey(t => t.RecurringIncomeId)
                .IsRequired(false);
        }
    }
}
