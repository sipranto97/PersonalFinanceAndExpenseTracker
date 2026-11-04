using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace PersonalFinanceAndExpenseTracker.Domain.DesignTimeUtility
{
    public class DbContextFactory : IDesignTimeDbContextFactory<ExpenseTrackerDbContext>
    {
        public ExpenseTrackerDbContext CreateDbContext(string[] arg)
        {
            var builder = new DbContextOptionsBuilder<ExpenseTrackerDbContext>();
            builder.UseNpgsql(GetDbConnectionString());

            return new ExpenseTrackerDbContext(builder.Options);
        }

        private string GetDbConnectionString()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json")
                      .Build();

            return configuration["Default"]; ;
        }
    }
}
