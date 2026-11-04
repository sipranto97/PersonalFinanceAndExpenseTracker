using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceAndExpenseTracker.Domain.Entities;

namespace PersonalFinanceAndExpenseTracker.Domain.Configs
{
    public class ExpenseConfig : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> entity)
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
                .WithMany(u => u.Expenses)
                .HasForeignKey(t => t.CategoryId);

            entity.HasOne(t => t.RecurringExpense)
                .WithMany(r => r.GeneratedExpenses)
                .HasForeignKey(t => t.RecurringExpenseId)
                .IsRequired(false);
        }
    }
}
