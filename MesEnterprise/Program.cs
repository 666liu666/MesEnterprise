
using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Trace;
using Prometheus;
using MesEnterprise.Data;
using MesEnterprise.Domain;
using MesEnterprise.Auth;
using MesEnterprise.Middleware;
using MesEnterprise.Cache;
using MesEnterprise.Realtime;
using MesEnterprise.Plugins;
using Hangfire;
using MesEnterprise.Cache;
using MesEnterprise.Plugins;
using MesEnterprise.Realtime;


namespace MesEnterprise;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                             .AddEnvironmentVariables();

        // Add services
        // Oracle DbContext
        builder.Services.AddDbContext<MesDbContext>(options =>
            options.UseOracle(builder.Configuration.GetConnectionString("OracleDb")));
        builder.Services.AddIdentityCore<ApplicationUser>(opts => { })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<MesDbContext>()
            .AddDefaultTokenProviders();

        // JWT
        var jwtSection = builder.Configuration.GetSection("Jwt");
        builder.Services.Configure<JwtOptions>(jwtSection);
        builder.Services.AddSingleton<IJwtService, JwtService>();
        var key = Encoding.UTF8.GetBytes(jwtSection["Key"] ?? "dev_key_replace");
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSection["Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true
                };
            });

        // Authorization: dynamic policies for permissions
        builder.Services.AddAuthorization(options => { /* policies added at runtime via attribute */ });
        builder.Services.AddScoped<PermissionHandler>();

        // Caching
        builder.Services.AddMemoryCache();
        builder.Services.AddSingleton<IRedisConnector, RedisConnector>();
        builder.Services.AddSingleton<ICacheService, HybridCacheService>();

        // Hangfire
        builder.Services.AddHangfire(cfg => cfg.UseMemoryStorage());
        builder.Services.AddHangfireServer();

        // SignalR
        builder.Services.AddSignalR();

        // Health, OpenTelemetry, Prometheus
        builder.Services.AddHealthChecks()
            .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), name: "sql")
            .AddRedis(builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379", name: "redis");
        object value = builder.Services.AddOpenTelemetryTracing(tracerProviderBuilder =>
        {
            tracerProviderBuilder.AddAspNetCoreInstrumentation();
        });

        // Controllers, Swagger
        builder.Services.AddControllers().AddFluentValidation();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Plugin loader: load plugins from ./plugins at startup into DI (IPlugin)
        PluginLoader.LoadPlugins(Path.Combine(AppContext.BaseDirectory, "plugins"), builder.Services);

        // DI scan (simple)
        builder.Services.Scan(scan => scan.FromCallingAssembly().AddClasses().AsImplementedInterfaces().WithScopedLifetime());

        // Background audit worker (uses Channel inside)
        builder.Services.AddHostedService<AuditBackgroundWorker>();

        var app = builder.Build();

        // Ensure DB
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
            await DbSeeder.SeedAsync(scope.ServiceProvider);
        }

        // Middlewares
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<AuditMiddleware>();

        // Security headers
        app.Use(async (ctx, next) =>
        {
            ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
            ctx.Response.Headers["X-Frame-Options"] = "DENY";
            ctx.Response.Headers["Referrer-Policy"] = "no-referrer";
            await next();
        });

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        // Prometheus
        app.UseMetricServer();
        app.UseHttpMetrics();

        // Hangfire dashboard (in prod add auth)
        app.UseHangfireDashboard("/hangfire");

        // SignalR hub
        app.MapHub<MesHub>("/hubs/mes");

        // Health
        app.MapHealthChecks("/health/ready");
        app.MapHealthChecks("/health/live");

        if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
        app.MapControllers();

        app.Run();
    }
}

