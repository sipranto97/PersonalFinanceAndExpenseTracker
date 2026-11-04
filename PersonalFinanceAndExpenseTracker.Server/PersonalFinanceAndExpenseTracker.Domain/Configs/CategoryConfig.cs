using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceAndExpenseTracker.Domain.Entities;

namespace PersonalFinanceAndExpenseTracker.Domain.Configs
{
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> entity)
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.UserId);

            entity.Property(t => t.Name)
                .IsRequired();

            entity.Property(t => t.IsIncome)
                .IsRequired(); 

            entity.HasOne(t => t.User)
                .WithMany(u => u.Categories)
                .HasForeignKey(t => t.UserId);
        }
    }
}
