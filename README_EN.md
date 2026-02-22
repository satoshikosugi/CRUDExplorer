# CRUDExplorer

A cross-platform tool that automatically extracts CRUD operations (Create/Read/Update/Delete) from SQL files and generates table×process matrices

[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Avalonia UI](https://img.shields.io/badge/Avalonia-11.3-purple)](https://avaloniaui.net/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

[日本語版README](README.md) | **English**

## 📋 Overview

CRUDExplorer automatically extracts the following information from large SQL script collections:

- **CRUD Operation Detection**: Extracts table access from SELECT/INSERT/UPDATE/DELETE statements
- **Matrix Generation**: Automatically generates Table×Program/Function CRUD matrices
- **Multi-DB Support**: Supports PostgreSQL, MySQL, SQL Server, Oracle, SQLite, MariaDB
- **Cross-Platform**: Runs on Windows, macOS, and Linux

### Key Features

✅ **SQL Parsing Engine**: High-precision ANTLR4-based parser (supports complex subqueries, JOINs, UNION)
✅ **Database Support**: 6 RDBMS types + 4 cloud DB types (planned)
✅ **User-Friendly UI**: Intuitive GUI with Avalonia UI (13 windows total)
✅ **License Management**: License authentication via ASP.NET Core auth server
✅ **Export**: Matrix output in CSV, Excel, and HTML formats

## 🚀 Quick Start

### System Requirements

- **.NET 8 Runtime** or SDK
- **OS**: Windows 10/11, macOS 12+, Ubuntu 22.04+
- **Memory**: Minimum 2GB RAM (Recommended 4GB+)
- **Disk**: 200MB+ free space

### Installation

#### Windows

1. Download the latest `CRUDExplorer-win-x64.zip` from [Releases](../../releases)
2. Extract the ZIP file
3. Run `CRUDExplorer.UI.exe`

#### macOS

1. Download the latest `CRUDExplorer-osx-x64.zip` from [Releases](../../releases)
2. Extract the ZIP file
3. Run in terminal:
```bash
chmod +x CRUDExplorer.UI
./CRUDExplorer.UI
```

#### Linux

1. Download the latest `CRUDExplorer-linux-x64.tar.gz` from [Releases](../../releases)
2. Run in terminal:
```bash
tar -xzf CRUDExplorer-linux-x64.tar.gz
cd CRUDExplorer
chmod +x CRUDExplorer.UI
./CRUDExplorer.UI
```

### Build from Source

```bash
# Clone repository
git clone https://github.com/satoshikosugi/CRUDExplorer.git
cd CRUDExplorer

# Build
dotnet build CRUDExplorer.slnx

# Run
dotnet run --project src/CRUDExplorer.UI/CRUDExplorer.UI.csproj
```

## 📖 Usage

### 1. Basic CRUD Analysis Steps

1. **Launch Main Window**: Run `CRUDExplorer.UI`
2. **Select Target Folder**: Click "Select Folder" button to choose directory containing SQL files
3. **Execute Analysis**: Click "Start CRUD Analysis" button
4. **View Results**: Check results in the matrix display screen
5. **Export**: Save in CSV/Excel/HTML format from "Export" menu

### 2. Database Connection Settings

**Supported Databases:**
- PostgreSQL 12+
- MySQL 8.0+
- SQL Server 2019+
- Oracle 19c+
- SQLite 3.x
- MariaDB 10.5+

**Connection String Examples:**

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

### 3. Settings Window

Configure the following from the "Settings" menu:

- **External Editor**: Editor to open source files (Notepad, VSCode, Vim, etc.)
- **Program ID Pattern**: Extract program identifiers via regular expression
- **Double-Click Action**: Behavior when double-clicking file list items
- **Debug Mode**: Enable/disable detailed log output

## 🏗️ Architecture

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
│          CRUDExplorer.Core (Common Library)             │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │  Models      │  │  Utilities   │  │  Database    │  │
│  │  (Query,     │  │  (String,    │  │  (Schema     │  │
│  │   CrudInfo)  │  │   FileSystem)│  │   Provider)  │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└────────────────────────────┬────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────┐
│        CRUDExplorer.SqlParser (ANTLR4 Parser)           │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │  Sql.g4      │  │  PostgreSql  │  │  MySql       │  │
│  │  (Base       │  │  Dialect.g4  │  │  Dialect.g4  │  │
│  │   Grammar)   │  │              │  │              │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│     CRUDExplorer.AuthServer (Authentication Server)     │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │  License     │  │  Device      │  │  Audit       │  │
│  │  Controller  │  │  Management  │  │  Log         │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
```

### Technology Stack

- **UI**: Avalonia UI 11.3 (MVVM Pattern)
- **Parser**: ANTLR4 4.13.1
- **Database**: Entity Framework Core 8.0
- **Authentication**: ASP.NET Core 8.0 + JWT Bearer
- **Testing**: xUnit + Avalonia.Headless

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run individual project tests
dotnet test src/CRUDExplorer.Core.Tests/CRUDExplorer.Core.Tests.csproj
dotnet test src/CRUDExplorer.SqlParser.Tests/CRUDExplorer.SqlParser.Tests.csproj
dotnet test src/CRUDExplorer.IntegrationTests/CRUDExplorer.IntegrationTests.csproj
dotnet test src/CRUDExplorer.UI.Tests/CRUDExplorer.UI.Tests.csproj
```

**Test Results (v1.0.0):**
- Core Tests: 147/147 (100%)
- SqlParser Tests: 51/53 (96.2%)
- Integration Tests: 16/16 (100%)
- UI Tests: 13/13 (100%)
- **Total: 227/229 (99.1%)**

## 📚 Documentation

- [Installation Guide](docs/INSTALL.md) - Detailed installation instructions
- [User Manual](docs/USER_GUIDE.md) - Feature details and usage
- [Developer Guide](docs/DEVELOPER_GUIDE.md) - Development environment setup
- [API Reference](docs/API.md) - AuthServer API documentation
- [MIGRATION_PLAN.md](MIGRATION_PLAN.md) - VB.NET→C# migration plan

## 🤝 Contributing

Pull requests are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for details.

### Development Environment Setup

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build

# Run tests
dotnet test

# Launch UI (development mode)
dotnet run --project src/CRUDExplorer.UI/CRUDExplorer.UI.csproj
```

## 📄 License

This project is released under the [MIT License](LICENSE).

## 🙏 Acknowledgments

- [Avalonia UI](https://avaloniaui.net/) - Cross-platform UI framework
- [ANTLR](https://www.antlr.org/) - High-performance parser generator
- [Entity Framework Core](https://docs.microsoft.com/ef/core/) - ORM
- [xUnit](https://xunit.net/) - Testing framework

## 📞 Support

- **Issues**: [GitHub Issues](../../issues)
- **Discussions**: [GitHub Discussions](../../discussions)
- **Email**: support@crudexplorer.example.com

---

**CRUDExplorer** - SQL Analysis Made Easy 🚀
