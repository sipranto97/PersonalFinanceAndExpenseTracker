using Microsoft.Extensions.Configuration;
using Npgsql;

namespace PersonalFinanceAndExpenseTracker.Domain
{
    public static class DbConnectionString
    {
        public const string UsernameKey = "DB_PE_USERNAME";
        public const string PasswordKey = "DB_PE_PASSWORD";

        // Host/port/database come from appsettings; credentials come from the
        // DB_PE_USERNAME / DB_PE_PASSWORD environment variables (or user secrets).
        public static string Build(IConfiguration configuration)
        {
            var baseConnectionString = configuration.GetConnectionString("DefaultConnectionString");
            var builder = new NpgsqlConnectionStringBuilder(baseConnectionString);

            var username = configuration[UsernameKey] ?? Environment.GetEnvironmentVariable(UsernameKey);
            var password = configuration[PasswordKey] ?? Environment.GetEnvironmentVariable(PasswordKey);

            if (!string.IsNullOrEmpty(username))
            {
                builder.Username = username;
            }

            if (!string.IsNullOrEmpty(password))
            {
                builder.Password = password;
            }

            if (string.IsNullOrEmpty(builder.Username) || string.IsNullOrEmpty(builder.Password))
            {
                throw new InvalidOperationException(
                    $"Database credentials are missing. Set the {UsernameKey} and {PasswordKey} environment variables.");
            }

            return builder.ConnectionString;
        }
    }
}
