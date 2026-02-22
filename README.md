# CRUDExplorer

SQLファイルからCRUD操作（Create/Read/Update/Delete）を自動抽出し、テーブル×処理のマトリクスを生成するクロスプラットフォーム対応ツール

[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Avalonia UI](https://img.shields.io/badge/Avalonia-11.3-purple)](https://avaloniaui.net/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

## 📋 概要

CRUDExplorerは、大規模なSQLスクリプト群から以下の情報を自動抽出します：

- **CRUD操作の検出**: SELECT/INSERT/UPDATE/DELETE文からテーブルアクセスを抽出
- **マトリクス生成**: テーブル×プログラム/機能のCRUDマトリクスを自動生成
- **複数DB対応**: PostgreSQL、MySQL、SQL Server、Oracle、SQLite、MariaDBに対応
- **クロスプラットフォーム**: Windows、macOS、Linuxで動作

### 主要機能

✅ **SQL解析エンジン**: ANTLR4ベースの高精度パーサー（複雑なサブクエリ、JOIN、UNION対応）
✅ **データベース対応**: 6種類のRDBMS + 4種類のクラウドDB（計画中）
✅ **UIフレンドリー**: Avalonia UI による直感的なGUI（全13画面）
✅ **ライセンス管理**: ASP.NET Core認証サーバーによるライセンス認証
✅ **エクスポート**: CSV、Excel、HTML形式でのマトリクス出力

## 🚀 クイックスタート

### システム要件

- **.NET 8 Runtime** または SDK
- **OS**: Windows 10/11、macOS 12以降、Ubuntu 22.04以降
- **メモリ**: 最小 2GB RAM（推奨 4GB以上）
- **ディスク**: 200MB以上の空き容量

### インストール

#### Windows

1. [リリースページ](../../releases)から最新の`CRUDExplorer-win-x64.zip`をダウンロード
2. ZIPファイルを展開
3. `CRUDExplorer.UI.exe`を実行

#### macOS

1. [リリースページ](../../releases)から最新の`CRUDExplorer-osx-x64.zip`をダウンロード
2. ZIPファイルを展開
3. ターミナルで以下を実行:
```bash
chmod +x CRUDExplorer.UI
./CRUDExplorer.UI
```

#### Linux

1. [リリースページ](../../releases)から最新の`CRUDExplorer-linux-x64.tar.gz`をダウンロード
2. ターミナルで以下を実行:
```bash
tar -xzf CRUDExplorer-linux-x64.tar.gz
cd CRUDExplorer
chmod +x CRUDExplorer.UI
./CRUDExplorer.UI
```

### ソースからビルド

```bash
# リポジトリをクローン
git clone https://github.com/satoshikosugi/CRUDExplorer.git
cd CRUDExplorer

# ビルド
dotnet build CRUDExplorer.slnx

# 実行
dotnet run --project src/CRUDExplorer.UI/CRUDExplorer.UI.csproj
```

## 📖 使い方

### 1. CRUD解析の基本手順

1. **メイン画面起動**: `CRUDExplorer.UI`を起動
2. **対象フォルダ選択**: 「フォルダ選択」ボタンでSQLファイルを含むディレクトリを選択
3. **解析実行**: 「CRUD解析開始」ボタンをクリック
4. **結果確認**: マトリクス表示画面で結果を確認
5. **エクスポート**: 「エクスポート」から CSV/Excel/HTML 形式で保存

### 2. データベース接続設定

**対応データベース:**
- PostgreSQL 12+
- MySQL 8.0+
- SQL Server 2019+
- Oracle 19c+
- SQLite 3.x
- MariaDB 10.5+

**接続文字列例:**

```
# PostgreSQL
Host=localhost;Port=5432;Database=mydb;Username=user;Password=pass

# MySQL
Server=localhost;Port=3306;Database=mydb;Uid=user;Pwd=pass

# SQL Server
Server=localhost;Database=mydb;User Id=user;Password=pass;TrustServerCertificate=true

# Oracle
Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL)));User Id=user;Password=pass

# SQLite
Data Source=/path/to/database.db

# MariaDB
Server=localhost;Port=3306;Database=mydb;Uid=user;Pwd=pass
```

### 3. 設定画面

「設定」メニューから以下を設定可能:

- **外部エディタ**: ソースファイルを開くエディタ（Notepad、VSCode、Vim等）
- **プログラムIDパターン**: 正規表現でプログラム識別子を抽出
- **ダブルクリック動作**: ファイルリスト項目のダブルクリック動作
- **デバッグモード**: 詳細ログ出力の有効/無効

## 🏗️ アーキテクチャ

```
┌─────────────────────────────────────────────────────────┐
│                  CRUDExplorer.UI (Avalonia)             │
│  ┌─────────────┐  ┌─────────────┐  ┌────────────────┐  │
│  │  MainWindow │  │ MakeCrud    │  │ AnalyzeQuery   │  │
│  │             │  │ Window      │  │ Window         │  │
│  └─────────────┘  └─────────────┘  └────────────────┘  │
└────────────────────────────┬────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────┐
│              CRUDExplorer.Core (共通ライブラリ)         │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │  Models      │  │  Utilities   │  │  Database    │  │
│  │  (Query,     │  │  (String,    │  │  (Schema     │  │
│  │   CrudInfo)  │  │   FileSystem)│  │   Provider)  │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└────────────────────────────┬────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────┐
│         CRUDExplorer.SqlParser (ANTLR4パーサー)         │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │  Sql.g4      │  │  PostgreSql  │  │  MySql       │  │
│  │  (基本文法)  │  │  Dialect.g4  │  │  Dialect.g4  │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│       CRUDExplorer.AuthServer (認証サーバー)            │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │  License     │  │  Device      │  │  Audit       │  │
│  │  Controller  │  │  Management  │  │  Log         │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
```

### テクノロジースタック

- **UI**: Avalonia UI 11.3 (MVVM パターン)
- **パーサー**: ANTLR4 4.13.1
- **データベース**: Entity Framework Core 8.0
- **認証**: ASP.NET Core 8.0 + JWT Bearer
- **テスト**: xUnit + Avalonia.Headless

## 🧪 テスト

```bash
# 全テスト実行
dotnet test

# 個別プロジェクトテスト
dotnet test src/CRUDExplorer.Core.Tests/CRUDExplorer.Core.Tests.csproj
dotnet test src/CRUDExplorer.SqlParser.Tests/CRUDExplorer.SqlParser.Tests.csproj
dotnet test src/CRUDExplorer.IntegrationTests/CRUDExplorer.IntegrationTests.csproj
dotnet test src/CRUDExplorer.UI.Tests/CRUDExplorer.UI.Tests.csproj
```

**テスト結果（v1.0.0）:**
- Core Tests: 233/247 (94.3%)
- SqlParser Tests: 51/53 (96.2%)
- Integration Tests: 16/16 (100%)
- UI Tests: 13/13 (100%)
- **総計: 313/329 (95.1%)**

## 📚 ドキュメント

- [インストールガイド](docs/INSTALL.md) - 詳細なインストール手順
- [ユーザーマニュアル](docs/USER_GUIDE.md) - 機能詳細と使用方法
- [開発者ガイド](docs/DEVELOPER_GUIDE.md) - 開発環境セットアップ
- [API仕様書](docs/API.md) - AuthServer API リファレンス
- [MIGRATION_PLAN.md](MIGRATION_PLAN.md) - VB.NET→C#移行計画

## 🤝 コントリビューション

プルリクエストを歓迎します！詳細は [CONTRIBUTING.md](CONTRIBUTING.md) をご覧ください。

### 開発環境セットアップ

```bash
# 依存関係復元
dotnet restore

# ビルド
dotnet build

# テスト実行
dotnet test

# UI起動（開発モード）
dotnet run --project src/CRUDExplorer.UI/CRUDExplorer.UI.csproj
```

## 📄 ライセンス

このプロジェクトは [MIT License](LICENSE) の下で公開されています。

## 🙏 謝辞

- [Avalonia UI](https://avaloniaui.net/) - クロスプラットフォームUIフレームワーク
- [ANTLR](https://www.antlr.org/) - 高性能パーサージェネレーター
- [Entity Framework Core](https://docs.microsoft.com/ef/core/) - ORM
- [xUnit](https://xunit.net/) - テストフレームワーク

## 📞 サポート

- **Issues**: [GitHub Issues](../../issues)
- **Discussions**: [GitHub Discussions](../../discussions)
- **Email**: support@crudexplorer.example.com

---

**CRUDExplorer** - SQL解析を簡単に 🚀
