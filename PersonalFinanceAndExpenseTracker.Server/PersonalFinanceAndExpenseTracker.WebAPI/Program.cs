using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PersonalFinanceAndExpenseTracker.Domain;
using PersonalFinanceAndExpenseTracker.Services;
using PersonalFinanceAndExpenseTracker.Services.Authentication;
using PersonalFinanceAndExpenseTracker.Services.Exceptions;
using Serilog;
using System.Net;
using System.Text;
using System.Threading.RateLimiting;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine(context.HostingEnvironment.ContentRootPath, "logs", "log-.txt"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

var connectionString = DbConnectionString.Build(builder.Configuration);

builder.Services.AddDbContext<ExpenseTrackerDbContext>(options =>
    options.UseNpgsql(
        connectionString,
        x => x.MigrationsAssembly("PersonalFinanceAndExpenseTracker.Domain")));

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IncomeService>();
builder.Services.AddScoped<RecurringIncomeService>();
builder.Services.AddScoped<RecurringIncomeJobService>();
builder.Services.AddScoped<IncomeShareService>();
builder.Services.AddScoped<ExpenseService>();
builder.Services.AddScoped<RecurringExpenseService>();
builder.Services.AddScoped<RecurringExpenseJobService>();
builder.Services.AddScoped<ExpenseShareService>();
builder.Services.AddScoped<SavingsGoalService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<ChartShareService>();

builder.Services.AddHangfire(config => config
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(connectionString)));
builder.Services.AddHangfireServer();

var clientOrigin = builder.Configuration.GetValue<string>("AppSettings:ClientOrigin") ?? "http://localhost:4200";

builder.Services.AddCors(options =>
{
    options.AddPolicy("Default",
        policy =>
        {
            policy
                .WithOrigins(clientOrigin)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "XSRF-TOKEN";
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("auth", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });
});

var _authkey = builder.Configuration.GetValue<string>("JwtSettings:SecretKey");
builder.Services.AddAuthentication(item =>
{
    item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(item =>
{
    item.RequireHttpsMetadata = true;
    item.SaveToken = true;
    item.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authkey)),
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    item.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (string.IsNullOrEmpty(context.Token) &&
                context.Request.Cookies.TryGetValue("access_token", out var cookieToken))
            {
                context.Token = cookieToken;
            }

            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHangfireDashboard();
}

app.UseSerilogRequestLogging();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var ex = feature?.Error;

        var statusCode = ex switch
        {
            InvalidCredentialsException => (int)HttpStatusCode.Unauthorized,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            InvalidOperationException => (int)HttpStatusCode.Conflict,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("GlobalExceptionHandler");
        logger.LogError(ex, "Unhandled exception on {Path}", context.Request.Path);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(new
        {
            status = statusCode,
            title = statusCode == (int)HttpStatusCode.InternalServerError
                ? "An unexpected error occurred."
                : ex?.Message
        });
    });
});

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("Default");

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var method = context.Request.Method;
    var isMutating = HttpMethods.IsPost(method) || HttpMethods.IsPut(method)
        || HttpMethods.IsDelete(method) || HttpMethods.IsPatch(method);

    var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
    var csrfExemptPaths = new[]
    {
        "/api/authorize/login",
        "/api/authorize/register",
        "/api/authorize/refresh",
        "/api/googleauth/login"
    };

    if (isMutating && !csrfExemptPaths.Any(path.StartsWith))
    {
        var antiforgery = context.RequestServices.GetRequiredService<IAntiforgery>();

        try
        {
            await antiforgery.ValidateRequestAsync(context);
        }
        catch (AntiforgeryValidationException)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { status = 403, title = "CSRF token missing or invalid." });
            return;
        }
    }

    await next();
});

app.MapControllers();

var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();

recurringJobManager.AddOrUpdate<RecurringIncomeJobService>(
    "generate-recurring-incomes",
    service => service.GenerateDueIncomes(),
    Cron.Daily(2, 15),
    new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });

recurringJobManager.AddOrUpdate<RecurringExpenseJobService>(
    "generate-recurring-expenses",
    service => service.GenerateDueExpenses(),
    Cron.Daily(2, 15),
    new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });

app.Run();
