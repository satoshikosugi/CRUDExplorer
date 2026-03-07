using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRUDExplorer.Core.Models;
using CRUDExplorer.Core.Utilities;

namespace CRUDExplorer.UI.ViewModels;

public partial class CrudSearchViewModel : ViewModelBase
{
    private readonly Action _closeWindow;
    private const int MaxDetailTextLength = 80;

    [ObservableProperty]
    private string _tableNamePattern = string.Empty;

    [ObservableProperty]
    private string _columnNamePattern = string.Empty;

    [ObservableProperty]
    private bool _searchCreate = true;

    [ObservableProperty]
    private bool _searchRead = true;

    [ObservableProperty]
    private bool _searchUpdate = true;

    [ObservableProperty]
    private bool _searchDelete = true;

    [ObservableProperty]
    private ObservableCollection<CrudSearchResult> _searchResults = new();

    [ObservableProperty]
    private int _resultCount = 0;

    public CrudSearchViewModel(Action? closeWindow = null)
    {
        _closeWindow = closeWindow ?? (() => { });
    }

    [RelayCommand]
    private void Search()
    {
        SearchResults.Clear();

        var queryList = GlobalState.Instance.QueryList;
        var programNames = GlobalState.Instance.ProgramNames;

        foreach (var kvp in queryList)
        {
            var query = kvp.Value;

            // CRUD種別ごとにテーブルを確認
            CheckAndAddResults(query, query.TableC, "C", query.ColumnC);
            CheckAndAddResults(query, query.TableR, "R", query.ColumnR);
            CheckAndAddResults(query, query.TableU, "U", query.ColumnU);
            CheckAndAddResults(query, query.TableD, "D", query.ColumnD);
        }

        ResultCount = SearchResults.Count;
    }

    private void CheckAndAddResults(
        Query query,
        Dictionary<string, string> tables,
        string crudType,
        Dictionary<string, object> columns)
    {
        // CRUD種別フィルタ
        if (crudType == "C" && !SearchCreate) return;
        if (crudType == "R" && !SearchRead) return;
        if (crudType == "U" && !SearchUpdate) return;
        if (crudType == "D" && !SearchDelete) return;

        foreach (var tableEntry in tables.Values)
        {
            // tableEntryは "テーブル名\tエイリアス" 形式
            var parts = tableEntry.Split('\t');
            var tableName = parts[0];

            // テーブル名パターンフィルタ
            if (!string.IsNullOrWhiteSpace(TableNamePattern)
                && !Regex.IsMatch(tableName, TableNamePattern, RegexOptions.IgnoreCase))
                continue;

            if (!string.IsNullOrWhiteSpace(ColumnNamePattern))
            {
                // カラム名フィルタ：一致するカラムを探す
                foreach (var colKey in columns.Keys)
                {
                    if (Regex.IsMatch(colKey, ColumnNamePattern, RegexOptions.IgnoreCase))
                    {
                        AddResult(query, tableName, colKey, crudType);
                    }
                }
            }
            else
            {
                AddResult(query, tableName, string.Empty, crudType);
            }
        }
    }

    private void AddResult(Query query, string tableName, string columnName, string crudType)
    {
        var programNames = GlobalState.Instance.ProgramNames;
        var programId = System.IO.Path.GetFileNameWithoutExtension(query.FileName);
        if (programNames.TryGetValue(programId, out var programName))
            programId = $"{programName}({programId})";

        var displayText = string.IsNullOrEmpty(columnName)
            ? $"[{crudType}] {tableName}  {programId}  ({query.FileName}:{query.LineNo})"
            : $"[{crudType}] {tableName}.{columnName}  {programId}  ({query.FileName}:{query.LineNo})";

        SearchResults.Add(new CrudSearchResult
        {
            DisplayText = displayText,
            DetailText = query.QueryText.Length > MaxDetailTextLength ? query.QueryText[..MaxDetailTextLength] + "..." : query.QueryText,
            FileName = query.FileName,
            LineNo = query.LineNo,
            TableName = tableName,
            ColumnName = columnName,
            CrudType = crudType
        });
    }

    [RelayCommand]
    private void Close()
    {
        _closeWindow();
    }
}

public class CrudSearchResult
{
    public string DisplayText { get; set; } = string.Empty;
    public string DetailText { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public int LineNo { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public string CrudType { get; set; } = string.Empty;
}
