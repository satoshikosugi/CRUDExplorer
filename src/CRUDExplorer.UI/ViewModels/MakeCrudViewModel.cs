using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRUDExplorer.Core.Models;
using CRUDExplorer.Core.Utilities;
using CRUDExplorer.SqlParser.Analyzers;

namespace CRUDExplorer.UI.ViewModels;

public partial class MakeCrudViewModel : ViewModelBase
{
    private readonly Func<string, Task<string?>> _folderPicker;
    private readonly Action _closeWindow;

    [ObservableProperty]
    private string _sourcePath = string.Empty;

    [ObservableProperty]
    private string _destPath = string.Empty;

    [ObservableProperty]
    private bool _processAll = true;

    [ObservableProperty]
    private bool _step0_SourceCopy = true;

    [ObservableProperty]
    private bool _step1_ExtractDynamicSql = true;

    [ObservableProperty]
    private bool _step2_ExtractQuery = true;

    [ObservableProperty]
    private bool _step3_AnalyzeCrud = true;

    [ObservableProperty]
    private bool _step4_GenerateMatrix = true;

    [ObservableProperty]
    private bool _reflectViewIndirect = true;

    [ObservableProperty]
    private bool _checkReferenceCondition = false;

    [ObservableProperty]
    private string _analysisLog = string.Empty;

    [ObservableProperty]
    private bool _isAnalyzing = false;

    /// <summary>
    /// コンストラクタ。ウィンドウ依存の操作をコールバックで受け取る。
    /// </summary>
    /// <param name="folderPicker">フォルダ選択ダイアログを表示する非同期関数（タイトル→パス）</param>
    /// <param name="closeWindow">ウィンドウを閉じるアクション</param>
    public MakeCrudViewModel(
        Func<string, Task<string?>>? folderPicker = null,
        Action? closeWindow = null)
    {
        _folderPicker = folderPicker ?? (_ => Task.FromResult<string?>(null));
        _closeWindow  = closeWindow  ?? (() => { });
    }

    [RelayCommand]
    private async Task SelectSourceFolder()
    {
        var path = await _folderPicker("ソースフォルダを選択");
        if (path != null) SourcePath = path;
    }

    [RelayCommand]
    private async Task SelectDestFolder()
    {
        var path = await _folderPicker("出力先（解析結果）フォルダを選択");
        if (path != null) DestPath = path;
    }

    [RelayCommand]
    private void RunTextEditor()
    {
        if (string.IsNullOrEmpty(DestPath))
        {
            AppendLog("[エラー] 出力先フォルダが選択されていません。");
            return;
        }

        var querysDir = Path.Combine(DestPath, "querys");
        if (!Directory.Exists(querysDir))
        {
            AppendLog("[エラー] querysフォルダが見つかりません。先にCRUD解析を実行してください。");
            return;
        }

        var settings = Settings.Load();
        var launcher = new ExternalEditorLauncher(settings);

        // querysフォルダ内の最初のクエリファイルを開く
        var queryFiles = Directory.GetFiles(querysDir, "*.query");
        if (queryFiles.Length > 0)
        {
            launcher.RunTextEditor(queryFiles[0], 1, string.Empty);
            AppendLog($"エディタを起動しました: {queryFiles[0]}");
        }
        else
        {
            AppendLog("[エラー] クエリファイルが見つかりません。");
        }
    }

    [RelayCommand]
    private void AnalyzeQuery()
    {
        if (string.IsNullOrEmpty(DestPath))
        {
            AppendLog("[エラー] 出力先フォルダが選択されていません。");
            return;
        }

        // GlobalStateに解析結果パスを設定
        // NOTE: このウィンドウからは直接クエリ解析ウィンドウを開けないため、
        // メインウィンドウの「クエリ解析ウィンドウ」メニューから起動してもらう
        GlobalState.Instance.LastAnalysisDestPath = DestPath;
        AppendLog($"解析結果パス: {DestPath}");
        AppendLog("クエリ解析ウィンドウはメインメニュー「クエリ解析ウィンドウ」から起動してください。");
    }

    [RelayCommand]
    private async Task ExecuteAnalysis()
    {
        if (string.IsNullOrEmpty(SourcePath))
        {
            AppendLog("[エラー] ソースフォルダが選択されていません。");
            return;
        }
        if (string.IsNullOrEmpty(DestPath))
        {
            AppendLog("[エラー] 出力先フォルダが選択されていません。");
            return;
        }
        if (SourcePath.Equals(DestPath, StringComparison.OrdinalIgnoreCase))
        {
            AppendLog("[エラー] ソースフォルダと出力先フォルダに同じパスは指定できません。");
            return;
        }
        if (!Directory.Exists(SourcePath))
        {
            AppendLog($"[エラー] ソースフォルダが存在しません: {SourcePath}");
            return;
        }

        IsAnalyzing = true;
        AnalysisLog = string.Empty;
        AppendLog("=== CRUD解析を開始します ===");
        AppendLog($"ソースフォルダ: {SourcePath}");
        AppendLog($"出力先フォルダ: {DestPath}");

        try
        {
            // 出力フォルダ作成
            Directory.CreateDirectory(DestPath);
            var querysDir = Path.Combine(DestPath, "querys");
            Directory.CreateDirectory(querysDir);

            await Task.Run(() =>
            {
                // Step 0: ソースコピー
                if (ProcessAll || Step0_SourceCopy)
                {
                    AppendLog("[Step 0] ソースファイルをコピー中...");
                    CopySourceFiles(SourcePath, DestPath);
                    AppendLog("[Step 0] 完了");
                }

                // Step 1: 動的SQL抽出（簡易：ソースファイルと同じ内容をそのまま使用）
                if (ProcessAll || Step1_ExtractDynamicSql)
                {
                    AppendLog("[Step 1] 動的SQL抽出（静的SQL解析）...");
                    // 現実装ではStep2に直接渡すため特別処理なし
                    AppendLog("[Step 1] 完了");
                }

                // Step 2: クエリ抽出 → querys/*.query ファイル生成
                if (ProcessAll || Step2_ExtractQuery)
                {
                    AppendLog("[Step 2] クエリ抽出を実行中...");
                    var querysDir = Path.Combine(DestPath, "querys");
                    Directory.CreateDirectory(querysDir);
                    // 既存queryファイルを削除
                    foreach (var f in Directory.GetFiles(querysDir, "*.query"))
                        File.Delete(f);

                    var srcDir = Directory.Exists(Path.Combine(DestPath)) ? DestPath : SourcePath;
                    var files = Directory.GetFiles(srcDir);
                    int cnt = 0;
                    foreach (var file in files)
                    {
                        var ext = Path.GetExtension(file).ToLowerInvariant();
                        if (ext == ".tsv" || ext == ".query") continue;
                        cnt++;
                        AppendLog($"  [{cnt}/{files.Length}] {Path.GetFileName(file)}");
                        try
                        {
                            var src = File.ReadAllText(file, Encoding.UTF8);
                            var queries = ExtractSqlQueries(src, Path.GetFileName(file));
                            if (queries.Count > 0)
                            {
                                var outPath = Path.Combine(querysDir, Path.GetFileName(file) + ".query");
                                File.WriteAllLines(outPath, queries, Encoding.UTF8);
                            }
                        }
                        catch (Exception ex2)
                        {
                            AppendLog($"    [警告] {ex2.Message}");
                        }
                    }
                    AppendLog("[Step 2] 完了");
                }

                // Step 3: CRUD解析 → CRUD.tsv / CRUDColumns.tsv 生成
                if (ProcessAll || Step3_AnalyzeCrud)
                {
                    AppendLog("[Step 3] CRUD解析を実行中...");
                    var querysDir = Path.Combine(DestPath, "querys");
                    var crudTsv    = Path.Combine(querysDir, "CRUD.tsv");
                    var colTsv     = Path.Combine(querysDir, "CRUDColumns.tsv");
                    if (File.Exists(crudTsv)) File.Delete(crudTsv);
                    if (File.Exists(colTsv))  File.Delete(colTsv);

                    var analyzer = new SqlAnalyzer();
                    var settings = GlobalState.Instance.AppSettings;
                    var sb    = new StringBuilder();
                    var sbCol = new StringBuilder();

                    var queryFiles = Directory.GetFiles(querysDir, "*.query");
                    int cnt = 0;
                    foreach (var qf in queryFiles)
                    {
                        cnt++;
                        var moduleName = Path.GetFileName(qf);
                        moduleName = moduleName.Substring(0, moduleName.Length - 6); // strip ".query"
                        var programId = ProgramIdExtractor.GetProgramId(moduleName, settings.ProgramIdPattern);
                        if (string.IsNullOrEmpty(programId)) programId = moduleName;

                        AppendLog($"  [{cnt}/{queryFiles.Length}] {moduleName}");

                        foreach (var line in File.ReadAllLines(qf, Encoding.UTF8))
                        {
                            if (string.IsNullOrWhiteSpace(line)) continue;
                            var parts = line.Split('\t');
                            var lineNo    = parts.Length > 0 ? parts[0] : "0";
                            var sql       = parts.Length > 1 ? parts[1] : "";
                            var funcName  = parts.Length > 2 ? parts[2] : "";

                            if (string.IsNullOrWhiteSpace(sql)) continue;

                            var query = analyzer.AnalyzeSql(sql, moduleName, int.TryParse(lineNo, out var ln) ? ln : 0);

                            // テーブルCRUD出力
                            WriteTableCrud(sb, query.GetAllTableC(), moduleName, programId, lineNo, "C", funcName);
                            WriteTableCrud(sb, query.GetAllTableR(), moduleName, programId, lineNo, "R", funcName);
                            WriteTableCrud(sb, query.GetAllTableU(), moduleName, programId, lineNo, "U", funcName);
                            WriteTableCrud(sb, query.GetAllTableD(), moduleName, programId, lineNo, "D", funcName);

                            // カラムCRUD出力
                            WriteColumnCrud(sbCol, query.ColumnInsert, moduleName, programId, lineNo, "C", funcName);
                            WriteColumnCrud(sbCol, query.ColumnSelect, moduleName, programId, lineNo, "R", funcName);
                            WriteColumnCrud(sbCol, query.ColumnUpdate, moduleName, programId, lineNo, "U", funcName);
                            WriteColumnCrud(sbCol, query.ColumnDelete, moduleName, programId, lineNo, "D", funcName);
                        }
                    }

                    File.WriteAllText(crudTsv, sb.ToString(), Encoding.UTF8);
                    File.WriteAllText(colTsv,  sbCol.ToString(), Encoding.UTF8);
                    AppendLog("[Step 3] 完了");
                }

                // Step 4: CRUDMatrix.tsv / CRUDColumnsMatrix.tsv 生成
                if (ProcessAll || Step4_GenerateMatrix)
                {
                    AppendLog("[Step 4] CRUDマトリクス生成を実行中...");
                    var querysDir = Path.Combine(DestPath, "querys");

                    var crudTsv   = Path.Combine(querysDir, "CRUD.tsv");
                    var matrixTsv = Path.Combine(querysDir, "CRUDMatrix.tsv");
                    if (File.Exists(crudTsv))
                        GenerateMatrix(crudTsv, matrixTsv, colIndex: 3, progIndex: 1, crudIndex: 4);

                    var colTsv       = Path.Combine(querysDir, "CRUDColumns.tsv");
                    var colMatrixTsv = Path.Combine(querysDir, "CRUDColumnsMatrix.tsv");
                    if (File.Exists(colTsv))
                        GenerateMatrix(colTsv, colMatrixTsv, colIndex: 3, progIndex: 1, crudIndex: 4);

                    AppendLog("[Step 4] 完了");
                }
            });

            AppendLog("=== CRUD解析が完了しました ===");
            AppendLog($"結果フォルダ: {DestPath}");
            // MainWindowの自動リロード用に結果パスを保存
            GlobalState.Instance.LastAnalysisDestPath = DestPath;
        }
        catch (Exception ex)
        {
            AppendLog($"[エラー] {ex.Message}");
        }
        finally
        {
            IsAnalyzing = false;
        }
    }

    [RelayCommand]
    private void Close()
    {
        _closeWindow();
    }

    private void CopySourceFiles(string srcDir, string destDir)
    {
        foreach (var file in Directory.GetFiles(srcDir))
        {
            var destFile = Path.Combine(destDir, Path.GetFileName(file));
            File.Copy(file, destFile, overwrite: true);
        }
        AppendLog($"  {Directory.GetFiles(srcDir).Length} ファイルをコピーしました");
    }

    private void AppendLog(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        AnalysisLog += $"[{timestamp}] {message}\n";
    }

    // ─── Step 2: SQLクエリ抽出 ────────────────────────────────────────

    /// <summary>
    /// ソースファイルからSQLクエリ行を抽出する。
    /// 戻り値: "lineNo\tsql\tfuncName" の行リスト
    /// </summary>
    private static List<string> ExtractSqlQueries(string src, string fileName)
    {
        var result = new List<string>();

        // コメント削除
        src = RemoveComments(src);

        var lines = src.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
        var funcName = "";

        // PROCEDURE/FUNCTION/CURSORを追跡
        var funcPattern = new Regex(@"\b(PROCEDURE|FUNCTION|CURSOR)\s+(\w+)", RegexOptions.IgnoreCase);

        // SELECT/INSERT/UPDATE/DELETE/TRUNCATE/WITH の開始を検出
        // 行末の SELECT（OPEN v_cursor FOR\n SELECT 等）にもマッチするよう $ を含める
        var sqlStartPattern = new Regex(
            @"(?:^|\s)(SELECT|INSERT|UPDATE|DELETE|TRUNCATE|WITH)(\s|$)",
            RegexOptions.IgnoreCase);

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            // 関数/プロシージャ名を追跡
            var fm = funcPattern.Match(line);
            if (fm.Success)
                funcName = fm.Groups[2].Value;

            var m = sqlStartPattern.Match(line);
            if (!m.Success) continue;

            var keyword = m.Groups[1].Value.ToUpperInvariant();
            var startLineNo = i + 1;

            // SQL文を複数行にわたって収集（セミコロンまたは / で終端）
            var sb = new StringBuilder();
            int j = i;
            bool found = false;
            while (j < lines.Length)
            {
                var cur = lines[j];
                // コメント除去済みなので ; か行末の / を終端とする
                var semiIdx = cur.IndexOf(';');
                var slashIdx = -1;
                var trimmed = cur.TrimEnd();
                if (trimmed == "/") slashIdx = j;

                if (semiIdx >= 0)
                {
                    sb.Append(' ').Append(cur[..semiIdx]);
                    found = true;
                    i = j; // 外側ループをここまで進める
                    break;
                }
                else if (slashIdx >= 0)
                {
                    found = true;
                    i = j;
                    break;
                }
                sb.Append(' ').Append(cur);
                j++;
            }

            if (!found) i = j;

            var sql = Regex.Replace(sb.ToString().Trim(), @"\s+", " ");
            if (string.IsNullOrWhiteSpace(sql)) continue;

            // 基本的な妥当性チェック
            var upper = sql.ToUpperInvariant();
            bool valid = keyword switch
            {
                "SELECT"   => upper.Contains(" FROM "),
                "INSERT"   => upper.Contains(" INTO "),
                "UPDATE"   => upper.Contains(" SET "),
                "DELETE"   => upper.Contains(" FROM ") || upper.Contains(" WHERE "),
                "TRUNCATE" => upper.Contains(" TABLE "),
                "WITH"     => upper.Contains(" SELECT "),
                _          => false
            };

            if (valid)
                result.Add($"{startLineNo}\t{sql}\t{funcName}");
        }

        return result;
    }

    /// <summary>SQL の -- 行コメントと /* */ ブロックコメントを除去</summary>
    private static string RemoveComments(string src)
    {
        // ブロックコメント
        src = Regex.Replace(src, @"/\*.*?\*/", " ", RegexOptions.Singleline);
        // 行コメント
        src = Regex.Replace(src, @"--[^\n]*", "");
        return src;
    }

    // ─── Step 3: CRUD.tsv 書き出しヘルパー ───────────────────────────

    private static void WriteTableCrud(
        StringBuilder sb,
        Dictionary<string, string> tableDict,
        string moduleName, string programId, string lineNo, string crud, string funcName)
    {
        foreach (var kvp in tableDict)
        {
            var parts    = kvp.Value.Split('\t');
            var tblName  = parts.Length > 0 ? parts[0] : kvp.Key;
            sb.AppendLine($"{moduleName}\t{programId}\t{lineNo}\t{tblName}\t{crud}\t{funcName}\t");
        }
    }

    private static void WriteColumnCrud(
        StringBuilder sb,
        ColumnCollection columns,
        string moduleName, string programId, string lineNo, string crud, string funcName)
    {
        foreach (var col in columns)
        {
            if (string.IsNullOrEmpty(col.Table) || string.IsNullOrEmpty(col.ColumnName)) continue;
            sb.AppendLine($"{moduleName}\t{programId}\t{lineNo}\t{col.Table}.{col.ColumnName}\t{crud}\t{funcName}\t");
        }
    }

    // ─── Step 4: マトリクス生成 ───────────────────────────────────────

    /// <summary>
    /// CRUD.tsv → CRUDMatrix.tsv をピボット生成する。
    /// colIndex: テーブル/カラム列, progIndex: プログラムID列, crudIndex: CRUD列
    /// </summary>
    private static void GenerateMatrix(
        string crudTsvPath, string matrixTsvPath,
        int colIndex, int progIndex, int crudIndex)
    {
        // entity → programId → CRUDセット
        var dict = new Dictionary<string, Dictionary<string, HashSet<char>>>(StringComparer.OrdinalIgnoreCase);
        var programs = new List<string>(); // プログラムID順序保持

        foreach (var line in File.ReadAllLines(crudTsvPath, Encoding.UTF8))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var parts = line.Split('\t');
            if (parts.Length <= Math.Max(colIndex, Math.Max(progIndex, crudIndex))) continue;

            var entity  = parts[colIndex].Trim().ToUpperInvariant();
            var prog    = parts[progIndex].Trim();
            var crud    = parts[crudIndex].Trim().ToUpperInvariant();

            if (string.IsNullOrEmpty(entity) || string.IsNullOrEmpty(crud)) continue;

            if (!dict.TryGetValue(entity, out var progDict))
            {
                progDict = new Dictionary<string, HashSet<char>>(StringComparer.OrdinalIgnoreCase);
                dict[entity] = progDict;
            }

            if (!progDict.TryGetValue(prog, out var crudSet))
            {
                crudSet = new HashSet<char>();
                progDict[prog] = crudSet;
            }

            foreach (var c in crud) crudSet.Add(c);

            if (!programs.Contains(prog, StringComparer.OrdinalIgnoreCase))
                programs.Add(prog);
        }

        programs.Sort(StringComparer.OrdinalIgnoreCase);

        var sb = new StringBuilder();

        // ヘッダ行
        sb.Append("テーブル名\t論理名\t合計");
        foreach (var p in programs) sb.Append('\t').Append(p);
        sb.AppendLine();

        // 集計行（プログラム数）
        sb.Append("\t\tプログラム数→");
        foreach (var _ in programs) sb.Append("\t-");
        sb.AppendLine();

        // データ行
        foreach (var kv in dict.OrderBy(k => k.Key))
        {
            var entity       = kv.Key;
            // 全プログラムを合算したCRUD
            var totalSet     = new HashSet<char>();
            foreach (var ps in kv.Value.Values)
                foreach (var c in ps) totalSet.Add(c);
            var total = string.Concat("CRUD".Where(totalSet.Contains));

            sb.Append($"{entity}\t\t{total}");
            foreach (var p in programs)
            {
                kv.Value.TryGetValue(p, out var cs);
                var cell = cs != null ? string.Concat("CRUD".Where(cs.Contains)) : "";
                sb.Append('\t').Append(cell);
            }
            sb.AppendLine();
        }

        File.WriteAllText(matrixTsvPath, sb.ToString(), Encoding.UTF8);
    }
}
