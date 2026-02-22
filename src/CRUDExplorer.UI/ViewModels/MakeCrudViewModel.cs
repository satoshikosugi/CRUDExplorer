using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CRUDExplorer.UI.ViewModels;

public partial class MakeCrudViewModel : ViewModelBase
{
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

    [RelayCommand]
    private void SelectSourceFolder()
    {
        // TODO: Implement folder picker dialog for source
        AppendLog("ソースフォルダを選択してください...");
    }

    [RelayCommand]
    private void SelectDestFolder()
    {
        // TODO: Implement folder picker dialog for destination
        AppendLog("出力先フォルダを選択してください...");
    }

    [RelayCommand]
    private void ExecuteAnalysis()
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

        AppendLog("=== CRUD解析を開始します ===");
        AppendLog($"ソースフォルダ: {SourcePath}");
        AppendLog($"出力先フォルダ: {DestPath}");

        // TODO: Implement multi-step CRUD analysis
        // Step 0: Source Copy
        if (ProcessAll || Step0_SourceCopy)
        {
            AppendLog("[Step 0] ソースコピーを実行中...");
            // Use FileSystemHelper to copy source files
            AppendLog("[Step 0] 完了");
        }

        // Step 1: Extract Dynamic SQL
        if (ProcessAll || Step1_ExtractDynamicSql)
        {
            AppendLog("[Step 1] 動的SQL抽出を実行中...");
            // Use SqlAnalyzer to extract dynamic SQL
            AppendLog("[Step 1] 完了");
        }

        // Step 2: Extract Query
        if (ProcessAll || Step2_ExtractQuery)
        {
            AppendLog("[Step 2] クエリ抽出を実行中...");
            // Use SqlAnalyzer to extract queries
            AppendLog("[Step 2] 完了");
        }

        // Step 3: Analyze CRUD
        if (ProcessAll || Step3_AnalyzeCrud)
        {
            AppendLog("[Step 3] CRUD解析を実行中...");
            // Use SqlAnalyzer to analyze CRUD operations
            AppendLog("[Step 3] 完了");
        }

        // Step 4: Generate CRUD Matrix
        if (ProcessAll || Step4_GenerateMatrix)
        {
            AppendLog("[Step 4] CRUDマトリクス生成を実行中...");
            // Generate CRUD matrix TSV files
            AppendLog("[Step 4] 完了");
        }

        AppendLog("=== CRUD解析が完了しました ===");
    }

    [RelayCommand]
    private void Close()
    {
        // TODO: Close window
        AppendLog("ウィンドウを閉じます");
    }

    private void AppendLog(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        AnalysisLog += $"[{timestamp}] {message}\n";
    }
}
