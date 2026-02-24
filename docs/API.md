# AuthServer API Documentation

CRUDExplorer認証サーバーのAPI仕様書

## 📋 概要

AuthServerは、CRUDExplorerクライアントアプリケーションのライセンス認証とデバイス管理を提供するASP.NET Core Web APIです。

- **ベースURL**: `https://your-domain.com/api`
- **認証方式**: JWT Bearer Token
- **レスポンス形式**: JSON
- **API バージョン**: v1.0

## 🔐 認証

### JWT トークン取得

ライセンスキーとデバイスIDでJWTトークンを取得します。

**エンドポイント**: `POST /api/license/authenticate`

**リクエストボディ**:
```json
{
  "licenseKey": "XXXX-XXXX-XXXX-XXXX",
  "deviceId": "unique-device-identifier",
  "deviceName": "My MacBook Pro"
}
```

**レスポンス（成功時）**:
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2026-03-22T12:00:00Z",
  "message": "Authentication successful"
}
```

**レスポンス（失敗時）**:
```json
{
  "success": false,
  "token": null,
  "expiresAt": null,
  "message": "Invalid license key"
}
```

**HTTPステータスコード**:
- `200 OK`: 認証成功
- `401 Unauthorized`: ライセンスキーが無効または期限切れ
- `409 Conflict`: 最大デバイス数超過
- `400 Bad Request`: リクエストが不正

---

### トークン検証

JWTトークンの有効性を確認します。

**エンドポイント**: `GET /api/license/validate`

**リクエストヘッダー**:
```
Authorization: Bearer {token}
```

**レスポンス**:
```json
{
  "valid": true
}
```

**HTTPステータスコード**:
- `200 OK`: トークン有効
- `401 Unauthorized`: トークン無効または期限切れ

---

## 👥 管理API

管理APIは認証が必要です（将来実装予定）。

### ユーザー一覧取得

**エンドポイント**: `GET /api/admin/users`

**クエリパラメータ**:
- `page` (optional): ページ番号（デフォルト: 1）
- `pageSize` (optional): 1ページあたりの件数（デフォルト: 20、最大: 100）

**レスポンス**:
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "emailAddress": "user@example.com",
    "isActive": true,
    "createdAt": "2026-01-15T10:30:00Z"
  }
]
```

---

### ライセンス一覧取得

**エンドポイント**: `GET /api/admin/licenses`

**クエリパラメータ**:
- `page` (optional): ページ番号
- `pageSize` (optional): 1ページあたりの件数

**レスポンス**:
```json
[
  {
    "id": "660e8400-e29b-41d4-a716-446655440000",
    "key": "ABCD-EFGH-IJKL-MNOP",
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "productType": "Professional",
    "maxDevices": 3,
    "isActive": true,
    "createdAt": "2026-01-20T14:00:00Z",
    "expiresAt": "2027-01-20T14:00:00Z"
  }
]
```

---

### ライセンス作成

**エンドポイント**: `POST /api/admin/licenses`

**リクエストボディ**:
```json
{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "emailAddress": "user@example.com",
  "maxDevices": 3,
  "productType": "Professional",
  "expiresAt": "2027-01-20T14:00:00Z"
}
```

**注**: `userId` または `emailAddress` のいずれかが必須です。

**レスポンス**:
```json
{
  "id": "660e8400-e29b-41d4-a716-446655440000",
  "key": "ABCD-EFGH-IJKL-MNOP",
  "message": "License created successfully"
}
```

**HTTPステータスコード**:
- `201 Created`: ライセンス作成成功
- `400 Bad Request`: リクエストが不正
- `404 Not Found`: ユーザーが見つからない

---

### ライセンス取り消し

**エンドポイント**: `PUT /api/admin/licenses/{id}/revoke`

**パスパラメータ**:
- `id`: ライセンスID（GUID）

**レスポンス**:
```json
{
  "message": "License revoked successfully"
}
```

**HTTPステータスコード**:
- `200 OK`: 取り消し成功
- `404 Not Found`: ライセンスが見つからない

---

### デバイスアクティベーション一覧

**エンドポイント**: `GET /api/admin/devices`

**クエリパラメータ**:
- `page` (optional): ページ番号
- `pageSize` (optional): 1ページあたりの件数

**レスポンス**:
```json
[
  {
    "id": "770e8400-e29b-41d4-a716-446655440000",
    "deviceId": "unique-device-id",
    "deviceName": "MacBook Pro",
    "licenseKey": "ABCD-EFGH-IJKL-MNOP",
    "emailAddress": "user@example.com",
    "activatedAt": "2026-01-25T09:00:00Z",
    "lastSeenAt": "2026-02-20T15:30:00Z"
  }
]
```

---

### デバイス無効化

**エンドポイント**: `DELETE /api/admin/devices/{id}`

**パスパラメータ**:
- `id`: デバイスアクティベーションID（GUID）

**レスポンス**:
```json
{
  "message": "Device deactivated successfully"
}
```

**HTTPステータスコード**:
- `200 OK`: 無効化成功
- `404 Not Found`: デバイスが見つからない

---

### 監査ログ取得

**エンドポイント**: `GET /api/admin/audit-logs`

**クエリパラメータ**:
- `page` (optional): ページ番号
- `pageSize` (optional): 1ページあたりの件数
- `userId` (optional): ユーザーIDでフィルタ

**レスポンス**:
```json
[
  {
    "id": "880e8400-e29b-41d4-a716-446655440000",
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "action": "LicenseValidated",
    "timestamp": "2026-02-22T10:15:00Z",
    "ipAddress": "192.168.1.100",
    "details": "{\"licenseKey\":\"ABCD-EFGH-IJKL-MNOP\",\"deviceId\":\"unique-id\"}"
  }
]
```

---

## 🔢 エラーコード

### HTTPステータスコード

| コード | 説明 |
|--------|------|
| 200 | 成功（OK） |
| 201 | 作成成功（Created） |
| 400 | リクエストが不正（Bad Request） |
| 401 | 認証失敗（Unauthorized） |
| 404 | リソースが見つからない（Not Found） |
| 409 | 競合（Conflict）- 最大デバイス数超過 |
| 500 | サーバーエラー（Internal Server Error） |

### エラーコード（ErrorCode フィールド）

| コード | 説明 |
|--------|------|
| INVALID_LICENSE | ライセンスキーが無効 |
| EXPIRED_LICENSE | ライセンスが期限切れ |
| MAX_DEVICES_EXCEEDED | 最大デバイス数超過 |
| INACTIVE_LICENSE | ライセンスが無効化されている |

---

## 📝 データモデル

### ライセンスキー形式

ライセンスキーは以下の形式である必要があります:
- フォーマット: `XXXX-XXXX-XXXX-XXXX`
- 各セグメント: 英数字4文字
- 例: `ABCD-EFGH-IJKL-MNOP`, `TEST-VALI-DLIC-EN01`

### プロダクトタイプ

- `Standard`: 標準版（1デバイス）
- `Professional`: プロフェッショナル版（3デバイス）
- `Enterprise`: エンタープライズ版（10デバイス）

---

## 🔒 セキュリティ

### JWT トークン

- **アルゴリズム**: HS256
- **有効期限**: 24時間（デフォルト）
- **クレーム**:
  - `sub`: ユーザーID
  - `email`: メールアドレス
  - `licenseId`: ライセンスID
  - `exp`: 有効期限
  - `iat`: 発行日時

### ベストプラクティス

1. **HTTPS必須**: 本番環境では必ずHTTPSを使用
2. **トークン保管**: トークンは安全な場所に保管（KeychainまたはSecureStorage）
3. **有効期限確認**: トークンの有効期限を定期的に確認
4. **ライセンスキー保護**: ライセンスキーをハードコードしない

---

## 🚀 サンプルコード

### C# (.NET)

```csharp
using System.Net.Http;
using System.Net.Http.Json;

public class AuthServerClient
{
    private readonly HttpClient _httpClient;

    public AuthServerClient(string baseUrl)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public async Task<AuthenticationResponse> AuthenticateAsync(
        string licenseKey, string deviceId, string deviceName)
    {
        var request = new
        {
            LicenseKey = licenseKey,
            DeviceId = deviceId,
            DeviceName = deviceName
        };

        var response = await _httpClient.PostAsJsonAsync(
            "/api/license/authenticate", request);

        return await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync("/api/license/validate");
        return response.IsSuccessStatusCode;
    }
}
```

---

## 📞 サポート

APIに関する質問や問題は以下までお問い合わせください:

- **GitHub Issues**: [https://github.com/satoshikosugi/CRUDExplorer/issues](../../issues)
- **Email**: api-support@crudexplorer.example.com

---

**AuthServer API v1.0** - CRUDExplorer License Management 🔐
