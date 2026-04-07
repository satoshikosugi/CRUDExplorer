using System.Text.Json;
using CRUDExplorer.Core.Utilities;
using CRUDExplorer.SqlParser.Analyzers;

namespace CRUDExplorer.Core.Tests;

public class LanguageSqlExtractorTests
{
    private static readonly string TestSampleDir = FindTestSampleDir();

    private static string FindTestSampleDir()
    {
        // Walk up from the test binary output directory until we find the TestSample folder
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            var candidate = Path.Combine(dir.FullName, "TestSample");
            if (Directory.Exists(candidate))
                return candidate;
            dir = dir.Parent;
        }
        // Fallback: 5 levels up from bin/Debug/net8.0
        return Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "TestSample"));
    }

    // ====== DetectLanguage Tests ======

    [Theory]
    [InlineData("Foo.java", LanguageKind.Java)]
    [InlineData("Bar.cs", LanguageKind.CSharp)]
    [InlineData("baz.py", LanguageKind.Python)]
    [InlineData("main.go", LanguageKind.Go)]
    [InlineData("index.php", LanguageKind.Php)]
    [InlineData("app.rb", LanguageKind.Ruby)]
    [InlineData("service.ts", LanguageKind.TypeScript)]
    [InlineData("util.js", LanguageKind.JavaScript)]
    [InlineData("mapper.xml", LanguageKind.Xml)]
    [InlineData("query.sql", LanguageKind.Sql)]
    [InlineData("proc.pls", LanguageKind.Sql)]
    [InlineData("proc.plsql", LanguageKind.Sql)]
    [InlineData("README.md", LanguageKind.Unknown)]
    public void DetectLanguage_ReturnsExpectedLanguage(string fileName, LanguageKind expected)
    {
        Assert.Equal(expected, LanguageSqlExtractor.DetectLanguage(fileName));
    }

    // ====== Java Tests ======

    [Fact]
    public void ExtractFromJava_SelectStatement_ExtractsSql()
    {
        var code = @"public User findUser(int id) {
    PreparedStatement ps = conn.prepareStatement(""SELECT id, name FROM users WHERE id = ?"");
}";
        var results = LanguageSqlExtractor.Extract(code, "Sample.java");
        Assert.Contains(results, r => r.Sql.Contains("users", StringComparison.OrdinalIgnoreCase)
            && r.Sql.Contains("SELECT", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ExtractFromJava_InsertStatement_ExtractsSql()
    {
        var code = @"public void createUser() {
    conn.prepareStatement(""INSERT INTO users (name) VALUES (?)"");
}";
        var results = LanguageSqlExtractor.Extract(code, "Sample.java");
        Assert.Contains(results, r => r.Sql.Contains("INSERT", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ExtractFromJava_UpdateStatement_ExtractsSql()
    {
        var code = @"public void updateUser() {
    conn.prepareStatement(""UPDATE users SET email = ? WHERE id = ?"");
}";
        var results = LanguageSqlExtractor.Extract(code, "Sample.java");
        Assert.Contains(results, r => r.Sql.Contains("UPDATE", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ExtractFromJava_DeleteStatement_ExtractsSql()
    {
        var code = @"public void deleteUser() {
    conn.prepareStatement(""DELETE FROM users WHERE id = ?"");
}";
        var results = LanguageSqlExtractor.Extract(code, "Sample.java");
        Assert.Contains(results, r => r.Sql.Contains("DELETE", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ExtractFromJava_NonSqlString_NotExtracted()
    {
        var code = @"String greeting = ""Hello, World!"";
String url = ""http://example.com"";";
        var results = LanguageSqlExtractor.Extract(code, "Sample.java");
        Assert.Empty(results);
    }

    // ====== C# Tests ======

    [Fact]
    public void ExtractFromCSharp_SqlCommandConstructor_ExtractsSql()
    {
        var code = @"var cmd = new SqlCommand(""SELECT id, name FROM users WHERE id = @id"", conn);";
        var results = LanguageSqlExtractor.Extract(code, "Sample.cs");
        Assert.Contains(results, r => r.Sql.Contains("users", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ExtractFromCSharp_CommandTextAssignment_ExtractsSql()
    {
        var code = @"cmd.CommandText = ""INSERT INTO users (name, email) VALUES (@name, @email)"";";
        var results = LanguageSqlExtractor.Extract(code, "Sample.cs");
        Assert.Contains(results, r => r.Sql.Contains("INSERT", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ExtractFromCSharp_VerbatimString_ExtractsSql()
    {
        var code = @"var sql = @""UPDATE users SET email = @email WHERE id = @id"";";
        var results = LanguageSqlExtractor.Extract(code, "Sample.cs");
        Assert.Contains(results, r => r.Sql.Contains("UPDATE", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ExtractFromCSharp_DeleteQuery_ExtractsSql()
    {
        var code = @"await conn.ExecuteAsync(""DELETE FROM order_items WHERE order_id = @OrderId"", new { OrderId = id });";
        var results = LanguageSqlExtractor.Extract(code, "Sample.cs");
        Assert.Contains(results, r => r.Sql.Contains("DELETE", StringComparison.OrdinalIgnoreCase));
    }

    // ====== Python Tests ======

    [Fact]
    public void ExtractFromPython_DoubleQuoteString_ExtractsSql()
    {
        var code = "cursor.execute(\"SELECT id, name FROM users WHERE id = %s\", (user_id,))";
        var results = LanguageSqlExtractor.Extract(code, "sample.py");
        Assert.Contains(results, r => r.Sql.Contains("users", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ExtractFromPython_SingleQuoteString_ExtractsSql()
    {
        var code = "cursor.execute('INSERT INTO users (name, email) VALUES (%s, %s)', (name, email))";
        var results = LanguageSqlExtractor.Extract(code, "sample.py");
        Assert.Contains(results, r => r.Sql.Contains("INSERT", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ExtractFromPython_UpdateQuery_ExtractsSql()
    {
        var code = "cursor.execute('UPDATE users SET email = %s WHERE id = %s', (email, user_id))";
        var results = LanguageSqlExtractor.Extract(code, "sample.py");
        Assert.Contains(results, r => r.Sql.Contains("UPDATE", StringComparison.OrdinalIgnoreCase));
    }

    // ====== Go Tests ======

    [Fact]
    public void ExtractFromGo_BacktickString_ExtractsSql()
    {
        var code = "row := db.QueryContext(ctx, `SELECT id, name FROM users WHERE id = $1`, userID)";
        var results = LanguageSqlExtractor.Extract(code, "sample.go");
        Assert.Contains(results, r => r.Sql.Contains("users", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ExtractFromGo_DoubleQuoteString_ExtractsSql()
    {
        var code = "db.ExecContext(ctx, \"INSERT INTO users (name, email) VALUES ($1, $2)\", name, email)";
        var results = LanguageSqlExtractor.Extract(code, "sample.go");
        Assert.Contains(results, r => r.Sql.Contains("INSERT", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ExtractFromGo_UpdateQuery_ExtractsSql()
    {
        var code = "db.ExecContext(ctx, \"UPDATE users SET email = $1 WHERE id = $2\", email, userID)";
        var results = LanguageSqlExtractor.Extract(code, "sample.go");
        Assert.Contains(results, r => r.Sql.Contains("UPDATE", StringComparison.OrdinalIgnoreCase));
    }

    // ====== PHP Tests ======

    [Fact]
    public void ExtractFromPhp_PdoPrepare_ExtractsSql()
    {
        var code = "$stmt = $this->pdo->prepare(\"SELECT id, name FROM users WHERE id = :id\");";
        var results = LanguageSqlExtractor.Extract(code, "sample.php");
        Assert.Contains(results, r => r.Sql.Contains("users", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ExtractFromPhp_InsertQuery_ExtractsSql()
    {
        var code = "$stmt = $pdo->prepare('INSERT INTO users (name, email) VALUES (:name, :email)');";
        var results = LanguageSqlExtractor.Extract(code, "sample.php");
        Assert.Contains(results, r => r.Sql.Contains("INSERT", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ExtractFromPhp_UpdateQuery_ExtractsSql()
    {
        var code = "$stmt = $pdo->prepare('UPDATE users SET email = :email WHERE id = :id');";
        var results = LanguageSqlExtractor.Extract(code, "sample.php");
        Assert.Contains(results, r => r.Sql.Contains("UPDATE", StringComparison.OrdinalIgnoreCase));
    }

    // ====== Ruby Tests ======

    [Fact]
    public void ExtractFromRuby_FindBySql_ExtractsSql()
    {
        var code = "User.find_by_sql(\"SELECT * FROM users WHERE id = #{id}\")";
        var results = LanguageSqlExtractor.Extract(code, "sample.rb");
        Assert.Contains(results, r => r.Sql.Contains("users", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ExtractFromRuby_ConnectionExecute_ExtractsSql()
    {
        var code = "ActiveRecord::Base.connection.execute(\"INSERT INTO users (name) VALUES ('Alice')\")";
        var results = LanguageSqlExtractor.Extract(code, "sample.rb");
        Assert.Contains(results, r => r.Sql.Contains("INSERT", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ExtractFromRuby_UpdateQuery_ExtractsSql()
    {
        var code = "ActiveRecord::Base.connection.execute(\"UPDATE users SET name = 'Bob' WHERE id = 1\")";
        var results = LanguageSqlExtractor.Extract(code, "sample.rb");
        Assert.Contains(results, r => r.Sql.Contains("UPDATE", StringComparison.OrdinalIgnoreCase));
    }

    // ====== TypeScript Tests ======

    [Fact]
    public void ExtractFromTypeScript_TemplateLiteral_ExtractsSql()
    {
        var code = "const result = await pool.query(`SELECT id, name FROM users WHERE id = $1`, [userId]);";
        var results = LanguageSqlExtractor.Extract(code, "sample.ts");
        Assert.Contains(results, r => r.Sql.Contains("users", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ExtractFromTypeScript_InsertQuery_ExtractsSql()
    {
        var code = "await client.query(`INSERT INTO users (name, email) VALUES ($1, $2)`, [name, email]);";
        var results = LanguageSqlExtractor.Extract(code, "sample.ts");
        Assert.Contains(results, r => r.Sql.Contains("INSERT", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ExtractFromTypeScript_UpdateQuery_ExtractsSql()
    {
        var code = "await client.query(`UPDATE orders SET status = $1 WHERE id = $2`, [status, orderId]);";
        var results = LanguageSqlExtractor.Extract(code, "sample.ts");
        Assert.Contains(results, r => r.Sql.Contains("UPDATE", StringComparison.OrdinalIgnoreCase));
    }

    // ====== XML Tests ======

    [Fact]
    public void ExtractFromXml_SelectTag_ExtractsSql()
    {
        var code = @"<select id=""findUser"" resultType=""User"">SELECT id, name FROM users WHERE id = #{id}</select>";
        var results = LanguageSqlExtractor.Extract(code, "mapper.xml");
        Assert.Contains(results, r => r.Sql.Contains("users", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(results, r => r.ContextName == "findUser");
    }

    [Fact]
    public void ExtractFromXml_InsertTag_ExtractsSql()
    {
        var code = @"<insert id=""insertUser"">INSERT INTO users (name, email) VALUES (#{name}, #{email})</insert>";
        var results = LanguageSqlExtractor.Extract(code, "mapper.xml");
        Assert.Contains(results, r => r.Sql.Contains("INSERT", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ExtractFromXml_UpdateTag_ExtractsSql()
    {
        var code = @"<update id=""updateUser"">UPDATE users SET name = #{name} WHERE id = #{id}</update>";
        var results = LanguageSqlExtractor.Extract(code, "mapper.xml");
        Assert.Contains(results, r => r.Sql.Contains("UPDATE", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ExtractFromXml_DeleteTag_ExtractsSql()
    {
        var code = @"<delete id=""deleteUser"">DELETE FROM users WHERE id = #{id}</delete>";
        var results = LanguageSqlExtractor.Extract(code, "mapper.xml");
        Assert.Contains(results, r => r.Sql.Contains("DELETE", StringComparison.OrdinalIgnoreCase));
    }

    // ====== End-to-End Tests ======

    [Theory]
    [InlineData("java_jdbc_sample.java")]
    [InlineData("java_mybatis_mapper.xml")]
    [InlineData("java_jpa_sample.java")]
    [InlineData("csharp_adonet_sample.cs")]
    [InlineData("csharp_dapper_sample.cs")]
    [InlineData("python_dbapi_sample.py")]
    [InlineData("python_sqlalchemy_sample.py")]
    [InlineData("go_database_sql_sample.go")]
    [InlineData("php_pdo_sample.php")]
    [InlineData("ruby_activerecord_sample.rb")]
    [InlineData("typescript_pg_sample.ts")]
    public void EndToEnd_SampleFile_ExtractsAtLeastOneValidSql(string sampleFileName)
    {
        var filePath = Path.Combine(TestSampleDir, sampleFileName);
        Assert.True(File.Exists(filePath), $"Sample file not found: {filePath}");

        var source = File.ReadAllText(filePath);
        var results = LanguageSqlExtractor.Extract(source, sampleFileName);

        Assert.NotEmpty(results);
        var analyzer = new SqlAnalyzer();
        foreach (var extracted in results)
        {
            var query = analyzer.AnalyzeSql(extracted.Sql, sampleFileName, extracted.LineNo);
            Assert.NotNull(query);
            var allTables = query.TableR.Count + query.TableC.Count + query.TableU.Count + query.TableD.Count;
            Assert.True(allTables > 0, $"Expected tables in: {extracted.Sql}");
        }
    }

    [Theory]
    [InlineData("java_jdbc_sample.java", "java_jdbc_sample.expected.json")]
    [InlineData("csharp_adonet_sample.cs", "csharp_adonet_sample.expected.json")]
    [InlineData("csharp_dapper_sample.cs", "csharp_dapper_sample.expected.json")]
    [InlineData("python_dbapi_sample.py", "python_dbapi_sample.expected.json")]
    [InlineData("go_database_sql_sample.go", "go_database_sql_sample.expected.json")]
    [InlineData("php_pdo_sample.php", "php_pdo_sample.expected.json")]
    [InlineData("typescript_pg_sample.ts", "typescript_pg_sample.expected.json")]
    public void EndToEnd_SampleFile_MatchesExpectedCrud(string sampleFileName, string expectedFileName)
    {
        var filePath = Path.Combine(TestSampleDir, sampleFileName);
        var expectedPath = Path.Combine(TestSampleDir, expectedFileName);

        Assert.True(File.Exists(filePath), $"Sample file not found: {filePath}");
        Assert.True(File.Exists(expectedPath), $"Expected file not found: {expectedPath}");

        var source = File.ReadAllText(filePath);
        var results = LanguageSqlExtractor.Extract(source, sampleFileName);
        Assert.NotEmpty(results);

        var expectedJson = File.ReadAllText(expectedPath);
        var expected = JsonSerializer.Deserialize<ExpectedResultFile>(expectedJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(expected);

        var analyzer = new SqlAnalyzer();

        foreach (var exp in expected.ExpectedResults)
        {
            var match = results.FirstOrDefault(r => ContainsKeywords(r.Sql, exp.Sql));
            Assert.True(match != null, $"Could not find extracted SQL matching: {exp.Sql}");

            var query = analyzer.AnalyzeSql(match!.Sql, sampleFileName, match.LineNo);

            foreach (var (table, ops) in exp.Tables)
            {
                foreach (var op in ops)
                {
                    switch (op)
                    {
                        case "R":
                            Assert.True(query.TableR.ContainsKey(table) || query.GetAllTableR().ContainsKey(table),
                                $"Expected table '{table}' in READ for SQL: {match.Sql}");
                            break;
                        case "C":
                            Assert.True(query.TableC.ContainsKey(table) || query.GetAllTableC().ContainsKey(table),
                                $"Expected table '{table}' in CREATE for SQL: {match.Sql}");
                            break;
                        case "U":
                            Assert.True(query.TableU.ContainsKey(table) || query.GetAllTableU().ContainsKey(table),
                                $"Expected table '{table}' in UPDATE for SQL: {match.Sql}");
                            break;
                        case "D":
                            Assert.True(query.TableD.ContainsKey(table) || query.GetAllTableD().ContainsKey(table),
                                $"Expected table '{table}' in DELETE for SQL: {match.Sql}");
                            break;
                    }
                }
            }
        }
    }

    private static bool ContainsKeywords(string extractedSql, string expectedSql)
    {
        var expUpper = expectedSql.ToUpperInvariant();
        var extUpper = extractedSql.ToUpperInvariant();

        var keywords = new[] { "SELECT", "INSERT", "UPDATE", "DELETE" };
        string? keyword = keywords.FirstOrDefault(k => expUpper.Contains(k));
        if (keyword == null) return false;
        if (!extUpper.Contains(keyword)) return false;

        // Check ALL tables referenced in the expected SQL (FROM, JOIN, INTO, UPDATE)
        var tableMatches = System.Text.RegularExpressions.Regex.Matches(
            expectedSql,
            @"\b(?:FROM|JOIN|INTO|UPDATE)\s+(\w+)",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        foreach (System.Text.RegularExpressions.Match m in tableMatches)
        {
            var tableName = m.Groups[1].Value.ToUpperInvariant();
            if (!extUpper.Contains(tableName)) return false;
        }
        return true;
    }

    private record ExpectedResultFile(
        string SourceFile,
        string Description,
        List<ExpectedResult> ExpectedResults);

    private record ExpectedResult(
        string Sql,
        Dictionary<string, List<string>> Tables,
        string? Context = null);
}
