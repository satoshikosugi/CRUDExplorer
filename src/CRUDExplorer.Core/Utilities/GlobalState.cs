using CRUDExplorer.Core.Models;

namespace CRUDExplorer.Core.Utilities;

/// <summary>
/// グローバル状態管理（VB.NET CommonModule.vbのグローバル変数から移植）
/// アプリケーション全体で共有するデータを管理
/// </summary>
public class GlobalState
{
    private static readonly Lazy<GlobalState> _lazy = new(() => new GlobalState());

    /// <summary>
    /// シングルトンインスタンス
    /// </summary>
    public static GlobalState Instance => _lazy.Value;

    private GlobalState()
    {
    }

    /// <summary>
    /// アプリケーション設定（VB.NET objSettings相当）
    /// </summary>
    public Settings AppSettings { get; set; } = new();

    /// <summary>
    /// テーブル名辞書（物理名→論理名、VB.NET dctTableName相当）
    /// </summary>
    public Dictionary<string, string> TableNames { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// テーブル定義辞書（テーブル名→TableDefinition、VB.NET dctTableDef相当）
    /// </summary>
    public Dictionary<string, TableDefinition> TableDefinitions { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// プログラム名辞書（プログラムID→プログラム名、VB.NET dctProgramName相当）
    /// </summary>
    public Dictionary<string, string> ProgramNames { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// プログラム別CRUD辞書（VB.NET dctCRUDProgram相当）
    /// </summary>
    public Dictionary<string, object> CrudPrograms { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// テーブル別CRUD辞書（VB.NET dctCRUDTable相当）
    /// </summary>
    public Dictionary<string, object> CrudTables { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 参照条件辞書（VB.NET dctReferenceCond相当）
    /// </summary>
    public Dictionary<string, object> ReferenceConditions { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// クエリリスト辞書（VB.NET dctQueryList相当）
    /// </summary>
    public Dictionary<string, Query> QueryList { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// ファイル辞書（VB.NET dctFiles相当）
    /// </summary>
    public Dictionary<string, object> Files { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// VIEWコレクション（VB.NET colViews相当）
    /// </summary>
    public ViewCollection Views { get; set; } = new();

    /// <summary>
    /// デモフラグ（VB.NET bolDemoFlag相当）
    /// </summary>
    public bool IsDemoMode { get; set; } = true;

    /// <summary>
    /// 開始画面表示フラグ（VB.NET bolShowStartup相当）
    /// </summary>
    public bool ShowStartup { get; set; } = true;

    /// <summary>
    /// MakeCRUD解析の最後の出力先パス（MainWindowの自動リロード用）
    /// </summary>
    public string LastAnalysisDestPath { get; set; } = string.Empty;

    /// <summary>
    /// グローバル状態をリセット
    /// </summary>
    public void Reset()
    {
        AppSettings = new Settings();
        TableNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        TableDefinitions = new Dictionary<string, TableDefinition>(StringComparer.OrdinalIgnoreCase);
        ProgramNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        CrudPrograms = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        CrudTables = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        ReferenceConditions = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        QueryList = new Dictionary<string, Query>(StringComparer.OrdinalIgnoreCase);
        Files = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        Views = new ViewCollection();
        IsDemoMode = true;
        ShowStartup = true;
        LastAnalysisDestPath = string.Empty;
    }
}
