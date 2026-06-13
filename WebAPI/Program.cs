using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog.Extensions.Logging;
using WebAPI.Auth;
using WebAPI.Configurations;
using WebAPI.Endpoints.Users;

namespace WebAPI
{
    public partial class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Railway sets PORT; Kestrel listens on it
            var port = int.Parse(Environment.GetEnvironmentVariable("PORT") ?? "8080");
            builder.WebHost.ConfigureKestrel(o =>
            {
                o.AddServerHeader = false;
                o.ListenAnyIP(port);
            });

            // Railway PostgreSQL plugin provides DATABASE_URL (postgresql://user:pass@host:port/db).
            // Parse it into the Npgsql connection string if ConnectionStrings:DefaultConnection is empty.
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            if (!string.IsNullOrEmpty(databaseUrl) &&
                string.IsNullOrEmpty(builder.Configuration.GetConnectionString("DefaultConnection")))
            {
                var uri = new Uri(databaseUrl);
                var userInfo = uri.UserInfo.Split(':');
                var host = uri.Host;
                var dbPort = uri.Port > 0 ? uri.Port : 5432;
                var database = uri.AbsolutePath.TrimStart('/');
                var username = Uri.UnescapeDataString(userInfo[0]);
                var password = Uri.UnescapeDataString(userInfo.Length > 1 ? userInfo[1] : "");
                builder.Configuration["ConnectionStrings:DefaultConnection"] =
                    $"Host={host};Port={dbPort};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
            }

            var logger = Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            logger.Information("Starting UserService");

            builder.AddLoggerConfigs();

            var appLogger = new SerilogLoggerFactory(logger).CreateLogger<Program>();

            builder.Services.AddOptionConfigs(builder.Configuration, appLogger, builder);
            builder.Services.AddServiceConfigs(appLogger, builder);

            builder.Services.AddFastEndpoints()
                .SwaggerDocument(o =>
                {
                    o.DocumentSettings = s =>
                    {
                        s.Title = "UserService API";
                        s.Version = "v1";
                        s.Description = "User Service API";
                    };
                    o.ShortSchemaNames = true;
                    o.EnableJWTBearerAuth = true;
                });

            builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();

            var jwtSection = builder.Configuration.GetSection(JwtSettings.SectionName);
            var secretKey = jwtSection["SecretKey"]
                ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured.");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                        ValidateIssuer = true,
                        ValidIssuer = jwtSection["Issuer"],
                        ValidateAudience = true,
                        ValidAudience = jwtSection["Audience"],
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddSingleton<JwtTokenService>();
            builder.Services.AddSingleton<ITokenService>(sp => sp.GetRequiredService<JwtTokenService>());

            builder.Services.AddMemoryCache();

            var app = builder.Build();

            await app.UseAppMiddleware();

            app.Run();
        }
    }

    public partial class Program { }
}
