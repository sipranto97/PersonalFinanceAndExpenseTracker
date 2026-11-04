using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceAndExpenseTracker.Domain.Entities;

namespace PersonalFinanceAndExpenseTracker.Domain.Configs
{
    public class ChartShareLinkConfig : IEntityTypeConfiguration<ChartShareLink>
    {
        public void Configure(EntityTypeBuilder<ChartShareLink> entity)
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.UserId);

            entity.Property(t => t.Token)
                .IsRequired()
                .HasMaxLength(32);

            entity.HasIndex(t => t.Token)
                .IsUnique();

            entity.Property(t => t.ChartType)
                .IsRequired();

            entity.Property(t => t.StartDate)
                .IsRequired();

            entity.Property(t => t.EndDate)
                .IsRequired();

            entity.Property(t => t.CreatedAt)
                .IsRequired();

            entity.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId);
        }
    }
}
