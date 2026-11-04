using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceAndExpenseTracker.Domain.Entities;

namespace PersonalFinanceAndExpenseTracker.Domain.Configs
{
    public class RefreshTokenConfig: IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> entity)
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.UserId)
                .IsRequired();

            entity.Property(t => t.TokenId)
                .IsRequired();

            entity.Property(t => t.RefreshUserToken);

            entity.Property(t => t.ExpiresAt)
                .IsRequired();

            entity.HasOne(t => t.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(t => t.UserId);
        }
    }
}
