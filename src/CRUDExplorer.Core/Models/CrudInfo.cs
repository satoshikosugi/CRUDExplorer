namespace CRUDExplorer.Core.Models;

/// <summary>
/// CRUD情報を保持するクラス
/// </summary>
public class CrudInfo
{
    public string TableName { get; set; } = string.Empty;
    public string AltName { get; set; } = string.Empty;
    public string FuncProcName { get; set; } = string.Empty;

    public bool C { get; set; }  // Create
    public bool R { get; set; }  // Read
    public bool U { get; set; }  // Update
    public bool D { get; set; }  // Delete

    // 参照CRUD（VIEW経由の間接参照）
    public bool RefC { get; set; }
    public bool RefR { get; set; }
    public bool RefU { get; set; }
    public bool RefD { get; set; }

    /// <summary>
    /// CRUD文字列を取得
    /// </summary>
    public string GetCRUD()
    {
        var result = string.Empty;
        if (C) result += "C";
        if (R) result += "R";
        if (U) result += "U";
        if (D) result += "D";
        return result;
    }

    /// <summary>
    /// 参照CRUD文字列を取得
    /// </summary>
    public string GetRefCRUD()
    {
        var result = string.Empty;
        if (RefC) result += "C";
        if (RefR) result += "R";
        if (RefU) result += "U";
        if (RefD) result += "D";
        return result;
    }
}
