using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceAndExpenseTracker.Domain.Entities;

namespace PersonalFinanceAndExpenseTracker.Domain.Configs
{
    public class RecurringIncomeConfig : IEntityTypeConfiguration<RecurringIncome>
    {
        public void Configure(EntityTypeBuilder<RecurringIncome> entity)
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.UserId);

            entity.Property(t => t.Amount)
                .IsRequired();

            entity.Property(t => t.CategoryId)
                .IsRequired();

            entity.Property(t => t.DayOfMonth)
                .IsRequired();

            entity.Property(t => t.IsActive)
                .IsRequired();

            entity.Property(t => t.CreatedAt)
                .IsRequired();

            entity.HasOne(t => t.Category)
                .WithMany()
                .HasForeignKey(t => t.CategoryId);

            entity.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId);
        }
    }
}
