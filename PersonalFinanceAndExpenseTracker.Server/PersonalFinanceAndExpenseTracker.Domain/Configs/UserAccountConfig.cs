using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceAndExpenseTracker.Domain.Entities;

namespace PersonalFinanceAndExpenseTracker.Domain.Configs
{
    public class UserAccountConfig : IEntityTypeConfiguration<UserAccount>
    {
        public void Configure(EntityTypeBuilder<UserAccount> entity)
        {
            entity.HasKey(u => u.Id);

            // Properties
            entity.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(u => u.Password);

            entity.Property(u => u.GoogleId);

            entity.Property(u => u.FullName);

            entity.Property(u => u.Role);

            entity.HasIndex(u => u.Username);

        }
    }
}
