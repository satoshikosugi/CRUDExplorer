namespace CRUDExplorer.Core.Models;

/// <summary>
/// コンテキストメニュー情報（VB.NET MenuContents.vbから移植）
/// メニュー項目に関連付けるデータを保持
/// </summary>
public class MenuContents
{
    /// <summary>
    /// メニュー種別（コピー対象種別やカラムインデックス等）
    /// </summary>
    public string MenuKind { get; set; }

    /// <summary>
    /// ソースフォルダパス
    /// </summary>
    public string SourcePath { get; set; }

    /// <summary>
    /// 対象ファイル名
    /// </summary>
    public string FileName { get; set; }

    public MenuContents(string menuKind, string sourcePath, string fileName = "")
    {
        MenuKind = menuKind;
        SourcePath = sourcePath;
        FileName = fileName;
    }
}
