# Changelog

All notable changes to the CRUDExplorer project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

### Phase 9.1 - Production Readiness Preparation (Partial) - 2026-02-22

#### Added
- **PRODUCTION_READINESS.md** (671 lines) - Comprehensive production deployment security checklist
  - Security configuration requirements (AdminController [Authorize], JWT secret keys, database credentials)
  - CORS production configuration guide
  - Logging setup (Serilog structured logging)
  - Database security settings (PostgreSQL permissions, SSL/TLS)
  - Additional security measures (rate limiting, security headers, Swagger UI control)
  - Pre-deployment checklist (mandatory/recommended/optional items)
  - Integration test modification guide for [Authorize] attribute

---

### Phase 8.3 - OpenAPI/Swagger Documentation - 2026-02-22

#### Added
- XML documentation for all AuthServer API endpoints (10 total endpoints)
- Swagger UI enhancements with complete API specifications
- `GenerateDocumentationFile` setting in CRUDExplorer.AuthServer.csproj
- XML comment file integration in Swagger configuration

#### Changed
- **LicenseController.cs** - Added comprehensive XML documentation for 3 endpoints
  - `POST /api/license/authenticate` - License authentication with sample requests
  - `GET /api/license/validate` - Token validation (GET method)
  - `POST /api/license/validate` - Token validation (POST method)
- **AdminController.cs** - Added comprehensive XML documentation for 7 endpoints
  - `POST /api/admin/licenses` - Create new license
  - `GET /api/admin/licenses` - List licenses (paginated)
  - `GET /api/admin/users` - List users (paginated)
  - `GET /api/admin/audit-logs` - Get audit logs (filtered, paginated)
  - `GET /api/admin/devices` - List device activations (paginated)
  - `DELETE /api/admin/devices/{id}` - Deactivate device
  - `PUT /api/admin/licenses/{id}/revoke` - Revoke license
- **Program.cs** - Enhanced Swagger configuration
  - Added project metadata (contact, license)
  - JWT Bearer authentication schema
  - XML comments integration

---

### Phase 8.2 - Developer Documentation (Partial) - 2026-02-22

#### Added
- **CONTRIBUTING.md** (650+ lines) - Bilingual (Japanese/English) developer contribution guide
  - Development environment setup instructions
  - Project architecture overview (4-layer structure)
  - Coding conventions (C# style, naming, comments)
  - Testing guidelines (xUnit + Avalonia.Headless)
  - Pull request process and review criteria
  - Bug report and feature request templates
  - Supported databases list (6 implemented, 4 planned)
  - MIT License information

- **docs/DATABASE_SUPPORT.md** (494 lines) - Database support status matrix
  - Detailed information for 6 implemented databases (PostgreSQL, MySQL, SQL Server, Oracle, SQLite, MariaDB)
  - Connection string examples and supported features
  - Test status for each database
  - NuGet package versions
  - Planned cloud database support (Snowflake, BigQuery, Databricks, Redshift)
  - SQL dialect compatibility matrix (ANTLR4 grammar)
  - Common infrastructure (connection pooling, error handling, connection string management)
  - Advanced SQL features not yet supported (16 failing tests analysis)

- **docs/ANTLR4_GRAMMAR_SPEC.md** (1,056 lines) - ANTLR4 grammar specification
  - Base SQL grammar specification (Sql.g4)
    - SELECT/INSERT/UPDATE/DELETE complete specifications
    - WITH clause (CTE), JOIN, WHERE, GROUP BY/HAVING, ORDER BY
    - 12 expression types with support status
  - PostgreSQL-specific extensions
    - RETURNING clause, ON CONFLICT (UPSERT)
    - LIMIT/OFFSET, JSON/JSONB operators
    - Type casting (:: operator, CAST function)
    - Array types, PostgreSQL-specific data types
  - MySQL-specific extensions
    - ON DUPLICATE KEY UPDATE, REPLACE statement
    - LIMIT syntax variations, UPDATE/DELETE with LIMIT
    - REGEXP operator, bitwise operators, XOR logical operator
    - CONVERT/BINARY, MySQL-specific data types
  - SQL Server/Oracle feature overview
  - SqlVisitor implementation overview (CRUD extraction logic)
  - Parser test results (51/53 passing - 96.2%)
  - Build and usage instructions with code examples
  - Future expansion plans (CTE, CASE, EXISTS, Window functions)

- **docs/DEPLOYMENT.md** (794 lines) - Comprehensive deployment guide
  - Docker deployment (Dockerfile + Docker Compose)
    - PostgreSQL container configuration
    - AuthServer container configuration
    - Environment variable examples (JWT secret key, DB connection strings)
  - Manual deployment procedures
    - Linux (systemd) and Windows (NSSM) service registration
    - PostgreSQL setup (Docker/manual)
    - Database migration execution
  - Client application distribution
    - Windows (self-contained/framework-dependent)
    - macOS (.app bundle + DMG creation)
    - Linux (AppImage + tar.gz)
  - HTTPS/SSL configuration (Nginx + Let's Encrypt)
  - Production security settings
    - AdminController [Authorize] attribute enablement
    - CORS production configuration
    - Rate limiting setup (AspNetCoreRateLimit)
    - Structured logging (Serilog)
  - Troubleshooting guide
  - GitHub Releases publication procedure

---

### Phase 8.1 - User Documentation - 2026-02-XX

#### Added
- **README.md** (366 lines) - Japanese project overview
  - Project overview with badges
  - System requirements and installation instructions (Windows/macOS/Linux)
  - Quick start guide
  - Usage instructions (CRUD analysis, database connection, settings)
  - Architecture diagram and technology stack
  - Test results summary
  - Build instructions and contribution guidelines

- **README_EN.md** - Complete English translation
  - Full README.md content in English
  - International user documentation

- **docs/API.md** (260 lines) - AuthServer API complete reference
  - Authentication endpoints
    - POST /api/license/authenticate - JWT token acquisition
    - GET /api/license/validate - Token validation
  - Admin API endpoints
    - User management (GET /api/admin/users)
    - License management (GET/POST/PUT /api/admin/licenses)
    - Device management (GET/DELETE /api/admin/devices)
    - Audit logs (GET /api/admin/audit-logs)
  - Request/response examples (JSON format)
  - HTTP status code list
  - Error code definitions
  - Security best practices
  - C# sample code (HttpClient usage)

---

### Phase 7.2 - Integration Tests - 2026-02-XX

#### Added
- **CRUDExplorer.IntegrationTests** project - AuthServer API integration tests
  - 16 test cases (all passing - 100%)
  - WebApplicationFactory + In-Memory database
  - License authentication flow tests (9 test cases)
    - Valid license authentication → JWT token acquisition
    - Invalid license → 401 Unauthorized
    - Expired license → 401 Unauthorized
    - Maximum device count exceeded → 409 Conflict
    - Same device multiple authentications → normal operation
    - Token validation (valid/invalid/none)
  - Admin API tests (7 test cases)
    - User list retrieval
    - License list retrieval
    - License creation
    - Device activation list
    - Device deactivation
    - Audit log retrieval (with filters)
    - License revocation

- **CRUDExplorer.UI.Tests** project - Avalonia UI integration tests
  - 13 test cases (all passing - 100%)
  - Avalonia.Headless 11.3.12 - Headless UI test execution
  - Avalonia.Headless.XUnit - xUnit integration
  - ViewModel initialization tests (10 test cases)
    - MainWindowViewModel, MakeCrudViewModel, AnalyzeQueryViewModel
    - SettingsViewModel, VersionViewModel, CrudSearchViewModel
    - FileListViewModel, FilterViewModel, GrepViewModel
    - TableDefinitionViewModel
  - Window initialization tests (3 test cases)
    - SettingsWindow initialization
    - ViewModel save command verification
    - Default settings load test

---

### Phase 7.1 - CRUD Extraction Tests - 2026-02-XX

#### Added
- **CrudExtractionTests.cs** - 100 new CRUD extraction test cases
  - Table-level CRUD extraction tests: 50 cases (42 passing)
    - SELECT, INSERT, UPDATE, DELETE basic operations
    - JOIN (INNER/LEFT/RIGHT/FULL/CROSS)
    - UNION/INTERSECT/MINUS
    - Subqueries (WHERE, FROM, EXISTS)
    - CTE (Common Table Expressions)
  - Column-level CRUD extraction tests: 50 cases (44 passing)
    - Column extraction for SELECT, INSERT, UPDATE, DELETE
    - WHERE, GROUP BY, HAVING, ORDER BY clauses
    - Aggregate functions, string functions, date functions
    - CASE expressions, arithmetic expressions, subqueries

#### Known Issues
- 16 tests failing due to advanced SQL features not yet implemented:
  - CTE (Common Table Expressions) - WITH clause
  - CASE expressions - Complex conditional logic
  - EXISTS/NOT EXISTS - Subquery existence checks
  - HAVING clause - Aggregate result filtering
  - TOP clause - SQL Server-specific syntax
  - Window Functions - RANK(), ROW_NUMBER(), etc.

---

### Phase 6.4 - Common Database Access Layer - 2026-02-XX

#### Added
- **ConnectionStringManager** - JSON-based connection string persistence
- **DatabaseException** hierarchy - 5 exception classes for database errors
- **DatabaseHelper** - Retry logic and connection string masking utilities
- **ConnectionPoolManager** - Semaphore-based connection pooling (max 10 connections per pool)

---

### Phase 6.1-6.2 - Database Support Implementation - 2026-02-XX

#### Added
- **PostgreSQL** database support
  - Connection factory and schema provider
  - Npgsql.EntityFrameworkCore.PostgreSQL 8.0.11
  - Full PostgreSQL dialect support in ANTLR4 grammar

- **MySQL** database support
  - Connection factory and schema provider
  - MySql.Data 9.2.0
  - MySQL dialect support in ANTLR4 grammar

- **SQL Server** database support
  - Connection factory and schema provider
  - Microsoft.Data.SqlClient 6.0.1
  - SQL Server dialect support in ANTLR4 grammar

- **Oracle** database support
  - Connection factory and schema provider
  - Oracle.ManagedDataAccess.Core 23.7.0
  - Oracle dialect support in ANTLR4 grammar

- **SQLite** database support
  - Connection factory and schema provider
  - Microsoft.Data.Sqlite 9.0.1

- **MariaDB** database support
  - Connection factory and schema provider
  - MySql.Data 9.2.0 (MariaDB compatible)

---

### Phase 5 - Avalonia UI Implementation - 2026-02-XX

#### Added
- Complete Avalonia UI implementation with 13 windows
  - **MainWindow** - Main application window
  - **MakeCrudWindow** - CRUD generation window
  - **AnalyzeQueryWindow** - SQL query analysis window
  - **VersionWindow** - Version information window
  - **SettingsWindow** - Application settings window
  - **CrudSearchWindow** - CRUD search window
  - **FileListWindow** - File list window
  - **FilterWindow** - Filter configuration window
  - **GrepWindow** - Grep search window
  - **GenericListWindow** - Generic list display window
  - **SearchWindow** - Search window
  - **StartupWindow** - Startup splash window
  - **TableDefinitionWindow** - Table definition window

- Total implementation statistics:
  - 76 ObservableProperties
  - 60 RelayCommands
  - Full MVVM pattern implementation with CommunityToolkit.Mvvm

---

### Phase 4 - Authentication Server Core API - 2026-02-XX

#### Added
- **AuthServer** - JWT-based authentication server
  - Entity Framework Core 8.0 + PostgreSQL (Npgsql)
  - 4 entity models: User, LicenseKey, DeviceActivation, AuditLog
  - Repository/service pattern implementation
  - JWT authentication with configurable expiration
  - Swagger/OpenAPI integration (Swashbuckle.AspNetCore 6.6.2)

- **LicenseController** - License authentication endpoints
  - `POST /api/license/authenticate` - License key + Device ID authentication
  - `GET /api/license/validate` - JWT token validation
  - Email-optional simplified authentication
  - Error code field for detailed error identification

- **AdminController** - Admin management API
  - User management endpoints
  - License management (creation, listing, revocation)
  - Device activation management
  - Audit log retrieval with filtering

- **Services**
  - LicenseGenerationService - License key generation
  - AuthenticationService - Authentication logic with device registration
  - AuditLogService - Audit logging

---

### Phase 3 - Common Module Migration - 2026-02-XX

#### Added
- **Core Utilities** (8 classes)
  - StringUtilities - String manipulation utilities
  - DictionaryHelper - Dictionary helper methods
  - RegexUtilities - Regular expression utilities
  - FileSystemHelper - File system operations
  - ExternalEditorLauncher - External editor integration
  - GlobalState - Application global state
  - ProgramIdExtractor - Program ID extraction
  - LogicalNameResolver - Logical name resolution
  - LicenseClient - License client implementation

- **ANTLR4 SQL Parser**
  - Base SQL grammar (Sql.g4)
  - 4 database dialect grammars (PostgreSQL, MySQL, SQL Server, Oracle)
  - SqlAnalyzer - SQL analysis engine
  - SqlVisitor - CRUD extraction visitor implementation

- **QueryFormatter** - SQL query formatting utility

---

### Phase 2 - Data Model Migration - 2026-02-XX

#### Added
- Complete data model migration from VB.NET to C#
- All model classes converted with proper C# conventions
- Case-insensitive dictionary usage (StringComparer.OrdinalIgnoreCase)

---

### Phase 1 - Project Structure Setup - 2026-02-XX

#### Added
- **.NET 8.0** project structure
- **Avalonia UI 11.3.12** cross-platform UI framework
- Solution file (.slnx) with all project references
- Initial project scaffolding
  - CRUDExplorer.Core
  - CRUDExplorer.SqlParser
  - CRUDExplorer.UI
  - CRUDExplorer.AuthServer
  - CRUDExplorer.Core.Tests
  - CRUDExplorer.SqlParser.Tests
  - CRUDExplorer.IntegrationTests
  - CRUDExplorer.UI.Tests

---

## Test Status Summary

### Overall Test Results (as of 2026-02-22)
- **Total Tests**: 329
- **Passing**: 313 (95.1%)
- **Failing**: 16 (4.9%)

### Test Breakdown by Project
- **CRUDExplorer.Core.Tests**: 233/247 passing (94.3%)
  - Includes 100 new CRUD extraction tests
- **CRUDExplorer.SqlParser.Tests**: 51/53 passing (96.2%)
- **CRUDExplorer.IntegrationTests**: 16/16 passing (100%)
- **CRUDExplorer.UI.Tests**: 13/13 passing (100%)

---

## Documentation Summary

### Total Documentation Created
- **8 major documents**
- **4,962+ lines** of comprehensive documentation

### Document List
1. README.md (366 lines) - Japanese project overview
2. README_EN.md - English project overview
3. docs/API.md (260 lines) - API reference
4. CONTRIBUTING.md (650+ lines) - Developer contribution guide
5. docs/DATABASE_SUPPORT.md (494 lines) - Database support matrix
6. docs/ANTLR4_GRAMMAR_SPEC.md (1,056 lines) - ANTLR4 grammar specification
7. docs/DEPLOYMENT.md (794 lines) - Deployment guide
8. docs/PRODUCTION_READINESS.md (671 lines) - Production readiness checklist

---

## Project Completion Status

### Overall Progress: ~92% Complete

### Completed Phases (100%)
- ✅ Phase 1: Project Structure & Architecture (100%)
- ✅ Phase 2: ANTLR4 Grammar Implementation (100%)
- ✅ Phase 3: SQL Parser Implementation (100%)
- ✅ Phase 4: Authentication Server Core API (100%)
- ✅ Phase 5: Avalonia UI - All 13 Windows (100%)
- ✅ Phase 6.1-6.2+6.4: Database Support - 6 Databases (100%)
- ✅ Phase 7.2: Integration Tests (100%)
- ✅ Phase 8.1: User Documentation (100%)
- ✅ Phase 8.3: OpenAPI/Swagger Documentation (100%)

### Partially Completed Phases
- 🟡 Phase 7.1: CRUD Extraction Tests (95.1% - 313/329 tests passing)
- 🟡 Phase 8.2: Developer Documentation (57% - 4/7 items complete)
- 🟡 Phase 9.1: Production Readiness Preparation (20% - 1/5 items complete)

### Pending Phases
- ⏸️ Phase 6.3: Cloud Database Support (Snowflake, BigQuery, Databricks, Redshift)
- ⏸️ Phase 8.4: Installer Creation (Windows MSI, macOS DMG)
- ⏸️ Phase 8.5: Deployment (Docker Hub, Azure App Service)
- ⏸️ Phase 8.6: CI/CD Pipeline (GitHub Actions)
- ⏸️ Phase 8.7: Release Notes
- ⏸️ Phase 9.2-9.3: Additional Security & Performance Optimization

---

## Technology Stack

### Backend
- .NET 8.0
- ASP.NET Core 8.0 (AuthServer)
- Entity Framework Core 8.0
- PostgreSQL (Npgsql 8.0.11)
- JWT Authentication (Microsoft.AspNetCore.Authentication.JwtBearer 8.0.11)
- Swashbuckle.AspNetCore 6.6.2 (Swagger/OpenAPI)

### Frontend
- Avalonia UI 11.3.12 (Cross-platform MVVM)
- CommunityToolkit.Mvvm 8.4.0
- Avalonia.Controls.DataGrid 11.3.12

### Database Support
- PostgreSQL (Npgsql.EntityFrameworkCore.PostgreSQL 8.0.11)
- MySQL (MySql.Data 9.2.0)
- SQL Server (Microsoft.Data.SqlClient 6.0.1)
- Oracle (Oracle.ManagedDataAccess.Core 23.7.0)
- SQLite (Microsoft.Data.Sqlite 9.0.1)
- MariaDB (MySql.Data 9.2.0 compatible)

### SQL Parsing
- ANTLR4.Runtime.Standard 4.13.2
- Custom SQL grammars (Sql.g4 + 4 dialect grammars)

### Testing
- xUnit 2.9.3
- Avalonia.Headless 11.3.12 (UI testing)
- Microsoft.AspNetCore.Mvc.Testing 8.0.11 (API testing)
- Microsoft.EntityFrameworkCore.InMemory 8.0.11 (test database)

---

## License

This project is licensed under the MIT License - see the LICENSE file for details.

---

## Contributors

- Project Migration: VB.NET Windows Forms → C# .NET 8 + Avalonia UI
- Original VB.NET Application: CRUDExplorer (Windows Forms)
- Migration Target: Cross-platform desktop application (Windows/macOS/Linux)

---

## Links

- GitHub Repository: https://github.com/satoshikosugi/CRUDExplorer
- Issue Tracker: https://github.com/satoshikosugi/CRUDExplorer/issues
- Documentation: See docs/ directory
