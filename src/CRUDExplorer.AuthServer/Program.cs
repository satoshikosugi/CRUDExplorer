using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using CRUDExplorer.AuthServer.Data;
using CRUDExplorer.AuthServer.Services;
using Serilog;
using Serilog.Events;
using AspNetCoreRateLimit;
using NWebsec.AspNetCore.Middleware;

// Serilog 構造化ロギング設定
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "CRUDExplorer.AuthServer")
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/authserver-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
        retainedFileCountLimit: 30)
    .CreateLogger();

try
{
    Log.Information("Starting CRUDExplorer AuthServer...");

    var builder = WebApplication.CreateBuilder(args);

    // Serilog をロギングプロバイダーとして使用
    builder.Host.UseSerilog();

    // データベース接続設定
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Host=localhost;Database=crudexplorer_auth;Username=postgres;Password=postgres";

    builder.Services.AddDbContext<AuthDbContext>(options =>
        options.UseNpgsql(connectionString));

    // サービス登録
    builder.Services.AddScoped<LicenseGenerationService>();
    builder.Services.AddScoped<AuthenticationService>();
    builder.Services.AddScoped<AuditLogService>();

    // レート制限設定
    builder.Services.AddMemoryCache();
    builder.Services.Configure<IpRateLimitOptions>(options =>
    {
        options.EnableEndpointRateLimiting = true;
        options.StackBlockedRequests = false;
        options.HttpStatusCode = 429;
        options.RealIpHeader = "X-Real-IP";
        options.ClientIdHeader = "X-ClientId";
        options.GeneralRules = new List<RateLimitRule>
        {
            new RateLimitRule
            {
                Endpoint = "*",
                Period = "1m",
                Limit = 60
            },
            new RateLimitRule
            {
                Endpoint = "*/api/license/authenticate",
                Period = "1m",
                Limit = 10
            }
        };
    });
    builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
    builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
    builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

    // JWT認証設定
    var jwtSecretKey = builder.Configuration["Jwt:SecretKey"]
        ?? "CRUDExplorer-Default-Secret-Key-Please-Change-In-Production-Min-32-Chars";
    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "CRUDExplorer.AuthServer";
    var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "CRUDExplorer.Client";

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
            };
        });

    builder.Services.AddAuthorization();

    // コントローラー追加
    builder.Services.AddControllers();

    // Swagger/OpenAPI設定
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "CRUDExplorer Authentication API",
            Version = "v1",
            Description = "認証・ライセンス管理APIサーバー",
            Contact = new OpenApiContact
            {
                Name = "CRUDExplorer Project",
                Url = new Uri("https://github.com/satoshikosugi/CRUDExplorer")
            },
            License = new OpenApiLicense
            {
                Name = "MIT License",
                Url = new Uri("https://opensource.org/licenses/MIT")
            }
        });

        // XML コメントを含める
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);

        // JWT Bearer認証設定
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
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
    });

    // CORS設定（環境変数による制御）
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
        ?? new[] { "http://localhost:5173", "http://localhost:3000" };

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("ProductionPolicy", policy =>
        {
            if (builder.Environment.IsDevelopment())
            {
                // 開発環境: すべて許可
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            }
            else
            {
                // 本番環境: 特定のオリジンのみ許可
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            }
        });
    });

    var app = builder.Build();

    // データベースマイグレーション自動適用（開発環境のみ）
    if (app.Environment.IsDevelopment())
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            try
            {
                db.Database.Migrate();
                Log.Information("Database migration completed successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while migrating the database");
            }
        }
    }

    // Serilog リクエストログ
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.GetLevel = (httpContext, elapsed, ex) => ex != null
            ? LogEventLevel.Error
            : httpContext.Response.StatusCode > 499
                ? LogEventLevel.Error
                : LogEventLevel.Information;
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
            diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString());
        };
    });

    // セキュリティヘッダー追加（NWebsec）
    app.Use(async (context, next) =>
    {
        // X-Content-Type-Options
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        // X-Frame-Options
        context.Response.Headers["X-Frame-Options"] = "DENY";
        // X-XSS-Protection
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
        // Referrer-Policy
        context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        // Content-Security-Policy
        context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline';";

        await next();
    });

    // レート制限ミドルウェア
    app.UseIpRateLimiting();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        Log.Information("Swagger UI enabled for development environment");
    }
    else
    {
        // 本番環境ではSwagger UIを無効化（セキュリティ対策）
        Log.Information("Swagger UI disabled for production environment");
    }

    app.UseCors("ProductionPolicy");

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    Log.Information("AuthServer started successfully on {Environment}", app.Environment.EnvironmentName);
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "AuthServer terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// 統合テスト用にProgramクラスをpublicにする
public partial class Program { }
