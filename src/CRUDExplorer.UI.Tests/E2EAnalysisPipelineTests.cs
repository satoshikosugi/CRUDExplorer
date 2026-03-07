using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CRUDExplorer.UI.ViewModels;
using Xunit;
using Xunit.Abstractions;

namespace CRUDExplorer.UI.Tests;

/// <summary>
/// TestSample SQLファイルを使ったE2Eテスト。
/// UIなしでMakeCrudViewModelのパイプラインを実行し、
/// 生成されたTSVファイルとMainWindowViewModelのマトリクス読み込みを検証する。
/// </summary>
public class E2EAnalysisPipelineTests : IDisposable
{
    private readonly ITestOutputHelper _output;
    private readonly string _testSampleDir;
    private readonly string _outputDir;

    // TestSampleで期待されるテーブル名
    private static readonly string[] ExpectedTables =
    [
        "ORDERS", "ORDER_ITEMS", "PRODUCTS", "CUSTOMERS", "INVENTORY"
    ];

    // TestSampleで期待されるプログラム（ファイル名ベース）
    private static readonly string[] ExpectedPrograms =
    [
        "SP_ORDER.sql", "SP_CUSTOMER.sql", "SP_INVENTORY.sql", "SP_REPORT.sql"
    ];

    // 期待されるCRUD組み合わせ（テーブル, プログラム, CRUD種別）
    private static readonly (string Table, string Program, string CrudContains)[] ExpectedCrud =
    [
        ("ORDERS",      "SP_ORDER.sql",     "C"),   // INSERT INTO ORDERS
        ("ORDERS",      "SP_ORDER.sql",     "R"),   // SELECT FROM ORDERS
        ("ORDERS",      "SP_ORDER.sql",     "U"),   // UPDATE ORDERS
        ("ORDER_ITEMS", "SP_ORDER.sql",     "C"),   // INSERT INTO ORDER_ITEMS
        ("ORDER_ITEMS", "SP_ORDER.sql",     "R"),   // SELECT FROM ORDER_ITEMS
        ("ORDER_ITEMS", "SP_ORDER.sql",     "D"),   // DELETE FROM ORDER_ITEMS
        ("PRODUCTS",    "SP_ORDER.sql",     "R"),   // SELECT FROM PRODUCTS
        ("CUSTOMERS",   "SP_CUSTOMER.sql",  "C"),   // INSERT INTO CUSTOMERS
        ("CUSTOMERS",   "SP_CUSTOMER.sql",  "R"),   // SELECT FROM CUSTOMERS
        ("CUSTOMERS",   "SP_CUSTOMER.sql",  "U"),   // UPDATE CUSTOMERS
        ("ORDERS",      "SP_CUSTOMER.sql",  "R"),   // SELECT FROM ORDERS
        ("INVENTORY",   "SP_INVENTORY.sql", "R"),   // SELECT FROM INVENTORY
        ("INVENTORY",   "SP_INVENTORY.sql", "U"),   // UPDATE INVENTORY
        ("PRODUCTS",    "SP_INVENTORY.sql", "R"),   // SELECT FROM PRODUCTS
        ("ORDERS",      "SP_REPORT.sql",    "R"),   // SELECT FROM ORDERS
        ("CUSTOMERS",   "SP_REPORT.sql",    "R"),   // SELECT FROM CUSTOMERS
    ];

    public E2EAnalysisPipelineTests(ITestOutputHelper output)
    {
        _output = output;

        // TestSample ディレクトリをアセンブリ位置から検索
        // bin/Debug/net8.0/ → CRUDExplorer.UI.Tests/ → src/ → CRUDExplorer/ → TestSample/
        var dir = AppContext.BaseDirectory;
        string? found = null;
        for (int i = 0; i < 8; i++)
        {
            var candidate = Path.Combine(dir, "TestSample");
            if (Directory.Exists(candidate))
            {
                found = candidate;
                break;
            }
            var parent = Directory.GetParent(dir)?.FullName;
            if (parent == null) break;
            dir = parent;
        }

        _testSampleDir = found ?? throw new DirectoryNotFoundException(
            "TestSampleディレクトリが見つかりません。プロジェクトルートに TestSample/ があることを確認してください。");

        // テストごとに一意の出力ディレクトリを作成
        _outputDir = Path.Combine(Path.GetTempPath(), $"CRUDExplorer_E2E_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_outputDir);

        _output.WriteLine($"TestSample: {_testSampleDir}");
        _output.WriteLine($"出力先: {_outputDir}");
    }

    public void Dispose()
    {
        // テスト後にテンポラリ出力ディレクトリを削除
        try { Directory.Delete(_outputDir, recursive: true); } catch { }
    }

    // ─────────────────────────────────────────────────────────────────
    // Step 2: クエリ抽出テスト
    // ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Step2_ExtractQuery_GeneratesQueryFiles()
    {
        // Arrange
        var vm = CreateViewModel();
        vm.ProcessAll    = false;
        vm.Step0_SourceCopy      = true;
        vm.Step1_ExtractDynamicSql = false;
        vm.Step2_ExtractQuery    = true;
        vm.Step3_AnalyzeCrud     = false;
        vm.Step4_GenerateMatrix  = false;

        // Act
        await vm.ExecuteAnalysisCommand.ExecuteAsync(null);
        LogAnalysisLog(vm);

        // Assert: querys/*.query ファイルが生成されている
        var querysDir = Path.Combine(_outputDir, "querys");
        Assert.True(Directory.Exists(querysDir), $"querysディレクトリが存在しません: {querysDir}");

        var queryFiles = Directory.GetFiles(querysDir, "*.query");
        _output.WriteLine($"生成された .query ファイル数: {queryFiles.Length}");
        foreach (var f in queryFiles)
        {
            var lines = File.ReadAllLines(f);
            _output.WriteLine($"  {Path.GetFileName(f)}: {lines.Length} クエリ");
        }

        Assert.NotEmpty(queryFiles);

        // 各SQLファイルに対応する .query ファイルが存在すること
        foreach (var sqlFile in ExpectedPrograms)
        {
            var queryFile = Path.Combine(querysDir, sqlFile + ".query");
            Assert.True(File.Exists(queryFile),
                $"{sqlFile}.query が生成されていません");
        }
    }

    [Fact]
    public async Task Step2_ExtractQuery_QueryFilesContainSqlStatements()
    {
        // Arrange
        var vm = CreateViewModel();
        vm.ProcessAll    = false;
        vm.Step0_SourceCopy      = true;
        vm.Step1_ExtractDynamicSql = false;
        vm.Step2_ExtractQuery    = true;
        vm.Step3_AnalyzeCrud     = false;
        vm.Step4_GenerateMatrix  = false;

        // Act
        await vm.ExecuteAnalysisCommand.ExecuteAsync(null);

        // Assert: 各.queryファイルにlineNo\tsql\tfuncName 形式の行が含まれる
        var querysDir = Path.Combine(_outputDir, "querys");
        foreach (var qf in Directory.GetFiles(querysDir, "*.query"))
        {
            var lines = File.ReadAllLines(qf);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split('\t');
                // 最低でも lineNo と SQL の2カラム
                Assert.True(parts.Length >= 2,
                    $"{Path.GetFileName(qf)}: 不正なクエリ行形式: {line}");
                // lineNo は数値
                Assert.True(int.TryParse(parts[0], out _),
                    $"{Path.GetFileName(qf)}: 行番号が数値でない: {parts[0]}");
                // SQL は SELECT/INSERT/UPDATE/DELETE/WITH/TRUNCATE で始まる
                var sql = parts[1].TrimStart().ToUpperInvariant();
                var validStart = new[] { "SELECT", "INSERT", "UPDATE", "DELETE", "TRUNCATE", "WITH" };
                Assert.True(validStart.Any(sql.StartsWith),
                    $"{Path.GetFileName(qf)}: SQLが期待キーワードで始まらない: {parts[1][..Math.Min(50, parts[1].Length)]}");
            }
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // Step 3: CRUD解析テスト
    // ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Step3_AnalyzeCrud_GeneratesCrudTsv()
    {
        // Arrange
        var vm = CreateViewModel();

        // Act: 全ステップ実行
        await vm.ExecuteAnalysisCommand.ExecuteAsync(null);
        LogAnalysisLog(vm);

        // Assert: CRUD.tsv と CRUDColumns.tsv が生成されている
        var querysDir = Path.Combine(_outputDir, "querys");
        var crudTsv = Path.Combine(querysDir, "CRUD.tsv");
        var colTsv  = Path.Combine(querysDir, "CRUDColumns.tsv");

        Assert.True(File.Exists(crudTsv),   $"CRUD.tsv が存在しません: {crudTsv}");
        Assert.True(File.Exists(colTsv),    $"CRUDColumns.tsv が存在しません: {colTsv}");

        var crudLines = File.ReadAllLines(crudTsv).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
        _output.WriteLine($"CRUD.tsv 行数: {crudLines.Length}");
        Assert.NotEmpty(crudLines);
    }

    [Fact]
    public async Task Step3_AnalyzeCrud_CrudTsvContainsExpectedEntries()
    {
        // Arrange
        var vm = CreateViewModel();

        // Act
        await vm.ExecuteAnalysisCommand.ExecuteAsync(null);

        var crudTsv   = Path.Combine(_outputDir, "querys", "CRUD.tsv");
        var crudLines = File.ReadAllLines(crudTsv)
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l => l.Split('\t'))
            .Where(p => p.Length >= 5)
            .ToList();

        _output.WriteLine("=== CRUD.tsv の内容 ===");
        foreach (var p in crudLines)
            _output.WriteLine($"  ファイル={p[0]} テーブル={p[3]} CRUD={p[4]}");

        // Assert: 期待するCRUD組み合わせが全て存在する
        foreach (var (table, program, crudContains) in ExpectedCrud)
        {
            bool found = crudLines.Any(p =>
                p[0].Equals(program, StringComparison.OrdinalIgnoreCase) &&
                p[3].Equals(table,   StringComparison.OrdinalIgnoreCase) &&
                p[4].Contains(crudContains, StringComparison.OrdinalIgnoreCase));

            Assert.True(found,
                $"期待するCRUDエントリが見つかりません: テーブル={table}, プログラム={program}, CRUD含む={crudContains}");
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // Step 4: CRUDマトリクス生成テスト
    // ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Step4_GenerateMatrix_GeneratesCrudMatrixTsv()
    {
        // Arrange
        var vm = CreateViewModel();

        // Act
        await vm.ExecuteAnalysisCommand.ExecuteAsync(null);
        LogAnalysisLog(vm);

        // Assert: CRUDMatrix.tsv が存在する
        var matrixTsv = Path.Combine(_outputDir, "querys", "CRUDMatrix.tsv");
        Assert.True(File.Exists(matrixTsv), $"CRUDMatrix.tsv が存在しません: {matrixTsv}");

        var lines = File.ReadAllLines(matrixTsv).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
        _output.WriteLine("=== CRUDMatrix.tsv の内容 ===");
        foreach (var l in lines) _output.WriteLine($"  {l}");

        // ヘッダ行 + 集計行 + 最低1データ行
        Assert.True(lines.Length >= 3, $"CRUDMatrix.tsv の行数が不足: {lines.Length}");
    }

    [Fact]
    public async Task Step4_GenerateMatrix_HeaderContainsAllPrograms()
    {
        // Arrange
        var vm = CreateViewModel();

        // Act
        await vm.ExecuteAnalysisCommand.ExecuteAsync(null);

        var matrixTsv = Path.Combine(_outputDir, "querys", "CRUDMatrix.tsv");
        var lines = File.ReadAllLines(matrixTsv);
        var header = lines[0].Split('\t');

        _output.WriteLine($"ヘッダ列: {string.Join(", ", header)}");

        // ヘッダに全プログラムが含まれる
        foreach (var prog in ExpectedPrograms)
        {
            Assert.True(header.Any(h => h.Equals(prog, StringComparison.OrdinalIgnoreCase)),
                $"CRUDMatrix.tsv ヘッダにプログラムが含まれていません: {prog}");
        }
    }

    [Fact]
    public async Task Step4_GenerateMatrix_MatrixContainsAllExpectedTables()
    {
        // Arrange
        var vm = CreateViewModel();

        // Act
        await vm.ExecuteAnalysisCommand.ExecuteAsync(null);

        var matrixTsv = Path.Combine(_outputDir, "querys", "CRUDMatrix.tsv");
        var lines = File.ReadAllLines(matrixTsv)
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Skip(2)    // ヘッダ行・集計行をスキップ
            .Select(l => l.Split('\t')[0].Trim().ToUpperInvariant())
            .ToList();

        _output.WriteLine($"マトリクス内テーブル: {string.Join(", ", lines)}");

        // 全期待テーブルがマトリクスに存在する
        foreach (var table in ExpectedTables)
        {
            Assert.True(lines.Contains(table, StringComparer.OrdinalIgnoreCase),
                $"CRUDMatrix.tsv にテーブルが含まれていません: {table}");
        }
    }

    [Fact]
    public async Task Step4_GenerateMatrix_CrudValuesAreCorrect()
    {
        // Arrange
        var vm = CreateViewModel();

        // Act
        await vm.ExecuteAnalysisCommand.ExecuteAsync(null);

        var matrixTsv = Path.Combine(_outputDir, "querys", "CRUDMatrix.tsv");
        var allLines = File.ReadAllLines(matrixTsv)
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToArray();

        var header = allLines[0].Split('\t');
        // col0=テーブル名, col1=論理名, col2=合計, col3+=プログラム列
        var progColumns = header.Skip(3).ToArray();

        // データ行をパース
        var matrixData = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var line in allLines.Skip(2))
        {
            var cols = line.Split('\t');
            if (cols.Length < 3) continue;
            var tableName = cols[0].Trim();
            if (string.IsNullOrEmpty(tableName)) continue;

            var progValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < progColumns.Length; i++)
            {
                var val = (i + 3 < cols.Length) ? cols[i + 3].Trim() : "";
                progValues[progColumns[i]] = val;
            }
            matrixData[tableName] = progValues;
        }

        _output.WriteLine("=== マトリクス内容 ===");
        foreach (var kv in matrixData)
        {
            _output.WriteLine($"  [{kv.Key}]");
            foreach (var pv in kv.Value.Where(x => !string.IsNullOrEmpty(x.Value)))
                _output.WriteLine($"    {pv.Key}: {pv.Value}");
        }

        // 期待するCRUD組み合わせをマトリクスで検証
        foreach (var (table, program, crudContains) in ExpectedCrud)
        {
            Assert.True(matrixData.ContainsKey(table),
                $"マトリクスにテーブルが存在しません: {table}");

            Assert.True(matrixData[table].ContainsKey(program),
                $"マトリクスにプログラム列が存在しません: {program}");

            var cellValue = matrixData[table][program];
            Assert.True(cellValue.Contains(crudContains, StringComparison.OrdinalIgnoreCase),
                $"マトリクスセルに期待CRUD値がありません: [{table}][{program}]={cellValue}, 期待含む={crudContains}");
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // MainWindowViewModel マトリクス読み込みテスト
    // ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task MainWindowViewModel_LoadMatrix_ParsesGeneratedTsv()
    {
        // Arrange: 先にパイプラインを実行してTSVを生成
        var makeCrudVm = CreateViewModel();
        await makeCrudVm.ExecuteAnalysisCommand.ExecuteAsync(null);
        LogAnalysisLog(makeCrudVm);

        var matrixTsv = Path.Combine(_outputDir, "querys", "CRUDMatrix.tsv");
        Assert.True(File.Exists(matrixTsv), "前提条件: CRUDMatrix.tsv が存在すること");

        // Act: MainWindowViewModelでTSVを読み込む（内部メソッドをリフレクションで呼ぶ代わりに
        //       パブリックプロパティを通じて結果を検証）
        var mainVm = new MainWindowViewModel(new Tests.Stubs.NullWindowService());
        mainVm.SourcePath = _outputDir;

        // LoadCrudDataAsync は private なのでリフレクションで呼び出す
        var method = typeof(MainWindowViewModel).GetMethod(
            "LoadCrudDataAsync",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(method);
        var task = (Task)method.Invoke(mainVm, null)!;
        await task;

        _output.WriteLine($"MatrixRows 件数: {mainVm.MatrixRows.Count}");
        _output.WriteLine($"MatrixHeaders 件数: {mainVm.MatrixHeaders.Length}");
        _output.WriteLine($"CrudListData 件数（初期状態=未選択）: {mainVm.CrudListData.Count}");
        foreach (var row in mainVm.MatrixRows)
            _output.WriteLine($"  [{row.TableName}] 合計={row.Total} Values={row.Values.Count}");

        // Assert
        Assert.True(mainVm.MatrixRows.Count > 0, "MatrixRows が空です");
        Assert.True(mainVm.MatrixHeaders.Length > 0, "MatrixHeaders が空です");

        // CrudListData は初期状態（セル未選択）では空。セル選択でフィルタされる仕様。
        // テーブル名で絞り込んで全件取得できることを検証
        mainVm.FilterCrudListBySelection("ORDERS", null);
        _output.WriteLine($"CrudListData 件数（ORDERS選択後）: {mainVm.CrudListData.Count}");
        Assert.True(mainVm.CrudListData.Count > 0, "FilterCrudListBySelection(ORDERS, null) で CrudListData が空です");

        // テーブル＋プログラムで絞り込み
        mainVm.FilterCrudListBySelection("ORDERS", "SP_ORDER.sql");
        _output.WriteLine($"CrudListData 件数（ORDERS+SP_ORDER.sql選択後）: {mainVm.CrudListData.Count}");
        Assert.True(mainVm.CrudListData.Count > 0, "FilterCrudListBySelection(ORDERS, SP_ORDER.sql) で CrudListData が空です");

        // 期待テーブルが全行に含まれる
        var rowTableNames = mainVm.MatrixRows.Select(r => r.TableName.ToUpperInvariant()).ToList();
        foreach (var table in ExpectedTables)
        {
            Assert.True(rowTableNames.Contains(table, StringComparer.OrdinalIgnoreCase),
                $"MatrixRows にテーブルが含まれていません: {table}");
        }

        // 期待プログラムがヘッダに含まれる
        foreach (var prog in ExpectedPrograms)
        {
            Assert.True(mainVm.MatrixHeaders.Contains(prog, StringComparer.OrdinalIgnoreCase),
                $"MatrixHeaders にプログラムが含まれていません: {prog}");
        }
    }

    [Fact]
    public async Task MainWindowViewModel_LoadMatrix_IndexerReturnsCorrectCrudValues()
    {
        // Arrange: パイプライン実行
        var makeCrudVm = CreateViewModel();
        await makeCrudVm.ExecuteAnalysisCommand.ExecuteAsync(null);

        // Act: MainWindowViewModel読み込み
        var mainVm = new MainWindowViewModel(new Tests.Stubs.NullWindowService());
        mainVm.SourcePath = _outputDir;
        var method = typeof(MainWindowViewModel).GetMethod(
            "LoadCrudDataAsync",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        await (Task)method!.Invoke(mainVm, null)!;

        // Assert: インデクサー経由でCRUD値が取得できる（DataGridバインディングと同じ経路）
        foreach (var (table, program, crudContains) in ExpectedCrud)
        {
            var row = mainVm.MatrixRows.FirstOrDefault(r =>
                r.TableName.Equals(table, StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(row);

            // インデクサーを使って値を取得（DataGridの [program] バインディングと同一）
            var cellValue = row[program];
            _output.WriteLine($"  [{table}][{program}] = '{cellValue}'");

            Assert.True(cellValue.Contains(crudContains, StringComparison.OrdinalIgnoreCase),
                $"インデクサー経由のセル値が不正: [{table}][{program}]='{cellValue}', 期待含む={crudContains}");
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // ヘルパー
    // ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// TestSample → _outputDir のパイプラインビューモデルを生成する。
    /// </summary>
    private MakeCrudViewModel CreateViewModel()
    {
        var vm = new MakeCrudViewModel();
        vm.SourcePath = _testSampleDir;
        vm.DestPath   = _outputDir;
        vm.ProcessAll = true;
        return vm;
    }

    private void LogAnalysisLog(MakeCrudViewModel vm)
    {
        _output.WriteLine("=== 解析ログ ===");
        foreach (var line in vm.AnalysisLog.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            _output.WriteLine(line);
    }
}
