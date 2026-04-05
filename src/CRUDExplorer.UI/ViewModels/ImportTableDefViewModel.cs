using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRUDExplorer.Core.Database;
using CRUDExplorer.Core.Database.Providers;
using CRUDExplorer.Core.Models;
using CRUDExplorer.Core.Utilities;

namespace CRUDExplorer.UI.ViewModels;

public partial class ImportTableDefViewModel : ViewModelBase
{
    private Action? _closeWindow;

    [ObservableProperty]
    private string _server = "localhost";

    [ObservableProperty]
    private string _port = "3306";

    [ObservableProperty]
    private string _database = string.Empty;

    [ObservableProperty]
    private string _userName = "root";

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private int _selectedDbType;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private int _importedTableCount;

    [ObservableProperty]
    private int _importedColumnCount;

    /// <summary>接続履歴のプルダウン用リスト</summary>
    public ObservableCollection<DbConnectionSettings> ConnectionHistoryList { get; } = new();

    [ObservableProperty]
    private DbConnectionSettings? _selectedConnectionHistory;

    partial void OnSelectedConnectionHistoryChanged(DbConnectionSettings? value)
    {
        if (value == null) return;
        SelectedDbType = value.DbTypeIndex;
        Server = value.Server;
        Port = value.Port;
        Database = value.Database;
        UserName = value.UserName;
        Password = value.GetPassword();
    }

    public ObservableCollection<string> DbTypes { get; } = new()
    {
        "MySQL",
        "PostgreSQL",
        "SQL Server",
        "Oracle",
        "SQLite",
        "MariaDB"
    };

    private static readonly DatabaseType[] DbTypeMap =
    {
        DatabaseType.MySQL,
        DatabaseType.PostgreSQL,
        DatabaseType.SqlServer,
        DatabaseType.Oracle,
        DatabaseType.SQLite,
        DatabaseType.MariaDB
    };

    public void SetCloseAction(Action closeWindow)
    {
        _closeWindow = closeWindow;
    }

    public void LoadSavedSettings()
    {
        var settings = Settings.Load();
        // 接続履歴をプルダウンに読み込む
        ConnectionHistoryList.Clear();
        foreach (var conn in settings.ConnectionHistory)
            ConnectionHistoryList.Add(conn);

        // 最後に使用した接続設定を復元
        var saved = settings.ImportDbConnection;
        if (saved != null)
        {
            SelectedDbType = saved.DbTypeIndex;
            Server = saved.Server;
            Port = saved.Port;
            Database = saved.Database;
            UserName = saved.UserName;
            Password = saved.GetPassword();
        }
    }

    private void SaveSettings()
    {
        var settings = Settings.Load();
        var current = new DbConnectionSettings
        {
            DbTypeIndex = SelectedDbType,
            Server = Server,
            Port = Port,
            Database = Database,
            UserName = UserName
        };
        current.SetPassword(Password);
        settings.ImportDbConnection = current;

        // 接続履歴に同一エントリがなければ追加（Server/Port/Database/UserName/DbTypeで一致判定）
        var existing = settings.ConnectionHistory.FindIndex(c =>
            c.DbTypeIndex == current.DbTypeIndex &&
            string.Equals(c.Server, current.Server, StringComparison.OrdinalIgnoreCase) &&
            c.Port == current.Port &&
            string.Equals(c.Database, current.Database, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(c.UserName, current.UserName, StringComparison.OrdinalIgnoreCase));

        if (existing >= 0)
        {
            // パスワードを更新
            settings.ConnectionHistory[existing].SetPassword(Password);
        }
        else
        {
            settings.ConnectionHistory.Add(current);
        }

        settings.Save();

        // プルダウンも更新
        ConnectionHistoryList.Clear();
        foreach (var conn in settings.ConnectionHistory)
            ConnectionHistoryList.Add(conn);
    }

    [RelayCommand]
    private void DeleteSelectedHistory()
    {
        if (SelectedConnectionHistory == null) return;

        var settings = Settings.Load();
        var target = SelectedConnectionHistory;
        settings.ConnectionHistory.RemoveAll(c =>
            c.DbTypeIndex == target.DbTypeIndex &&
            string.Equals(c.Server, target.Server, StringComparison.OrdinalIgnoreCase) &&
            c.Port == target.Port &&
            string.Equals(c.Database, target.Database, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(c.UserName, target.UserName, StringComparison.OrdinalIgnoreCase));
        settings.Save();

        ConnectionHistoryList.Remove(target);
        SelectedConnectionHistory = null;
        StatusMessage = "接続履歴を削除しました";
    }

    private string BuildConnectionString()
    {
        var dbType = DbTypeMap[SelectedDbType];
        return dbType switch
        {
            DatabaseType.MySQL or DatabaseType.MariaDB =>
                $"Server={Server};Port={Port};Database={Database};Uid={UserName};Pwd={Password};SslMode=None;AllowPublicKeyRetrieval=True;",
            DatabaseType.PostgreSQL =>
                $"Host={Server};Port={Port};Database={Database};Username={UserName};Password={Password};",
            DatabaseType.SqlServer =>
                $"Server={Server},{Port};Database={Database};User Id={UserName};Password={Password};TrustServerCertificate=True;",
            DatabaseType.Oracle =>
                $"Data Source={Server}:{Port}/{Database};User Id={UserName};Password={Password};",
            DatabaseType.SQLite =>
                $"Data Source={Database};",
            _ => string.Empty
        };
    }

    private ISchemaProvider CreateSchemaProvider()
    {
        var dbType = DbTypeMap[SelectedDbType];
        return dbType switch
        {
            DatabaseType.MySQL => new MySqlSchemaProvider(),
            DatabaseType.PostgreSQL => new PostgreSqlSchemaProvider(),
            DatabaseType.SqlServer => new SqlServerSchemaProvider(),
            DatabaseType.Oracle => new OracleSchemaProvider(),
            DatabaseType.SQLite => new SqliteSchemaProvider(),
            DatabaseType.MariaDB => new MariaDbSchemaProvider(),
            _ => throw new NotSupportedException($"Unsupported DB type: {dbType}")
        };
    }

    [RelayCommand]
    private async Task TestConnection()
    {
        if (string.IsNullOrEmpty(Database))
        {
            StatusMessage = "データベース名を入力してください";
            return;
        }

        IsLoading = true;
        StatusMessage = "接続テスト中...";

        try
        {
            var provider = CreateSchemaProvider();
            var connStr = BuildConnectionString();
            var tables = await provider.GetTablesAsync(connStr);
            StatusMessage = $"接続成功: {tables.Count} テーブルを検出";
        }
        catch (Exception ex)
        {
            StatusMessage = $"接続失敗: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ImportTableDef()
    {
        if (string.IsNullOrEmpty(Database))
        {
            StatusMessage = "データベース名を入力してください";
            return;
        }

        IsLoading = true;
        StatusMessage = "テーブル定義を取得中...";

        try
        {
            var provider = CreateSchemaProvider();
            var connStr = BuildConnectionString();
            var tableDefs = await provider.GetAllTableDefinitionsAsync(connStr);

            // GlobalStateに保存
            var gs = GlobalState.Instance;
            gs.TableDefinitions.Clear();
            gs.TableNames.Clear();

            int totalColumns = 0;
            foreach (var kvp in tableDefs)
            {
                var upperName = kvp.Key.ToUpperInvariant();
                gs.TableDefinitions[upperName] = kvp.Value;

                // TABLE_COMMENT をエンティティ名として保存
                // (MySQL INFORMATION_SCHEMA から取得した場合、AttributeName にカラムコメントが入る)
                // テーブル名→テーブル名（論理名はDB上のコメントから取得できる場合のみ）
                if (!gs.TableNames.ContainsKey(upperName))
                    gs.TableNames[upperName] = upperName;

                totalColumns += kvp.Value.Columns.Count;
            }

            ImportedTableCount = tableDefs.Count;
            ImportedColumnCount = totalColumns;
            StatusMessage = $"取込完了: {tableDefs.Count} テーブル, {totalColumns} カラム";
            SaveSettings();
        }
        catch (Exception ex)
        {
            StatusMessage = $"取込失敗: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ImportWithComments()
    {
        if (string.IsNullOrEmpty(Database))
        {
            StatusMessage = "データベース名を入力してください";
            return;
        }

        IsLoading = true;
        StatusMessage = "テーブル定義(コメント付き)を取得中...";

        try
        {
            var connStr = BuildConnectionString();
            var dbType = DbTypeMap[SelectedDbType];

            if (dbType != DatabaseType.MySQL && dbType != DatabaseType.MariaDB)
            {
                StatusMessage = "コメント付き取込はMySQL/MariaDBのみ対応しています";
                IsLoading = false;
                return;
            }

            var tableDefs = await ImportMySqlWithCommentsAsync(connStr);
            var gs = GlobalState.Instance;
            gs.TableDefinitions.Clear();
            gs.TableNames.Clear();

            int totalColumns = 0;
            foreach (var kvp in tableDefs)
            {
                gs.TableDefinitions[kvp.Key] = kvp.Value.TableDef;
                gs.TableNames[kvp.Key] = kvp.Value.TableComment;
                totalColumns += kvp.Value.TableDef.Columns.Count;
            }

            ImportedTableCount = tableDefs.Count;
            ImportedColumnCount = totalColumns;
            StatusMessage = $"取込完了: {tableDefs.Count} テーブル, {totalColumns} カラム (コメント付き)";
            SaveSettings();
        }
        catch (Exception ex)
        {
            StatusMessage = $"取込失敗: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private record TableWithComment(TableDefinition TableDef, string TableComment);

    private static async Task<Dictionary<string, TableWithComment>> ImportMySqlWithCommentsAsync(string connectionString)
    {
        var result = new Dictionary<string, TableWithComment>(StringComparer.OrdinalIgnoreCase);

        using var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
        await connection.OpenAsync();
        var schemaName = connection.Database;

        // テーブル名とコメントを取得
        var tableQuery = @"
            SELECT table_name, table_comment
            FROM information_schema.tables
            WHERE table_schema = @schema
              AND table_type = 'BASE TABLE'
            ORDER BY table_name";

        var tableComments = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(tableQuery, connection))
        {
            cmd.Parameters.AddWithValue("@schema", schemaName);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var name = reader.GetString(0).ToUpperInvariant();
                var comment = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                tableComments[name] = string.IsNullOrEmpty(comment) ? name : comment;
            }
        }

        // カラム名とコメントを取得
        var columnQuery = @"
            SELECT
                c.table_name,
                c.column_name,
                c.ordinal_position,
                c.data_type,
                c.character_maximum_length,
                c.numeric_precision,
                c.numeric_scale,
                c.is_nullable,
                c.column_key,
                c.column_comment
            FROM information_schema.columns c
            WHERE c.table_schema = @schema
            ORDER BY c.table_name, c.ordinal_position";

        using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(columnQuery, connection))
        {
            cmd.Parameters.AddWithValue("@schema", schemaName);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var tableName = reader.GetString(0).ToUpperInvariant();
                var columnName = reader.GetString(1).ToUpperInvariant();
                var columnComment = reader.IsDBNull(9) ? string.Empty : reader.GetString(9);

                if (!result.ContainsKey(tableName))
                {
                    var tableComment = tableComments.TryGetValue(tableName, out var tc) ? tc : tableName;
                    result[tableName] = new TableWithComment(new TableDefinition(), tableComment);
                }

                var colDef = new CRUDExplorer.Core.Models.ColumnDefinition
                {
                    TableName = tableName,
                    ColumnName = columnName,
                    AttributeName = string.IsNullOrEmpty(columnComment) ? columnName : columnComment,
                    Sequence = reader.GetInt32(2).ToString(),
                    DataType = reader.GetString(3),
                    Digits = reader.IsDBNull(4) ? (reader.IsDBNull(5) ? string.Empty : reader.GetValue(5).ToString()!) : reader.GetValue(4).ToString()!,
                    Accuracy = reader.IsDBNull(6) ? string.Empty : reader.GetValue(6).ToString()!,
                    Required = reader.GetString(7) == "NO" ? "NOT NULL" : string.Empty,
                    PrimaryKey = reader.GetString(8) == "PRI" ? "PK" : string.Empty,
                    ForeignKey = string.Empty
                };
                result[tableName].TableDef.Columns[columnName] = colDef;
            }
        }

        return result;
    }

    [RelayCommand]
    private void Close()
    {
        _closeWindow?.Invoke();
    }
}
