using System.Text;
using System.Transactions;
using API.Filters;
using API.Hubs;
using API.Middleware;
using Application.Interfaces;
using Domain.Interfaces;
using FluentValidation;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MySql;
using Infrastructure.BackgroundJobs;
using Infrastructure.Identity;
using Infrastructure.Notifications;
using Infrastructure.Persistence;
using Infrastructure.Receipts;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

// Configure Serilog early
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/app-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog for logging
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            path: "logs/app-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"));

    // Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Subscription & Membership Management API",
        Version = "v1",
        Description = "A comprehensive SaaS API for managing subscriptions, memberships, payments, and business operations. " +
                      "This API provides complete functionality for small and medium businesses to manage their subscription-based services.",
        Contact = new OpenApiContact
        {
            Name = "API Support",
            Email = "support@subscriptionmanagement.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token. " +
                      "Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Enable annotations
 
    
    // Use full names for schema IDs to avoid conflicts
        c.CustomSchemaIds(type => type.FullName);
    });

    // Database configuration
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var databaseProvider = builder.Configuration["Database:Provider"] ?? "MySQL";

    if (databaseProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
    }
    else if (databaseProvider.Equals("MySQL", StringComparison.OrdinalIgnoreCase))
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
    }
    else
    {
        throw new InvalidOperationException($"Unsupported database provider: {databaseProvider}");
    }

    // JWT Authentication
    var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "SubscriptionManagement";
    var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "SubscriptionManagement";

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        // Configure JWT for SignalR WebSocket connections
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                // If the request is for the notification hub and contains an access token
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationHub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

    builder.Services.AddAuthorization();

    // Application services
    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(typeof(Application.Features.Auth.Register.RegisterCommand).Assembly);
        cfg.AddOpenBehavior(typeof(Application.Behaviors.ValidationBehavior<,>));
    });
    builder.Services.AddValidatorsFromAssembly(typeof(Application.Features.Auth.Register.RegisterCommandValidator).Assembly);

    // Infrastructure services
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();
    builder.Services.AddScoped<IMemberRepository, MemberRepository>();
    builder.Services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
    builder.Services.AddScoped<IMembershipRepository, MembershipRepository>();
    builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

    builder.Services.AddScoped<IJwtService, JwtService>();
    builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    builder.Services.AddScoped<INotificationService>(sp =>
    {
        var hubContext = sp.GetRequiredService<IHubContext<NotificationHub>>();
        var unitOfWork = sp.GetRequiredService<IUnitOfWork>();
        var logger = sp.GetRequiredService<ILogger<NotificationService>>();
        return new NotificationService(hubContext.Clients, unitOfWork, logger);
    });
    builder.Services.AddScoped<IReceiptService, ReceiptService>();

    // Background job service
    builder.Services.AddScoped<MembershipExpiryService>();

    builder.Services.AddHttpContextAccessor();

    // SignalR configuration
    builder.Services.AddSignalR(options =>
    {
        options.EnableDetailedErrors = builder.Environment.IsDevelopment();
        options.MaximumReceiveMessageSize = 1024 * 1024; // 1MB
        options.StreamBufferCapacity = 10;
    });

    // Hangfire configuration
    var hangfireConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseStorage(new MySqlStorage(
            hangfireConnectionString,
            new MySqlStorageOptions
            {
                TransactionIsolationLevel = IsolationLevel.ReadCommitted,
                QueuePollInterval = TimeSpan.FromSeconds(15),
                JobExpirationCheckInterval = TimeSpan.FromHours(1),
                CountersAggregateInterval = TimeSpan.FromMinutes(5),
                PrepareSchemaIfNecessary = true,
                DashboardJobListLimit = 50000,
                TransactionTimeout = TimeSpan.FromMinutes(1),
                TablesPrefix = "Hangfire"
            })));

    builder.Services.AddHangfireServer(options =>
    {
        options.WorkerCount = Environment.ProcessorCount * 5;
        options.ServerTimeout = TimeSpan.FromMinutes(4);
        options.SchedulePollingInterval = TimeSpan.FromSeconds(15);
        options.HeartbeatInterval = TimeSpan.FromSeconds(30);
        options.ServerCheckInterval = TimeSpan.FromMinutes(1);
        options.CancellationCheckInterval = TimeSpan.FromSeconds(5);
    });

    // CORS - Updated to support SignalR
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:5000", "http://localhost:5001", "http://localhost:5002", "https://localhost:5001", "https://localhost:5003")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials(); // Required for SignalR
        });
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Test")
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseCors("AllowAll");

    app.UseAuthentication();
    app.UseAuthorization();

    // Hangfire Dashboard
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        DashboardTitle = "Subscription Management - Background Jobs",
        Authorization = new[] { new HangfireAuthorizationFilter() },
        StatsPollingInterval = 2000,
        DisplayStorageConnectionString = false,
        IsReadOnlyFunc = (DashboardContext context) => false
    });

    app.MapControllers();

    // Map SignalR Hub
    app.MapHub<NotificationHub>("/notificationHub", options =>
    {
        options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets |
                             Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
    });

    // Ensure database is created and seeded
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.EnsureCreated();
        
        // Seed data for development
        if (app.Environment.IsDevelopment())
        {
            await Infrastructure.Persistence.SeedData.SeedAsync(dbContext);
        }

        // Configure Hangfire recurring jobs
        RecurringJob.AddOrUpdate<MembershipExpiryService>(
            "process-expired-memberships",
            service => service.ProcessExpiredMembershipsAsync(CancellationToken.None),
            Cron.Daily(2, 0), // Run daily at 2:00 AM UTC
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc,
                MisfireHandling = MisfireHandlingMode.Relaxed
            });
    }

    Log.Information("Application started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

