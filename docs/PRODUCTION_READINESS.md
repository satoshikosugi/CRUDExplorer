# CRUDExplorer 本番環境対応チェックリスト

本ドキュメントでは、CRUDExplorer 認証サーバーを本番環境にデプロイする前に必ず実施すべきセキュリティ設定と構成変更について説明します。

**⚠️ 重要**: 以下の設定変更を実施せずに本番環境にデプロイすると、重大なセキュリティリスクが発生します。

---

## 目次

1. [セキュリティ設定の必須変更](#セキュリティ設定の必須変更)
2. [認証・認可の有効化](#認証認可の有効化)
3. [環境変数設定](#環境変数設定)
4. [CORS設定の本番化](#cors設定の本番化)
5. [ロギング設定](#ロギング設定)
6. [データベース設定](#データベース設定)
7. [追加のセキュリティ対策](#追加のセキュリティ対策)
8. [デプロイ前チェックリスト](#デプロイ前チェックリスト)

---

## セキュリティ設定の必須変更

### 🔴 優先度: 最高

以下の設定は**必ず**変更してください。これらを変更しないと、認証が突破される、データが漏洩する等の重大なセキュリティ問題が発生します。

---

## 1. 認証・認可の有効化

### AdminController の [Authorize] 属性有効化

**現在の状態**: 統合テスト用に `[Authorize]` 属性がコメントアウトされています。

**ファイル**: `src/CRUDExplorer.AuthServer/Controllers/AdminController.cs`

**変更前**:
```csharp
[ApiController]
[Route("api/[controller]")]
// [Authorize] // 統合テスト用に一時的に無効化 - TODO: 本番環境では有効化すること
public class AdminController : ControllerBase
{
    // ...
}
```

**変更後**:
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // ✅ 本番環境では必ず有効化
public class AdminController : ControllerBase
{
    // ...
}
```

**影響**:
- 管理APIへのアクセスにJWTトークンが必須になります
- 認証なしでアクセスすると 401 Unauthorized が返されます
- **注意**: この変更により統合テスト (`AdminApiTests.cs`) が失敗する可能性があります。テストではモックのJWTトークンを使用するように修正してください。

**テスト対応**:
統合テストでJWTトークンを生成してAuthorizationヘッダーに設定する必要があります。詳細は後述の「統合テストの修正」セクションを参照してください。

---

## 2. 環境変数設定

### JWT 秘密鍵の変更

**現在の状態**: デフォルトの秘密鍵がハードコードされています。

**ファイル**: `src/CRUDExplorer.AuthServer/Program.cs`

**変更前**:
```csharp
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"]
    ?? "CRUDExplorer-Default-Secret-Key-Please-Change-In-Production-Min-32-Chars";
```

**問題点**:
- デフォルトの秘密鍵は公開されているため、攻撃者が偽造トークンを作成できます
- **絶対に本番環境でデフォルト値を使用しないでください**

**変更方法**:

#### Linux/macOS:
```bash
# 強力なランダム秘密鍵を生成（48文字以上推奨）
openssl rand -base64 48

# 環境変数に設定
export Jwt__SecretKey="生成された秘密鍵をここに貼り付け"
```

#### Windows (PowerShell):
```powershell
# 強力なランダム秘密鍵を生成
-join ((48..57) + (65..90) + (97..122) | Get-Random -Count 64 | % {[char]$_})

# 環境変数に設定
$env:Jwt__SecretKey="生成された秘密鍵をここに貼り付け"
```

#### Docker Compose:
`.env` ファイルに設定:
```bash
JWT_SECRET_KEY=your-generated-secret-key-min-48-chars-here
```

#### appsettings.Production.json:
**推奨しません**（ファイルに秘密鍵を保存するのは危険です。環境変数を使用してください）

もし設定ファイルを使用する場合は、以下のように設定し、ファイルのパーミッションを厳重に管理してください:
```json
{
  "Jwt": {
    "SecretKey": "your-generated-secret-key-min-48-chars-here"
  }
}
```

---

### データベース接続文字列の環境変数化

**現在の状態**: デフォルトの接続文字列に平文パスワードがハードコードされています。

**ファイル**: `src/CRUDExplorer.AuthServer/Program.cs`

**変更前**:
```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Database=crudexplorer_auth;Username=postgres;Password=postgres";
```

**問題点**:
- パスワードが平文でソースコードに含まれています
- デフォルトパスワード `postgres` は非常に脆弱です

**変更方法**:

#### 環境変数で設定:
```bash
export ConnectionStrings__DefaultConnection="Host=your-db-host;Database=crudexplorer_auth;Username=your-user;Password=your-strong-password"
```

#### Docker Compose:
`.env` ファイル:
```bash
POSTGRES_USER=crudexplorer
POSTGRES_PASSWORD=your-strong-password-here
POSTGRES_DB=crudexplorer_auth
```

`docker-compose.yml`:
```yaml
environment:
  - ConnectionStrings__DefaultConnection=Host=postgres;Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD}
```

#### Azure App Service:
アプリケーション設定で以下を追加:
- 名前: `ConnectionStrings__DefaultConnection`
- 値: `Host=your-azure-postgres.postgres.database.azure.com;Database=crudexplorer_auth;Username=your-user@your-server;Password=your-password;SSL Mode=Require`

---

## 3. CORS設定の本番化

### AllowAnyOrigin() の削除

**現在の状態**: すべてのオリジンからのアクセスを許可しています。

**ファイル**: `src/CRUDExplorer.AuthServer/Program.cs`

**変更前**:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ...

app.UseCors("AllowAll");
```

**問題点**:
- CSRF (Cross-Site Request Forgery) 攻撃のリスク
- 任意のWebサイトからAPIにアクセス可能

**変更後**:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionPolicy", policy =>
    {
        // 本番環境の実際のクライアントアプリケーションのURLを指定
        policy.WithOrigins(
                  "https://yourapp.example.com",
                  "https://www.yourapp.example.com"
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // WithOrigins 使用時は AllowCredentials() を指定可能
    });
});

// ...

// 環境に応じてCORSポリシーを切り替え
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAll");
}
else
{
    app.UseCors("ProductionPolicy");
}
```

**環境変数で動的に設定する方法**:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionPolicy", policy =>
    {
        var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]?.Split(',')
            ?? new[] { "https://yourapp.example.com" };

        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

`appsettings.Production.json`:
```json
{
  "Cors": {
    "AllowedOrigins": "https://yourapp.example.com,https://www.yourapp.example.com"
  }
}
```

---

## 4. ロギング設定

### 本番環境用ロギングレベル設定

**ファイル**: `appsettings.Production.json` を作成

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Error"
    }
  }
}
```

**推奨**: Serilog を使用した構造化ロギング

#### NuGet パッケージ追加:
```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Seq  # オプション: Seq サーバー使用時
```

#### Program.cs に追加:
```csharp
using Serilog;

// アプリケーション起動前にSerilogを設定
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        "logs/authserver-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting CRUDExplorer AuthServer");

    var builder = WebApplication.CreateBuilder(args);

    // Serilog を ASP.NET Core に統合
    builder.Host.UseSerilog();

    // ... 既存の設定 ...

    var app = builder.Build();

    // ... 既存のミドルウェア設定 ...

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
```

---

## 5. データベース設定

### PostgreSQL セキュリティ設定

#### 強力なパスワードの使用:
```bash
# 強力なパスワード生成
openssl rand -base64 32
```

#### PostgreSQL ユーザー権限の制限:
```sql
-- 管理者権限のないユーザーを作成
CREATE USER crudexplorer_app WITH PASSWORD 'your-strong-password';

-- 必要最小限の権限のみ付与
GRANT CONNECT ON DATABASE crudexplorer_auth TO crudexplorer_app;
GRANT USAGE ON SCHEMA public TO crudexplorer_app;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO crudexplorer_app;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO crudexplorer_app;

-- 今後作成されるテーブルにも同じ権限を付与
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO crudexplorer_app;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT USAGE, SELECT ON SEQUENCES TO crudexplorer_app;
```

#### SSL/TLS 接続の有効化:
接続文字列に `SSL Mode=Require` を追加:
```
Host=your-db-host;Database=crudexplorer_auth;Username=crudexplorer_app;Password=your-password;SSL Mode=Require
```

---

## 6. 追加のセキュリティ対策

### レート制限の実装

**NuGet パッケージ追加**:
```bash
dotnet add package AspNetCoreRateLimit
```

**Program.cs に追加**:
```csharp
using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

// レート制限設定
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "POST:/api/license/authenticate",
            Period = "1m",
            Limit = 10 // 1分間に10回まで
        },
        new RateLimitRule
        {
            Endpoint = "*",
            Period = "1m",
            Limit = 100 // その他のエンドポイントは1分間に100回まで
        }
    };
});

builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// ...

var app = builder.Build();

// ミドルウェアパイプラインに追加（認証・認可の前に配置）
app.UseIpRateLimiting();
app.UseAuthentication();
app.UseAuthorization();

// ...
```

**appsettings.Production.json**:
```json
{
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "RealIpHeader": "X-Real-IP",
    "ClientIdHeader": "X-ClientId",
    "HttpStatusCode": 429
  }
}
```

---

### セキュリティヘッダーの追加

**NuGet パッケージ追加**:
```bash
dotnet add package NWebsec.AspNetCore.Middleware
```

**Program.cs に追加**:
```csharp
var app = builder.Build();

// セキュリティヘッダー追加
app.UseXContentTypeOptions();
app.UseReferrerPolicy(opts => opts.NoReferrer());
app.UseXXssProtection(opts => opts.EnabledWithBlockMode());
app.UseXfo(opts => opts.Deny());

// Content Security Policy
app.UseCsp(opts => opts
    .DefaultSources(s => s.Self())
    .ScriptSources(s => s.Self())
    .StyleSources(s => s.Self())
);

// HSTS (HTTPS 強制)
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// ...
```

---

### Swagger UIの本番環境での無効化（推奨）

**Program.cs 変更**:
```csharp
// Swagger/OpenAPI 設定は開発環境とステージング環境のみ有効化
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CRUDExplorer Authentication API v1");
    });
}
```

**本番環境でSwaggerを有効化する場合**:
認証を追加し、アクセスを制限してください:
```csharp
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CRUDExplorer Authentication API v1");
});

// Swagger へのアクセスに認証を要求
app.MapSwagger().RequireAuthorization();
```

---

## 7. データベース設定

### マイグレーションの自動適用を無効化

**ファイル**: `src/CRUDExplorer.AuthServer/Program.cs`

**変更前**:
```csharp
// データベースマイグレーション自動適用（開発環境のみ）
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        try
        {
            db.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }
}
```

**理由**: 本番環境では、マイグレーションは手動で制御するべきです。自動マイグレーションは予期しないスキーマ変更を引き起こす可能性があります。

**本番環境でのマイグレーション実行**:
```bash
# デプロイ前に手動でマイグレーション実行
cd src/CRUDExplorer.AuthServer
dotnet ef database update --context AuthDbContext
```

---

## 8. デプロイ前チェックリスト

以下のチェックリストを使用して、本番環境デプロイ前にすべての必須設定が完了していることを確認してください。

### 🔴 必須項目（セキュリティクリティカル）

- [ ] **AdminController の `[Authorize]` 属性を有効化**
- [ ] **JWT 秘密鍵を強力なランダム文字列に変更（48文字以上）**
- [ ] **データベース接続文字列を環境変数化**
- [ ] **PostgreSQL パスワードを強力なものに変更**
- [ ] **CORS 設定を `AllowAnyOrigin()` から特定のオリジンのみに変更**
- [ ] **デフォルトの秘密鍵・パスワードがコードに残っていないことを確認**

### 🟡 推奨項目（セキュリティベストプラクティス）

- [ ] **Serilog 等の構造化ロギングを実装**
- [ ] **レート制限を実装（AspNetCoreRateLimit）**
- [ ] **セキュリティヘッダーを追加（NWebsec）**
- [ ] **Swagger UI を本番環境で無効化、または認証を要求**
- [ ] **HTTPS/TLS を有効化（Let's Encrypt 等）**
- [ ] **PostgreSQL 接続に SSL/TLS を使用**
- [ ] **データベースマイグレーションの自動適用を無効化**

### 🟢 オプション項目（追加のセキュリティ強化）

- [ ] **ヘルスチェックエンドポイント実装**
- [ ] **監視・アラート設定（Application Insights, Datadog 等）**
- [ ] **定期的なデータベースバックアップ設定**
- [ ] **API バージョニング実装**
- [ ] **IP ホワイトリスト/ブラックリスト設定**

---

## 統合テストの修正（[Authorize] 有効化後）

`[Authorize]` 属性を有効化すると、統合テスト (`AdminApiTests.cs`) が失敗します。以下のようにテストを修正してください。

### AuthServerWebApplicationFactory.cs の拡張

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class AuthServerWebApplicationFactory : WebApplicationFactory<Program>
{
    // JWT トークン生成メソッド追加
    public string GenerateTestJwtToken(
        string userId = "test-user-id",
        string email = "test@example.com")
    {
        var jwtSecretKey = "CRUDExplorer-Test-Secret-Key-Min-32-Chars-For-Testing";
        var jwtIssuer = "CRUDExplorer.AuthServer";
        var jwtAudience = "CRUDExplorer.Client";

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ...
}
```

### AdminApiTests.cs の修正

```csharp
public class AdminApiTests : IClassFixture<AuthServerWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly AuthServerWebApplicationFactory _factory;

    public AdminApiTests(AuthServerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();

        // すべてのリクエストに JWT トークンを追加
        var token = _factory.GenerateTestJwtToken();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    // ... テストメソッドは変更不要 ...
}
```

---

## まとめ

本番環境へのデプロイ前に、このチェックリストのすべての**必須項目**を完了してください。推奨項目とオプション項目も、セキュリティを強化するために実装することを強く推奨します。

**セキュリティは継続的なプロセスです**。定期的に以下を実施してください:
- 依存パッケージの更新（セキュリティパッチ適用）
- ログの監視とレビュー
- 脆弱性スキャン
- ペネトレーションテスト

ご質問や問題がある場合は、[GitHub Issues](https://github.com/satoshikosugi/CRUDExplorer/issues) で報告してください。
