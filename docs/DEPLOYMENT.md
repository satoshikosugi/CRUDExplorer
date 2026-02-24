# CRUDExplorer デプロイメントガイド

本ドキュメントでは、CRUDExplorer認証サーバー（AuthServer）とクライアントアプリケーション（Avalonia UI）のデプロイ手順について説明します。

---

## 目次

1. [システム要件](#システム要件)
2. [認証サーバーのデプロイ](#認証サーバーのデプロイ)
   - [Docker を使用したデプロイ](#docker-を使用したデプロイ)
   - [手動デプロイ](#手動デプロイ)
   - [環境変数設定](#環境変数設定)
3. [PostgreSQL セットアップ](#postgresql-セットアップ)
4. [クライアントアプリケーションのデプロイ](#クライアントアプリケーションのデプロイ)
5. [HTTPS/SSL 設定](#httpsssl-設定)
6. [本番環境セキュリティ設定](#本番環境セキュリティ設定)
7. [トラブルシューティング](#トラブルシューティング)

---

## システム要件

### 認証サーバー
- **.NET 8.0 Runtime** 以降
- **PostgreSQL 14** 以降（推奨: PostgreSQL 15+）
- **メモリ**: 最小 512MB、推奨 1GB+
- **ストレージ**: 最小 1GB
- **OS**: Linux (Ubuntu 22.04+/Debian 11+), Windows Server 2019+, macOS 12+

### クライアントアプリケーション
- **.NET 8.0 Desktop Runtime** 以降
- **OS**: Windows 10+, macOS 12+, Linux (Ubuntu 22.04+)
- **メモリ**: 最小 512MB、推奨 2GB+
- **ストレージ**: 最小 500MB

---

## 認証サーバーのデプロイ

### Docker を使用したデプロイ

#### 1. Dockerfile の作成

プロジェクトルートに以下の `Dockerfile` を作成します：

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/CRUDExplorer.AuthServer/CRUDExplorer.AuthServer.csproj", "CRUDExplorer.AuthServer/"]
COPY ["src/CRUDExplorer.Core/CRUDExplorer.Core.csproj", "CRUDExplorer.Core/"]
RUN dotnet restore "CRUDExplorer.AuthServer/CRUDExplorer.AuthServer.csproj"
COPY src/ .
WORKDIR "/src/CRUDExplorer.AuthServer"
RUN dotnet build "CRUDExplorer.AuthServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CRUDExplorer.AuthServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CRUDExplorer.AuthServer.dll"]
```

#### 2. Docker Compose の作成

`docker-compose.yml` を作成します：

```yaml
version: '3.8'

services:
  postgres:
    image: postgres:15-alpine
    container_name: crudexplorer-postgres
    environment:
      POSTGRES_USER: ${POSTGRES_USER:-crudexplorer}
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD:-changeme}
      POSTGRES_DB: ${POSTGRES_DB:-crudexplorer_auth}
    volumes:
      - postgres-data:/var/lib/postgresql/data
    ports:
      - "${POSTGRES_PORT:-5432}:5432"
    networks:
      - crudexplorer-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U ${POSTGRES_USER:-crudexplorer}"]
      interval: 10s
      timeout: 5s
      retries: 5

  authserver:
    build:
      context: .
      dockerfile: Dockerfile
    container_name: crudexplorer-authserver
    environment:
      - ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT:-Production}
      - ASPNETCORE_URLS=http://+:80
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=${POSTGRES_DB:-crudexplorer_auth};Username=${POSTGRES_USER:-crudexplorer};Password=${POSTGRES_PASSWORD:-changeme}
      - Jwt__SecretKey=${JWT_SECRET_KEY}
      - Jwt__Issuer=${JWT_ISSUER:-CRUDExplorer.AuthServer}
      - Jwt__Audience=${JWT_AUDIENCE:-CRUDExplorer.Client}
      - Jwt__ExpirationMinutes=${JWT_EXPIRATION_MINUTES:-60}
    ports:
      - "${AUTHSERVER_PORT:-5000}:80"
    depends_on:
      postgres:
        condition: service_healthy
    networks:
      - crudexplorer-network
    restart: unless-stopped

volumes:
  postgres-data:
    driver: local

networks:
  crudexplorer-network:
    driver: bridge
```

#### 3. 環境変数ファイルの作成

`.env` ファイルを作成します（**このファイルは `.gitignore` に追加してください**）：

```bash
# PostgreSQL 設定
POSTGRES_USER=crudexplorer
POSTGRES_PASSWORD=your-strong-password-here
POSTGRES_DB=crudexplorer_auth
POSTGRES_PORT=5432

# AuthServer 設定
ASPNETCORE_ENVIRONMENT=Production
AUTHSERVER_PORT=5000

# JWT 設定（32文字以上の強力なランダム文字列を設定）
JWT_SECRET_KEY=your-jwt-secret-key-min-32-chars-change-in-production
JWT_ISSUER=CRUDExplorer.AuthServer
JWT_AUDIENCE=CRUDExplorer.Client
JWT_EXPIRATION_MINUTES=60
```

**重要**: `JWT_SECRET_KEY` は以下のコマンドで生成できます：

```bash
# Linux/macOS
openssl rand -base64 48

# PowerShell (Windows)
-join ((48..57) + (65..90) + (97..122) | Get-Random -Count 48 | % {[char]$_})
```

#### 4. Docker Compose でデプロイ

```bash
# ビルドと起動
docker-compose up -d

# ログ確認
docker-compose logs -f authserver

# データベースマイグレーション実行
docker-compose exec authserver dotnet ef database update

# 停止
docker-compose down

# データベースも削除する場合
docker-compose down -v
```

#### 5. 動作確認

```bash
# ヘルスチェック
curl http://localhost:5000/health

# Swagger UI
open http://localhost:5000/swagger
```

---

### 手動デプロイ

#### 1. ビルド

```bash
cd src/CRUDExplorer.AuthServer
dotnet publish -c Release -o /path/to/publish
```

#### 2. PostgreSQL セットアップ

PostgreSQL をインストールし、データベースとユーザーを作成します：

```sql
-- PostgreSQL にログイン
psql -U postgres

-- データベース作成
CREATE DATABASE crudexplorer_auth;

-- ユーザー作成
CREATE USER crudexplorer WITH PASSWORD 'your-strong-password';

-- 権限付与
GRANT ALL PRIVILEGES ON DATABASE crudexplorer_auth TO crudexplorer;
```

#### 3. appsettings.Production.json の作成

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=crudexplorer_auth;Username=crudexplorer;Password=your-strong-password"
  },
  "Jwt": {
    "SecretKey": "your-jwt-secret-key-min-32-chars-change-in-production",
    "Issuer": "CRUDExplorer.AuthServer",
    "Audience": "CRUDExplorer.Client",
    "ExpirationMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

#### 4. データベースマイグレーション

```bash
cd /path/to/publish
dotnet CRUDExplorer.AuthServer.dll --migrate

# または EF Core CLI を使用
dotnet ef database update --project src/CRUDExplorer.AuthServer
```

#### 5. サービスとして登録（Linux - systemd）

`/etc/systemd/system/crudexplorer-authserver.service` を作成：

```ini
[Unit]
Description=CRUDExplorer Authentication Server
After=network.target postgresql.service

[Service]
Type=notify
User=www-data
WorkingDirectory=/path/to/publish
ExecStart=/usr/bin/dotnet /path/to/publish/CRUDExplorer.AuthServer.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=crudexplorer-authserver
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

サービスの有効化と起動：

```bash
sudo systemctl daemon-reload
sudo systemctl enable crudexplorer-authserver
sudo systemctl start crudexplorer-authserver
sudo systemctl status crudexplorer-authserver
```

#### 6. サービスとして登録（Windows - NSSM）

1. [NSSM](https://nssm.cc/) をダウンロード
2. 管理者権限で以下を実行：

```powershell
nssm install CRUDExplorerAuthServer "C:\Program Files\dotnet\dotnet.exe" "C:\path\to\publish\CRUDExplorer.AuthServer.dll"
nssm set CRUDExplorerAuthServer AppDirectory "C:\path\to\publish"
nssm set CRUDExplorerAuthServer AppEnvironmentExtra ASPNETCORE_ENVIRONMENT=Production
nssm start CRUDExplorerAuthServer
```

---

### 環境変数設定

本番環境では、以下の環境変数を必ず設定してください：

| 環境変数 | 説明 | 例 |
|---------|------|-----|
| `ASPNETCORE_ENVIRONMENT` | 実行環境 | `Production` |
| `ConnectionStrings__DefaultConnection` | PostgreSQL接続文字列 | `Host=db.example.com;Database=crudexplorer_auth;Username=user;Password=pass` |
| `Jwt__SecretKey` | JWT署名用秘密鍵（32文字以上） | `your-secret-key-min-32-chars` |
| `Jwt__Issuer` | JWT発行者 | `CRUDExplorer.AuthServer` |
| `Jwt__Audience` | JWT対象者 | `CRUDExplorer.Client` |
| `Jwt__ExpirationMinutes` | トークン有効期限（分） | `60` |

---

## PostgreSQL セットアップ

### Docker での PostgreSQL セットアップ

```bash
docker run -d \
  --name crudexplorer-postgres \
  -e POSTGRES_USER=crudexplorer \
  -e POSTGRES_PASSWORD=your-strong-password \
  -e POSTGRES_DB=crudexplorer_auth \
  -p 5432:5432 \
  -v postgres-data:/var/lib/postgresql/data \
  postgres:15-alpine
```

### 手動セットアップ（Ubuntu/Debian）

```bash
# PostgreSQL インストール
sudo apt update
sudo apt install postgresql postgresql-contrib

# PostgreSQL 起動
sudo systemctl start postgresql
sudo systemctl enable postgresql

# ユーザー切り替えとDB作成
sudo -u postgres psql

# PostgreSQL内で実行
CREATE DATABASE crudexplorer_auth;
CREATE USER crudexplorer WITH PASSWORD 'your-strong-password';
GRANT ALL PRIVILEGES ON DATABASE crudexplorer_auth TO crudexplorer;
\q
```

### データベースバックアップ

```bash
# バックアップ作成
pg_dump -U crudexplorer -h localhost crudexplorer_auth > backup_$(date +%Y%m%d).sql

# リストア
psql -U crudexplorer -h localhost crudexplorer_auth < backup_20260222.sql
```

---

## クライアントアプリケーションのデプロイ

### Windows 版

#### 1. 自己完結型（Self-Contained）ビルド

```bash
dotnet publish src/CRUDExplorer.UI/CRUDExplorer.UI.csproj \
  -c Release \
  -r win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  -o ./publish/win-x64
```

#### 2. フレームワーク依存型（Framework-Dependent）ビルド

```bash
dotnet publish src/CRUDExplorer.UI/CRUDExplorer.UI.csproj \
  -c Release \
  -r win-x64 \
  --self-contained false \
  -o ./publish/win-x64-fx
```

**配布時の注意**: フレームワーク依存型の場合、ユーザーが .NET 8 Desktop Runtime をインストールする必要があります。

#### 3. ZIP パッケージ作成

```bash
cd publish/win-x64
7z a ../CRUDExplorer-win-x64-v1.0.0.zip *
```

---

### macOS 版

#### 1. macOS アプリバンドル作成

```bash
dotnet publish src/CRUDExplorer.UI/CRUDExplorer.UI.csproj \
  -c Release \
  -r osx-arm64 \
  --self-contained true \
  -p:PublishSingleFile=false \
  -o ./publish/osx-arm64
```

#### 2. .app バンドル構造作成

```bash
mkdir -p CRUDExplorer.app/Contents/MacOS
mkdir -p CRUDExplorer.app/Contents/Resources

# バイナリコピー
cp -r publish/osx-arm64/* CRUDExplorer.app/Contents/MacOS/

# Info.plist 作成
cat > CRUDExplorer.app/Contents/Info.plist << 'EOF'
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleExecutable</key>
    <string>CRUDExplorer.UI</string>
    <key>CFBundleIdentifier</key>
    <string>com.crudexplorer.app</string>
    <key>CFBundleName</key>
    <string>CRUDExplorer</string>
    <key>CFBundleVersion</key>
    <string>1.0.0</string>
    <key>CFBundleShortVersionString</key>
    <string>1.0.0</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>LSMinimumSystemVersion</key>
    <string>12.0</string>
    <key>NSHighResolutionCapable</key>
    <true/>
</dict>
</plist>
EOF

# 実行権限付与
chmod +x CRUDExplorer.app/Contents/MacOS/CRUDExplorer.UI
```

#### 3. DMG イメージ作成

```bash
# create-dmg をインストール（Homebrew）
brew install create-dmg

# DMG 作成
create-dmg \
  --volname "CRUDExplorer" \
  --window-pos 200 120 \
  --window-size 800 400 \
  --icon-size 100 \
  --icon "CRUDExplorer.app" 200 190 \
  --hide-extension "CRUDExplorer.app" \
  --app-drop-link 600 185 \
  "CRUDExplorer-v1.0.0.dmg" \
  "CRUDExplorer.app"
```

---

### Linux 版

#### 1. AppImage 作成（推奨）

```bash
# ビルド
dotnet publish src/CRUDExplorer.UI/CRUDExplorer.UI.csproj \
  -c Release \
  -r linux-x64 \
  --self-contained true \
  -o ./publish/linux-x64

# AppImage 構造作成
mkdir -p CRUDExplorer.AppDir/usr/bin
cp -r publish/linux-x64/* CRUDExplorer.AppDir/usr/bin/

# デスクトップファイル作成
cat > CRUDExplorer.AppDir/CRUDExplorer.desktop << 'EOF'
[Desktop Entry]
Type=Application
Name=CRUDExplorer
Exec=CRUDExplorer.UI
Icon=crudexplorer
Categories=Development;
EOF

# AppRun スクリプト作成
cat > CRUDExplorer.AppDir/AppRun << 'EOF'
#!/bin/bash
SELF=$(readlink -f "$0")
HERE=${SELF%/*}
export PATH="${HERE}/usr/bin/:${PATH}"
EXEC="${HERE}/usr/bin/CRUDExplorer.UI"
exec "${EXEC}" "$@"
EOF
chmod +x CRUDExplorer.AppDir/AppRun

# appimagetool でパッケージ化
wget https://github.com/AppImage/AppImageKit/releases/download/continuous/appimagetool-x86_64.AppImage
chmod +x appimagetool-x86_64.AppImage
./appimagetool-x86_64.AppImage CRUDExplorer.AppDir CRUDExplorer-x86_64.AppImage
```

#### 2. tar.gz パッケージ作成

```bash
cd publish/linux-x64
tar -czf ../CRUDExplorer-linux-x64-v1.0.0.tar.gz *
```

---

## HTTPS/SSL 設定

### Nginx リバースプロキシ + Let's Encrypt

#### 1. Nginx インストール

```bash
sudo apt update
sudo apt install nginx certbot python3-certbot-nginx
```

#### 2. Nginx 設定

`/etc/nginx/sites-available/crudexplorer-authserver` を作成：

```nginx
server {
    listen 80;
    server_name auth.example.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

シンボリックリンク作成と有効化：

```bash
sudo ln -s /etc/nginx/sites-available/crudexplorer-authserver /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

#### 3. Let's Encrypt SSL 証明書取得

```bash
sudo certbot --nginx -d auth.example.com
```

自動更新設定：

```bash
sudo certbot renew --dry-run
```

---

## 本番環境セキュリティ設定

### 1. AdminController の [Authorize] 属性有効化

**重要**: 現在、統合テスト用に `AdminController` の `[Authorize]` 属性がコメントアウトされています。本番環境では必ず有効化してください。

`src/CRUDExplorer.AuthServer/Controllers/AdminController.cs`:

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]  // ← このコメントを外す
public class AdminController : ControllerBase
{
    // ...
}
```

### 2. CORS 設定強化

`Program.cs` で特定のオリジンのみ許可：

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionPolicy", policy =>
    {
        policy.WithOrigins("https://yourapp.example.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ...

app.UseCors("ProductionPolicy");
```

### 3. レート制限設定

`AspNetCoreRateLimit` パッケージを使用：

```bash
dotnet add package AspNetCoreRateLimit
```

```csharp
// Program.cs
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "*",
            Limit = 100,
            Period = "1m"
        }
    };
});
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// ...

app.UseIpRateLimiting();
```

### 4. データベース接続文字列の暗号化

環境変数または Azure Key Vault / AWS Secrets Manager を使用し、設定ファイルに平文で保存しないようにしてください。

### 5. ロギング設定

Serilog を使用した構造化ロギング：

```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
```

```csharp
// Program.cs
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/authserver-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
```

---

## トラブルシューティング

### 問題: AuthServer が起動しない

**原因**: PostgreSQL に接続できない

**解決策**:
1. PostgreSQL サービスが起動しているか確認
   ```bash
   sudo systemctl status postgresql
   ```
2. 接続文字列が正しいか確認
3. ファイアウォール設定を確認
4. PostgreSQL のログを確認
   ```bash
   sudo tail -f /var/log/postgresql/postgresql-15-main.log
   ```

---

### 問題: JWT トークンが無効

**原因**: `Jwt:SecretKey` が異なる、またはトークンの有効期限切れ

**解決策**:
1. `appsettings.Production.json` の `Jwt:SecretKey` が正しいか確認
2. トークンの有効期限（`ExpirationMinutes`）を確認
3. サーバーの時刻が正確か確認（NTP 同期）

---

### 問題: データベースマイグレーションエラー

**原因**: EF Core マイグレーションファイルが適用されていない

**解決策**:
```bash
cd src/CRUDExplorer.AuthServer
dotnet ef database update
```

---

### 問題: クライアントアプリが認証サーバーに接続できない

**原因**: ファイアウォール、CORS、またはネットワーク設定

**解決策**:
1. クライアントの設定ファイルで認証サーバーのURLが正しいか確認
2. CORS 設定を確認（開発時は `AllowAnyOrigin()` で動作確認）
3. ファイアウォールでポート 5000 (または設定したポート) が開いているか確認
   ```bash
   sudo ufw allow 5000/tcp
   ```

---

### 問題: Swagger UI が表示されない

**原因**: 本番環境で Swagger が無効化されている

**解決策**:
`Program.cs` で環境に応じて Swagger を有効化：

```csharp
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

本番環境でも Swagger を有効にする場合は条件を削除してください（セキュリティリスクに注意）。

---

## GitHub Releases でのリリース

### 1. タグ作成

```bash
git tag -a v1.0.0 -m "Release version 1.0.0"
git push origin v1.0.0
```

### 2. GitHub Releases ページでリリース作成

1. GitHub リポジトリの "Releases" タブに移動
2. "Draft a new release" をクリック
3. タグを選択（v1.0.0）
4. リリースタイトル: "CRUDExplorer v1.0.0"
5. リリースノート記載
6. ビルド済みバイナリをアップロード：
   - `CRUDExplorer-win-x64-v1.0.0.zip`
   - `CRUDExplorer-v1.0.0.dmg`
   - `CRUDExplorer-linux-x64-v1.0.0.tar.gz`
   - `CRUDExplorer-x86_64.AppImage`
7. "Publish release" をクリック

---

## まとめ

本ドキュメントでは、CRUDExplorer のデプロイ手順を説明しました。

**重要なチェックリスト**:
- [ ] PostgreSQL セットアップ完了
- [ ] JWT 秘密鍵を強力なランダム文字列に変更
- [ ] `AdminController` の `[Authorize]` 属性を有効化
- [ ] CORS 設定を本番環境用に制限
- [ ] HTTPS/SSL 設定完了
- [ ] データベースバックアップ設定
- [ ] ログ監視設定
- [ ] レート制限設定（オプション）

ご質問や問題がある場合は、[GitHub Issues](https://github.com/satoshikosugi/CRUDExplorer/issues) で報告してください。
