# コントリビューションガイド / Contributing Guide

CRUDExplorerへのコントリビューションに興味を持っていただき、ありがとうございます！

Thank you for your interest in contributing to CRUDExplorer!

[English version below](#english-version)

---

## 日本語版

### 目次

- [行動規範](#行動規範)
- [開発環境のセットアップ](#開発環境のセットアップ)
- [プロジェクト構成](#プロジェクト構成)
- [コーディング規約](#コーディング規約)
- [テストの書き方](#テストの書き方)
- [プルリクエストの出し方](#プルリクエストの出し方)
- [バグ報告](#バグ報告)
- [機能要望](#機能要望)

### 行動規範

このプロジェクトは[Contributor Covenant](https://www.contributor-covenant.org/)の行動規範を採用しています。参加することで、あなたはこの規範を守ることに同意したとみなされます。

### 開発環境のセットアップ

#### 必要なツール

- **.NET 8 SDK** または .NET 10 SDK
- **Git**
- **IDE**: Visual Studio 2022、JetBrains Rider、または VS Code（C# Dev Kit拡張機能）

#### クローンとビルド

```bash
# リポジトリをクローン
git clone https://github.com/satoshikosugi/CRUDExplorer.git
cd CRUDExplorer

# 依存関係の復元
dotnet restore

# ビルド
dotnet build

# テスト実行
dotnet test

# UI起動（開発モード）
dotnet run --project src/CRUDExplorer.UI/CRUDExplorer.UI.csproj
```

#### データベースのセットアップ（AuthServer開発時）

```bash
cd src/CRUDExplorer.AuthServer

# PostgreSQLを起動（Dockerを使用する場合）
docker run --name crudexplorer-postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:16

# マイグレーション適用
dotnet ef database update

# AuthServer起動
dotnet run
```

Swagger UIは `https://localhost:5001/swagger` でアクセス可能です。

### プロジェクト構成

```
CRUDExplorer/
├── src/
│   ├── CRUDExplorer.Core/          # コアロジック・データモデル・ユーティリティ
│   ├── CRUDExplorer.SqlParser/     # ANTLR4ベースSQLパーサー
│   ├── CRUDExplorer.UI/            # Avalonia UIアプリケーション
│   ├── CRUDExplorer.AuthServer/    # ASP.NET Core認証サーバー
│   ├── CRUDExplorer.Core.Tests/    # Coreユニットテスト
│   ├── CRUDExplorer.SqlParser.Tests/ # SQLパーサーテスト
│   ├── CRUDExplorer.IntegrationTests/ # AuthServer統合テスト
│   └── CRUDExplorer.UI.Tests/      # UI統合テスト
├── docs/                            # ドキュメント
│   └── API.md                      # AuthServer API仕様書
├── README.md                        # プロジェクト概要（日本語）
├── README_EN.md                     # プロジェクト概要（英語）
├── MIGRATION_PLAN.md               # VB.NET→C#移行計画
└── CONTRIBUTING.md                  # このファイル
```

### アーキテクチャ概要

CRUDExplorerは以下の4層アーキテクチャで構成されています：

1. **UI層 (Avalonia UI)**
   - MVVM パターン
   - CommunityToolkit.Mvvm使用（ObservableProperty、RelayCommand）
   - 13ウィンドウ実装済み

2. **コアロジック層 (CRUDExplorer.Core)**
   - CRUD抽出ロジック
   - データモデル（Query、Column、CrudInfo等）
   - ユーティリティクラス（StringUtilities、FileSystemHelper等）
   - データベース接続管理

3. **SQLパーサー層 (CRUDExplorer.SqlParser)**
   - ANTLR4 4.13.1ベースのパーサー
   - SQL方言対応（PostgreSQL、MySQL、SQL Server、Oracle）
   - ビジターパターンによるAST走査

4. **認証サーバー層 (CRUDExplorer.AuthServer)**
   - ASP.NET Core 8.0 Web API
   - Entity Framework Core + PostgreSQL
   - JWT Bearer認証

### コーディング規約

#### C#スタイル

- **.NET コーディング規約**に従う（PascalCase、camelCase等）
- **nullable参照型**を有効化（`<Nullable>enable</Nullable>`）
- **暗黙的using**を使用（`<ImplicitUsings>enable</ImplicitUsings>`）

#### コメント

- **日本語コメント推奨**（このプロジェクトは日本語がメイン言語です）
- XMLドキュメントコメント（`/// <summary>`）をpublicメンバーに記述
- 複雑なロジックには説明コメントを追加

#### 命名規則

- **クラス名**: PascalCase（例: `SqlAnalyzer`、`QueryFormatter`）
- **メソッド名**: PascalCase（例: `AnalyzeSql`、`GetCRUD`）
- **プライベートフィールド**: camelCase with `_` prefix（例: `_analyzer`、`_query`）
- **プロパティ**: PascalCase（例: `QueryKind`、`TableName`）

### テストの書き方

#### ユニットテスト（xUnit）

```csharp
using Xunit;
using CRUDExplorer.Core.Utilities;

namespace CRUDExplorer.Core.Tests;

public class StringUtilitiesTests
{
    [Fact]
    public void RemoveComments_SingleLineComment_RemovesCorrectly()
    {
        // Arrange
        var input = "SELECT * FROM users -- comment";

        // Act
        var result = StringUtilities.RemoveComments(input);

        // Assert
        Assert.Equal("SELECT * FROM users ", result);
    }

    [Theory]
    [InlineData("test", "TEST")]
    [InlineData("Test", "TEST")]
    public void ToUpper_ConvertsCorrectly(string input, string expected)
    {
        Assert.Equal(expected, input.ToUpper());
    }
}
```

#### UI統合テスト（Avalonia.Headless）

```csharp
using Avalonia.Headless.XUnit;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Tests;

public class ViewModelTests
{
    [AvaloniaFact]
    public void MainWindowViewModel_Initializes_Successfully()
    {
        // Arrange & Act
        var viewModel = new MainWindowViewModel();

        // Assert
        Assert.NotNull(viewModel);
        Assert.NotNull(viewModel.SettingsCommand);
    }
}
```

#### テスト実行

```bash
# 全テスト実行
dotnet test

# 特定プロジェクトのテスト
dotnet test src/CRUDExplorer.Core.Tests/CRUDExplorer.Core.Tests.csproj

# カバレッジ付き実行
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### プルリクエストの出し方

1. **Issueの確認**: 既存のIssueを確認し、重複がないかチェック
2. **ブランチ作成**: `feature/issue-123-description` または `fix/issue-123-description`
3. **コミットメッセージ**: 明確で簡潔に（日本語推奨）
   ```
   フェーズ7.1完了: CRUD抽出テスト100ケース実装

   - CrudExtractionTests.cs 作成（100テストケース）
   - テーブルレベルCRUD抽出テスト: 50ケース
   - カラムレベルCRUD抽出テスト: 50ケース
   ```
4. **テスト**: PRを出す前に全テストが成功することを確認
5. **ドキュメント更新**: 必要に応じてREADMEやドキュメントを更新
6. **PRテンプレート**: 以下の情報を含める
   - 変更内容の概要
   - 関連Issue番号（`Closes #123`）
   - テスト結果
   - スクリーンショット（UI変更の場合）

### PRレビュー基準

- ✅ 全テストがパス
- ✅ コーディング規約に準拠
- ✅ 適切なテストが追加されている
- ✅ ドキュメントが更新されている（必要な場合）
- ✅ コミットメッセージが明確

### バグ報告

バグを見つけた場合は、[GitHub Issues](https://github.com/satoshikosugi/CRUDExplorer/issues)で報告してください。

**含めるべき情報:**
- **環境**: OS、.NETバージョン
- **再現手順**: バグを再現する具体的な手順
- **期待される動作**: 本来どうあるべきか
- **実際の動作**: 実際に何が起こったか
- **スクリーンショット**: あれば添付
- **ログ**: エラーメッセージやスタックトレース

### 機能要望

新機能の提案は[GitHub Discussions](https://github.com/satoshikosugi/CRUDExplorer/discussions)で歓迎します。

**含めるべき情報:**
- **ユースケース**: なぜこの機能が必要か
- **提案内容**: どのような機能か
- **代替案**: 他に検討した方法はあるか

### 対応しているデータベース

現在サポート済み（Phase 6.1-6.2完了）:
- ✅ PostgreSQL
- ✅ MySQL
- ✅ SQL Server
- ✅ Oracle
- ✅ SQLite
- ✅ MariaDB

今後対応予定（Phase 6.3）:
- ⏳ Snowflake
- ⏳ Google BigQuery
- ⏳ Databricks
- ⏳ Amazon Redshift

### ライセンス

このプロジェクトは[MIT License](LICENSE)の下で公開されています。コントリビューションもMITライセンスの下で公開されることに同意したとみなされます。

### 質問・サポート

- **GitHub Discussions**: 一般的な質問や議論
- **GitHub Issues**: バグ報告や機能要望
- **メール**: satoshi.kosugi@example.com（メンテナー）

---

## English Version

### Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Development Setup](#development-setup)
- [Project Structure](#project-structure-1)
- [Coding Conventions](#coding-conventions)
- [Writing Tests](#writing-tests)
- [Submitting Pull Requests](#submitting-pull-requests)
- [Bug Reports](#bug-reports)
- [Feature Requests](#feature-requests)

### Code of Conduct

This project adopts the [Contributor Covenant](https://www.contributor-covenant.org/) Code of Conduct. By participating, you are expected to uphold this code.

### Development Setup

#### Required Tools

- **.NET 8 SDK** or .NET 10 SDK
- **Git**
- **IDE**: Visual Studio 2022, JetBrains Rider, or VS Code (with C# Dev Kit extension)

#### Clone and Build

```bash
# Clone repository
git clone https://github.com/satoshikosugi/CRUDExplorer.git
cd CRUDExplorer

# Restore dependencies
dotnet restore

# Build
dotnet build

# Run tests
dotnet test

# Launch UI (development mode)
dotnet run --project src/CRUDExplorer.UI/CRUDExplorer.UI.csproj
```

#### Database Setup (for AuthServer development)

```bash
cd src/CRUDExplorer.AuthServer

# Start PostgreSQL (using Docker)
docker run --name crudexplorer-postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:16

# Apply migrations
dotnet ef database update

# Run AuthServer
dotnet run
```

Swagger UI is available at `https://localhost:5001/swagger`.

### Project Structure

```
CRUDExplorer/
├── src/
│   ├── CRUDExplorer.Core/          # Core logic, data models, utilities
│   ├── CRUDExplorer.SqlParser/     # ANTLR4-based SQL parser
│   ├── CRUDExplorer.UI/            # Avalonia UI application
│   ├── CRUDExplorer.AuthServer/    # ASP.NET Core authentication server
│   ├── CRUDExplorer.Core.Tests/    # Core unit tests
│   ├── CRUDExplorer.SqlParser.Tests/ # SQL parser tests
│   ├── CRUDExplorer.IntegrationTests/ # AuthServer integration tests
│   └── CRUDExplorer.UI.Tests/      # UI integration tests
├── docs/                            # Documentation
│   └── API.md                      # AuthServer API specification
├── README.md                        # Project overview (Japanese)
├── README_EN.md                     # Project overview (English)
├── MIGRATION_PLAN.md               # VB.NET→C# migration plan
└── CONTRIBUTING.md                  # This file
```

### Architecture Overview

CRUDExplorer follows a 4-layer architecture:

1. **UI Layer (Avalonia UI)**
   - MVVM pattern
   - Uses CommunityToolkit.Mvvm (ObservableProperty, RelayCommand)
   - 13 windows implemented

2. **Core Logic Layer (CRUDExplorer.Core)**
   - CRUD extraction logic
   - Data models (Query, Column, CrudInfo, etc.)
   - Utility classes (StringUtilities, FileSystemHelper, etc.)
   - Database connection management

3. **SQL Parser Layer (CRUDExplorer.SqlParser)**
   - ANTLR4 4.13.1-based parser
   - SQL dialect support (PostgreSQL, MySQL, SQL Server, Oracle)
   - Visitor pattern for AST traversal

4. **Authentication Server Layer (CRUDExplorer.AuthServer)**
   - ASP.NET Core 8.0 Web API
   - Entity Framework Core + PostgreSQL
   - JWT Bearer authentication

### Coding Conventions

#### C# Style

- Follow **.NET coding conventions** (PascalCase, camelCase, etc.)
- Enable **nullable reference types** (`<Nullable>enable</Nullable>`)
- Use **implicit usings** (`<ImplicitUsings>enable</ImplicitUsings>`)

#### Comments

- **Japanese comments are preferred** (this project's primary language is Japanese)
- Add XML documentation comments (`/// <summary>`) for public members
- Add explanatory comments for complex logic

#### Naming Conventions

- **Class names**: PascalCase (e.g., `SqlAnalyzer`, `QueryFormatter`)
- **Method names**: PascalCase (e.g., `AnalyzeSql`, `GetCRUD`)
- **Private fields**: camelCase with `_` prefix (e.g., `_analyzer`, `_query`)
- **Properties**: PascalCase (e.g., `QueryKind`, `TableName`)

### Writing Tests

#### Unit Tests (xUnit)

```csharp
using Xunit;
using CRUDExplorer.Core.Utilities;

namespace CRUDExplorer.Core.Tests;

public class StringUtilitiesTests
{
    [Fact]
    public void RemoveComments_SingleLineComment_RemovesCorrectly()
    {
        // Arrange
        var input = "SELECT * FROM users -- comment";

        // Act
        var result = StringUtilities.RemoveComments(input);

        // Assert
        Assert.Equal("SELECT * FROM users ", result);
    }

    [Theory]
    [InlineData("test", "TEST")]
    [InlineData("Test", "TEST")]
    public void ToUpper_ConvertsCorrectly(string input, string expected)
    {
        Assert.Equal(expected, input.ToUpper());
    }
}
```

#### UI Integration Tests (Avalonia.Headless)

```csharp
using Avalonia.Headless.XUnit;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Tests;

public class ViewModelTests
{
    [AvaloniaFact]
    public void MainWindowViewModel_Initializes_Successfully()
    {
        // Arrange & Act
        var viewModel = new MainWindowViewModel();

        // Assert
        Assert.NotNull(viewModel);
        Assert.NotNull(viewModel.SettingsCommand);
    }
}
```

#### Running Tests

```bash
# Run all tests
dotnet test

# Run tests for specific project
dotnet test src/CRUDExplorer.Core.Tests/CRUDExplorer.Core.Tests.csproj

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Submitting Pull Requests

1. **Check Issues**: Review existing issues to avoid duplicates
2. **Create Branch**: `feature/issue-123-description` or `fix/issue-123-description`
3. **Commit Messages**: Clear and concise (Japanese preferred)
   ```
   Phase 7.1 complete: 100 CRUD extraction test cases

   - Created CrudExtractionTests.cs (100 test cases)
   - Table-level CRUD extraction tests: 50 cases
   - Column-level CRUD extraction tests: 50 cases
   ```
4. **Tests**: Ensure all tests pass before submitting PR
5. **Update Documentation**: Update README or docs if needed
6. **PR Template**: Include the following:
   - Summary of changes
   - Related issue number (`Closes #123`)
   - Test results
   - Screenshots (for UI changes)

### PR Review Criteria

- ✅ All tests pass
- ✅ Follows coding conventions
- ✅ Appropriate tests added
- ✅ Documentation updated (if needed)
- ✅ Clear commit messages

### Bug Reports

If you find a bug, please report it on [GitHub Issues](https://github.com/satoshikosugi/CRUDExplorer/issues).

**Information to include:**
- **Environment**: OS, .NET version
- **Steps to reproduce**: Specific steps to reproduce the bug
- **Expected behavior**: What should happen
- **Actual behavior**: What actually happened
- **Screenshots**: If available
- **Logs**: Error messages or stack traces

### Feature Requests

Feature suggestions are welcome on [GitHub Discussions](https://github.com/satoshikosugi/CRUDExplorer/discussions).

**Information to include:**
- **Use case**: Why is this feature needed?
- **Proposal**: What is the feature?
- **Alternatives**: Any other approaches considered?

### Supported Databases

Currently supported (Phase 6.1-6.2 complete):
- ✅ PostgreSQL
- ✅ MySQL
- ✅ SQL Server
- ✅ Oracle
- ✅ SQLite
- ✅ MariaDB

Planned support (Phase 6.3):
- ⏳ Snowflake
- ⏳ Google BigQuery
- ⏳ Databricks
- ⏳ Amazon Redshift

### License

This project is released under the [MIT License](LICENSE). By contributing, you agree that your contributions will be licensed under the MIT License.

### Questions & Support

- **GitHub Discussions**: General questions and discussions
- **GitHub Issues**: Bug reports and feature requests
- **Email**: satoshi.kosugi@example.com (Maintainer)

---

## 謝辞 / Acknowledgments

コントリビューターの皆様に感謝いたします！

Thank you to all contributors!
