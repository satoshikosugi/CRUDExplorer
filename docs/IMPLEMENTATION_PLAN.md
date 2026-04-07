# 多言語SQLクエリ抽出 実装計画 / Multi-Language SQL Extraction Implementation Plan

## 概要 / Overview

This feature enables CRUDExplorer to extract raw SQL statements from source files written in multiple programming languages and frameworks. Extracted SQL is then passed to `SqlAnalyzer` to produce CRUD matrices that show which tables are created, read, updated, and deleted by each program unit.

## 対応DBアクセス方式 / Supported DB Access Methods

### Java

| Framework/Method | Pattern | Status |
|---|---|---|
| JDBC PreparedStatement | String literal SQL | ✅ Implemented |
| MyBatis XML Mapper | XML select/insert/update/delete tags | ✅ Implemented |
| JPA @Query (nativeQuery) | @Query annotation value | ✅ Implemented |
| Hibernate HQL | Not SQL - not supported | ⏳ Planned |
| Spring JdbcTemplate | String literal SQL | ✅ Implemented |

### C#

| Framework/Method | Pattern | Status |
|---|---|---|
| ADO.NET SqlCommand | CommandText / constructor | ✅ Implemented |
| Dapper QueryAsync/Execute | Method argument | ✅ Implemented |
| EF Core FromSqlRaw | Method argument | ✅ Implemented |
| EF Core ExecuteSqlRaw | Method argument | ✅ Implemented |

### Python

| Framework/Method | Pattern | Status |
|---|---|---|
| DB-API cursor.execute | Method argument | ✅ Implemented |
| SQLAlchemy text() | text() argument | ✅ Implemented |
| Django raw() | Method argument | ✅ Implemented |

### Go

| Framework/Method | Pattern | Status |
|---|---|---|
| database/sql Query/Exec | String literal | ✅ Implemented |
| sqlx Select/Get | String literal | ✅ Implemented |
| GORM Raw | Method argument | ⏳ Planned |

### PHP

| Framework/Method | Pattern | Status |
|---|---|---|
| PDO prepare/query | Method argument | ✅ Implemented |
| Laravel DB::select | Facade method | ✅ Implemented |

### Ruby

| Framework/Method | Pattern | Status |
|---|---|---|
| ActiveRecord find_by_sql | Method argument | ✅ Implemented |
| connection.execute | Method argument | ✅ Implemented |
| Heredoc SQL | <<~SQL heredoc | ✅ Implemented |

### TypeScript/JavaScript

| Framework/Method | Pattern | Status |
|---|---|---|
| pg pool/client.query | Template literal | ✅ Implemented |
| knex.raw | Method argument | ⏳ Planned |
| Sequelize.query | Method argument | ⏳ Planned |

### XML (MyBatis)

| Tag | Status |
|---|---|
| &lt;select&gt; | ✅ Implemented |
| &lt;insert&gt; | ✅ Implemented |
| &lt;update&gt; | ✅ Implemented |
| &lt;delete&gt; | ✅ Implemented |

## 実装メモ / Implementation Notes

- SQL validation: must contain (SELECT+FROM) OR (INSERT+INTO) OR (UPDATE+SET) OR (DELETE+FROM or WHERE)
- Context tracking: class/function/method names captured for traceability
- Whitespace normalization applied to all extracted SQL
