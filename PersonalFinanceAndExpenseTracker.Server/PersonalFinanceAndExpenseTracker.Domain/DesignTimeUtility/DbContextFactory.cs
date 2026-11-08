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

            // The file only holds host/database. Credentials come from the
            // DB_PE_USERNAME / DB_PE_PASSWORD environment variables, exactly as
            // they do for the running app, so reuse the same builder.
            var appStyleConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    // "Default" in Domain/appsettings.json (Package Manager Console), or the
                    // API's own key when the tools run with the WebAPI project's settings.
                    ["ConnectionStrings:DefaultConnectionString"] =
                        configuration["Default"] ?? configuration["ConnectionStrings:DefaultConnectionString"]
                })
                .Build();

            return DbConnectionString.Build(appStyleConfiguration);
        }
    }
}
