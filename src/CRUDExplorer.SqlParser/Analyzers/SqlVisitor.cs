using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CRUDExplorer.Core.Models;

namespace CRUDExplorer.SqlParser.Analyzers;

/// <summary>
/// SQLビジター（ANTLR4ビジターパターン）
/// パース結果のASTを走査してQueryオブジェクトに情報を格納
/// </summary>
internal class SqlVisitor : Grammar.SqlBaseVisitor<object?>
{
    private readonly Query _query;

    public SqlVisitor(Query query)
    {
        _query = query;
    }

    public override object? VisitSelectStatement([NotNull] Grammar.SqlParser.SelectStatementContext context)
    {
        _query.QueryKind = "SELECT";

        // SELECT句の処理
        if (context.selectClause() != null)
        {
            VisitSelectClause(context.selectClause());
        }

        // FROM句の処理
        if (context.fromClause() != null)
        {
            VisitFromClause(context.fromClause());
        }

        // WHERE句の処理
        if (context.whereClause() != null)
        {
            VisitWhereClause(context.whereClause());
        }

        // GROUP BY句の処理
        if (context.groupByClause() != null)
        {
            VisitGroupByClause(context.groupByClause());
        }

        // HAVING句の処理
        if (context.havingClause() != null)
        {
            VisitHavingClause(context.havingClause());
        }

        // ORDER BY句の処理
        if (context.orderByClause() != null)
        {
            VisitOrderByClause(context.orderByClause());
        }

        // WITH句の処理
        if (context.withClause() != null)
        {
            VisitWithClause(context.withClause());
        }

        // UNION/MINUS/INTERSECTの処理
        // Note: QueryKindは"SELECT"のまま維持する（UNIONはSELECTの一種として扱う）
        // UNION/MINUS/INTERSECT の右側のSELECT文を処理
        if ((context.UNION() != null || context.MINUS() != null || context.INTERSECT() != null) && context.selectStatement() != null)
        {
            // 右側のSELECT文を訪問（同じQueryオブジェクトにマージ）
            var rightSelect = context.selectStatement();
            if (rightSelect.fromClause() != null)
            {
                VisitFromClause(rightSelect.fromClause());
            }
            if (rightSelect.selectClause() != null)
            {
                VisitSelectClause(rightSelect.selectClause());
            }
            if (rightSelect.whereClause() != null)
            {
                VisitWhereClause(rightSelect.whereClause());
            }
        }

        return null;
    }

    public override object? VisitSelectClause([NotNull] Grammar.SqlParser.SelectClauseContext context)
    {
        // SELECT句内の各項目を処理
        var selectList = context.selectList();
        if (selectList != null)
        {
            foreach (var selectItem in selectList.selectItem())
            {
                var expression = selectItem.expression();
                if (expression != null)
                {
                    ProcessSelectExpression(expression, selectItem);
                }
            }
        }
        return null;
    }

    public override object? VisitFromClause([NotNull] Grammar.SqlParser.FromClauseContext context)
    {
        // FROM句内の各テーブル参照を処理
        foreach (var tableRef in context.tableReference())
        {
            ProcessTableReference(tableRef);
        }
        return null;
    }

    public override object? VisitInsertStatement([NotNull] Grammar.SqlParser.InsertStatementContext context)
    {
        _query.QueryKind = "INSERT";

        // テーブル名取得
        var tableName = context.tableName()?.GetText();
        if (!string.IsNullOrEmpty(tableName))
        {
            _query.TableC[tableName] = $"{tableName}\t";
        }

        // カラムリスト処理
        var columnList = context.columnList();
        if (columnList != null)
        {
            foreach (var column in columnList.identifier())
            {
                var columnName = column.GetText();
                var col = new Column(columnName, tableName ?? "", "", ClauseKind.Insert);
                _query.ColumnInsert.Add(col);
                _query.ColumnC[$"{tableName}.{columnName}"] = col;
            }
        }

        // INSERT ... SELECT の処理
        // SELECT文をサブクエリとして処理するのではなく、直接テーブルとカラムを現在のクエリに追加
        var selectStmt = context.selectStatement();
        if (selectStmt != null)
        {
            // FROM句のテーブルを処理
            if (selectStmt.fromClause() != null)
            {
                VisitFromClause(selectStmt.fromClause());
            }
            // SELECT句のカラムを処理
            if (selectStmt.selectClause() != null)
            {
                VisitSelectClause(selectStmt.selectClause());
            }
            // WHERE句の処理
            if (selectStmt.whereClause() != null)
            {
                VisitWhereClause(selectStmt.whereClause());
            }
        }

        return null;
    }

    public override object? VisitUpdateStatement([NotNull] Grammar.SqlParser.UpdateStatementContext context)
    {
        _query.QueryKind = "UPDATE";

        // テーブル名取得
        var tableName = context.tableName()?.GetText();
        if (!string.IsNullOrEmpty(tableName))
        {
            _query.TableU[tableName] = $"{tableName}\t";
        }

        // SET句処理
        var setClauseList = context.setClauseList();
        if (setClauseList != null)
        {
            foreach (var setClause in setClauseList.setClause())
            {
                var columnName = setClause.identifier()?.GetText();
                if (!string.IsNullOrEmpty(columnName))
                {
                    // 更新されるカラム (ColumnUpdate)
                    var colUpdate = new Column(columnName, tableName ?? "", "", ClauseKind.Update);
                    _query.ColumnUpdate.Add(colUpdate);
                    _query.ColumnU[$"{tableName}.{columnName}"] = colUpdate;

                    // SET句の条件に含まれるカラム (ColumnSetCond) - 左辺も含む
                    var colSetCond = new Column(columnName, tableName ?? "", "", ClauseKind.SetCondition);
                    _query.ColumnSetCond.Add(colSetCond);
                }

                // SET句の右辺（参照カラム）を処理
                var expression = setClause.expression();
                if (expression != null)
                {
                    ProcessSetConditionExpression(expression, tableName ?? "");
                }
            }
        }

        // WHERE句処理
        if (context.whereClause() != null)
        {
            VisitWhereClause(context.whereClause());
        }

        return null;
    }

    public override object? VisitDeleteStatement([NotNull] Grammar.SqlParser.DeleteStatementContext context)
    {
        _query.QueryKind = "DELETE";

        // テーブル名取得
        var tableName = context.tableName()?.GetText();
        if (!string.IsNullOrEmpty(tableName))
        {
            _query.TableD[tableName] = $"{tableName}\t";
        }

        // WHERE句処理
        if (context.whereClause() != null)
        {
            VisitWhereClause(context.whereClause());
        }

        return null;
    }

    private void ProcessSelectExpression(Grammar.SqlParser.ExpressionContext expression, Grammar.SqlParser.SelectItemContext selectItem)
    {
        // カラム参照の場合
        if (expression is Grammar.SqlParser.ColumnReferenceExpressionContext colRef)
        {
            var columnName = colRef.identifier()?.GetText();
            if (!string.IsNullOrEmpty(columnName))
            {
                var col = new Column(columnName, "", "", ClauseKind.Select);
                _query.ColumnSelect.Add(col);

                // エイリアスがある場合は記録
                var alias = selectItem.identifier()?.GetText();
                if (!string.IsNullOrEmpty(alias))
                {
                    _query.Selects[alias] = columnName;
                }
            }
        }
        // 修飾カラム参照の場合（table.column）
        else if (expression is Grammar.SqlParser.QualifiedColumnExpressionContext qualifiedCol)
        {
            var identifiers = qualifiedCol.identifier();
            if (identifiers.Length == 2)
            {
                var tableName = identifiers[0].GetText();
                var columnName = identifiers[1].GetText();
                var col = new Column(columnName, tableName, "", ClauseKind.Select);
                _query.ColumnSelect.Add(col);
                _query.ColumnR[$"{tableName}.{columnName}"] = col;

                // エイリアスがある場合は記録
                var alias = selectItem.identifier()?.GetText();
                if (!string.IsNullOrEmpty(alias))
                {
                    _query.Selects[alias] = $"{tableName}.{columnName}";
                }
            }
        }
        // サブクエリの場合
        else if (expression is Grammar.SqlParser.SubqueryExpressionContext subqueryExpr)
        {
            var subquery = subqueryExpr.selectStatement();
            if (subquery != null)
            {
                ProcessSubquery(subquery);
            }
        }
        // 関数呼び出しの場合 (例: COUNT(*), SUM(amount), UPPER(name))
        else if (expression is Grammar.SqlParser.FunctionCallExpressionContext function)
        {
            var exprList = function.expressionList();
            if (exprList != null)
            {
                foreach (var expr in exprList.expression())
                {
                    ExtractColumnsFromExpression(expr);
                }
            }
        }
        // 算術式の場合 (例: price * quantity, amount + tax)
        else if (expression is Grammar.SqlParser.ArithmeticExpressionContext arithmetic)
        {
            ExtractColumnsFromExpression(arithmetic.expression(0));
            ExtractColumnsFromExpression(arithmetic.expression(1));
        }
        // CASE式の場合
        else if (expression is Grammar.SqlParser.CaseExpressionContext caseExpr)
        {
            foreach (var whenClause in caseExpr.caseWhenClause())
            {
                ExtractColumnsFromExpression(whenClause.expression(0));
                ExtractColumnsFromExpression(whenClause.expression(1));
            }
            if (caseExpr.expression() != null)
            {
                ExtractColumnsFromExpression(caseExpr.expression());
            }
        }
        // 括弧付き式の場合
        else if (expression is Grammar.SqlParser.ParenthesizedExpressionContext parenthesized)
        {
            ExtractColumnsFromExpression(parenthesized.expression());
        }
    }

    // SELECT句内の式からカラム参照を抽出するヘルパー
    private void ExtractColumnsFromExpression(Grammar.SqlParser.ExpressionContext expression)
    {
        if (expression is Grammar.SqlParser.ColumnReferenceExpressionContext colRef)
        {
            var columnName = colRef.identifier()?.GetText();
            if (!string.IsNullOrEmpty(columnName))
            {
                var col = new Column(columnName, "", "", ClauseKind.Select);
                _query.ColumnSelect.Add(col);
            }
        }
        else if (expression is Grammar.SqlParser.QualifiedColumnExpressionContext qualifiedCol)
        {
            var identifiers = qualifiedCol.identifier();
            if (identifiers.Length == 2)
            {
                var tableName = identifiers[0].GetText();
                var columnName = identifiers[1].GetText();
                var col = new Column(columnName, tableName, "", ClauseKind.Select);
                _query.ColumnSelect.Add(col);
                _query.ColumnR[$"{tableName}.{columnName}"] = col;
            }
        }
        else if (expression is Grammar.SqlParser.FunctionCallExpressionContext function)
        {
            var exprList = function.expressionList();
            if (exprList != null)
            {
                foreach (var expr in exprList.expression())
                {
                    ExtractColumnsFromExpression(expr);
                }
            }
        }
        else if (expression is Grammar.SqlParser.ArithmeticExpressionContext arithmetic)
        {
            ExtractColumnsFromExpression(arithmetic.expression(0));
            ExtractColumnsFromExpression(arithmetic.expression(1));
        }
        else if (expression is Grammar.SqlParser.ParenthesizedExpressionContext parenthesized)
        {
            ExtractColumnsFromExpression(parenthesized.expression());
        }
        else if (expression is Grammar.SqlParser.CaseExpressionContext caseExpr)
        {
            foreach (var whenClause in caseExpr.caseWhenClause())
            {
                ExtractColumnsFromExpression(whenClause.expression(0));
                ExtractColumnsFromExpression(whenClause.expression(1));
            }
            if (caseExpr.expression() != null)
            {
                ExtractColumnsFromExpression(caseExpr.expression());
            }
        }
    }

    private void ProcessTableReference(Grammar.SqlParser.TableReferenceContext tableRef)
    {
        // ANTLR の左再帰除去により、tableReference : tableReference joinClause の場合、
        // 左側の tableReference は tableReference() メソッドで取得できる子ノードとなる
        var childTableRef = tableRef.tableReference();
        if (childTableRef != null)
        {
            // 再帰的に左側の tableReference を処理
            ProcessTableReference(childTableRef);
        }

        // テーブル名の取得 (tableReference : tableName (AS? tableAlias)?)
        var tableName = tableRef.tableName()?.GetText();
        var tableAlias = tableRef.tableAlias()?.GetText();

        if (!string.IsNullOrEmpty(tableName))
        {
            var key = tableName;
            var value = string.IsNullOrEmpty(tableAlias)
                ? $"{tableName}\t"
                : $"{tableName}\t{tableAlias}";

            _query.TableR[key] = value;
        }

        // サブクエリの処理 (tableReference : '(' selectStatement ')' (AS? tableAlias)?)
        var subquery = tableRef.selectStatement();
        if (subquery != null)
        {
            ProcessSubquery(subquery);
        }

        // JOIN句の処理 (tableReference : tableReference joinClause)
        var joinClause = tableRef.joinClause();
        if (joinClause != null)
        {
            VisitJoinClause(joinClause);
        }
    }

    private void ProcessSetConditionExpression(Grammar.SqlParser.ExpressionContext expression, string tableName)
    {
        // SET句の右辺で参照されるカラムを抽出
        if (expression is Grammar.SqlParser.ColumnReferenceExpressionContext colRef)
        {
            var columnName = colRef.identifier()?.GetText();
            if (!string.IsNullOrEmpty(columnName))
            {
                var col = new Column(columnName, tableName, "", ClauseKind.SetCondition);
                _query.ColumnSetCond.Add(col);
            }
        }
        else if (expression is Grammar.SqlParser.QualifiedColumnExpressionContext qualifiedCol)
        {
            var identifiers = qualifiedCol.identifier();
            if (identifiers.Length == 2)
            {
                var table = identifiers[0].GetText();
                var columnName = identifiers[1].GetText();
                var col = new Column(columnName, table, "", ClauseKind.SetCondition);
                _query.ColumnSetCond.Add(col);
            }
        }
        // 算術式の場合 (例: SET value = value + 1)
        else if (expression is Grammar.SqlParser.ArithmeticExpressionContext arithmetic)
        {
            ProcessSetConditionExpression(arithmetic.expression(0), tableName);
            ProcessSetConditionExpression(arithmetic.expression(1), tableName);
        }
        // 関数呼び出しの場合 (例: SET name = UPPER(name))
        else if (expression is Grammar.SqlParser.FunctionCallExpressionContext function)
        {
            var exprList = function.expressionList();
            if (exprList != null)
            {
                foreach (var expr in exprList.expression())
                {
                    ProcessSetConditionExpression(expr, tableName);
                }
            }
        }
        // CASE式の場合
        else if (expression is Grammar.SqlParser.CaseExpressionContext caseExpr)
        {
            foreach (var whenClause in caseExpr.caseWhenClause())
            {
                ProcessSetConditionExpression(whenClause.expression(0), tableName);
                ProcessSetConditionExpression(whenClause.expression(1), tableName);
            }
            if (caseExpr.expression() != null)
            {
                ProcessSetConditionExpression(caseExpr.expression(), tableName);
            }
        }
        // サブクエリの場合
        else if (expression is Grammar.SqlParser.SubqueryExpressionContext subqueryExpr)
        {
            var subquery = subqueryExpr.selectStatement();
            if (subquery != null)
            {
                ProcessSubquery(subquery);
            }
        }
        // 括弧付き式の場合
        else if (expression is Grammar.SqlParser.ParenthesizedExpressionContext parenthesized)
        {
            ProcessSetConditionExpression(parenthesized.expression(), tableName);
        }
    }

    private void ProcessSubquery(Grammar.SqlParser.SelectStatementContext subquery)
    {
        // サブクエリを別のQueryオブジェクトとして解析
        var subQueryObj = new Query
        {
            QueryText = subquery.GetText(),
            Parent = _query,
            SubQueryIndex = $"SQ{_query.SubQueries.Count + 1}"
        };

        var subVisitor = new SqlVisitor(subQueryObj);
        subVisitor.Visit(subquery);

        _query.SubQueries[subQueryObj.SubQueryIndex] = subQueryObj;

        // サブクエリ内のテーブルを親クエリのTableRにマージ
        foreach (var kvp in subQueryObj.TableR)
        {
            if (!_query.TableR.ContainsKey(kvp.Key))
            {
                _query.TableR[kvp.Key] = kvp.Value;
            }
        }
    }

    public override object? VisitWhereClause([NotNull] Grammar.SqlParser.WhereClauseContext context)
    {
        // WHERE句内の式を処理してカラム参照を抽出
        var expression = context.expression();
        if (expression != null)
        {
            ProcessWhereExpression(expression);
        }
        return null;
    }

    private void ProcessWhereExpression(Grammar.SqlParser.ExpressionContext expression)
    {
        // カラム参照の場合
        if (expression is Grammar.SqlParser.ColumnReferenceExpressionContext colRef)
        {
            var columnName = colRef.identifier()?.GetText();
            if (!string.IsNullOrEmpty(columnName))
            {
                var col = new Column(columnName, "", "", ClauseKind.Where);
                _query.ColumnWhere.Add(col);
            }
        }
        else if (expression is Grammar.SqlParser.QualifiedColumnExpressionContext qualifiedCol)
        {
            var identifiers = qualifiedCol.identifier();
            if (identifiers.Length == 2)
            {
                var tableName = identifiers[0].GetText();
                var columnName = identifiers[1].GetText();
                var col = new Column(columnName, tableName, "", ClauseKind.Where);
                _query.ColumnWhere.Add(col);
            }
        }
        // 比較式の場合 (例: id = 1, name <> 'test')
        else if (expression is Grammar.SqlParser.ComparisonExpressionContext comparison)
        {
            ProcessWhereExpression(comparison.expression(0));
            ProcessWhereExpression(comparison.expression(1));
        }
        // 論理式の場合 (AND, OR)
        else if (expression is Grammar.SqlParser.LogicalExpressionContext logical)
        {
            ProcessWhereExpression(logical.expression(0));
            ProcessWhereExpression(logical.expression(1));
        }
        // NOT式の場合
        else if (expression is Grammar.SqlParser.NotExpressionContext notExpr)
        {
            ProcessWhereExpression(notExpr.expression());
        }
        // IS NULL / IS NOT NULL の場合
        else if (expression is Grammar.SqlParser.IsNullExpressionContext isNull)
        {
            ProcessWhereExpression(isNull.expression());
        }
        // IN句の場合
        else if (expression is Grammar.SqlParser.InExpressionContext inExpr)
        {
            ProcessWhereExpression(inExpr.expression());
            // IN句内のサブクエリ処理
            var subquery = inExpr.selectStatement();
            if (subquery != null)
            {
                ProcessSubquery(subquery);
            }
        }
        // BETWEEN句の場合
        else if (expression is Grammar.SqlParser.BetweenExpressionContext between)
        {
            ProcessWhereExpression(between.expression(0));
            ProcessWhereExpression(between.expression(1));
            ProcessWhereExpression(between.expression(2));
        }
        // LIKE句の場合
        else if (expression is Grammar.SqlParser.LikeExpressionContext like)
        {
            ProcessWhereExpression(like.expression(0));
            ProcessWhereExpression(like.expression(1));
            if (like.expression().Length > 2)
            {
                ProcessWhereExpression(like.expression(2)); // ESCAPE clause
            }
        }
        // 算術式の場合 (+, -, *, /, %, ||)
        else if (expression is Grammar.SqlParser.ArithmeticExpressionContext arithmetic)
        {
            ProcessWhereExpression(arithmetic.expression(0));
            ProcessWhereExpression(arithmetic.expression(1));
        }
        // 括弧付き式の場合
        else if (expression is Grammar.SqlParser.ParenthesizedExpressionContext parenthesized)
        {
            ProcessWhereExpression(parenthesized.expression());
        }
        // サブクエリ式の場合
        else if (expression is Grammar.SqlParser.SubqueryExpressionContext subqueryExpr)
        {
            var subquery = subqueryExpr.selectStatement();
            if (subquery != null)
            {
                ProcessSubquery(subquery);
            }
        }
        // CASE式の場合
        else if (expression is Grammar.SqlParser.CaseExpressionContext caseExpr)
        {
            foreach (var whenClause in caseExpr.caseWhenClause())
            {
                ProcessWhereExpression(whenClause.expression(0)); // WHEN condition
                ProcessWhereExpression(whenClause.expression(1)); // THEN value
            }
            // ELSE clause
            if (caseExpr.expression() != null)
            {
                ProcessWhereExpression(caseExpr.expression());
            }
        }
        // EXISTS式の場合
        else if (expression is Grammar.SqlParser.ExistsExpressionContext exists)
        {
            var subquery = exists.selectStatement();
            if (subquery != null)
            {
                ProcessSubquery(subquery);
            }
        }
        // 関数呼び出しの場合
        else if (expression is Grammar.SqlParser.FunctionCallExpressionContext function)
        {
            var exprList = function.expressionList();
            if (exprList != null)
            {
                foreach (var expr in exprList.expression())
                {
                    ProcessWhereExpression(expr);
                }
            }
        }
    }

    public override object? VisitGroupByClause([NotNull] Grammar.SqlParser.GroupByClauseContext context)
    {
        foreach (var expression in context.expression())
        {
            ProcessGroupByExpression(expression);
        }
        return null;
    }

    private void ProcessGroupByExpression(Grammar.SqlParser.ExpressionContext expression)
    {
        if (expression is Grammar.SqlParser.ColumnReferenceExpressionContext colRef)
        {
            var columnName = colRef.identifier()?.GetText();
            if (!string.IsNullOrEmpty(columnName))
            {
                var col = new Column(columnName, "", "", ClauseKind.GroupBy);
                _query.ColumnGroupBy.Add(col);
            }
        }
        else if (expression is Grammar.SqlParser.QualifiedColumnExpressionContext qualifiedCol)
        {
            var identifiers = qualifiedCol.identifier();
            if (identifiers.Length == 2)
            {
                var tableName = identifiers[0].GetText();
                var columnName = identifiers[1].GetText();
                var col = new Column(columnName, tableName, "", ClauseKind.GroupBy);
                _query.ColumnGroupBy.Add(col);
            }
        }
        // 関数呼び出しの場合 (例: YEAR(date), SUBSTRING(name, 1, 10))
        else if (expression is Grammar.SqlParser.FunctionCallExpressionContext function)
        {
            var exprList = function.expressionList();
            if (exprList != null)
            {
                foreach (var expr in exprList.expression())
                {
                    ProcessGroupByExpression(expr);
                }
            }
        }
        // 算術式の場合
        else if (expression is Grammar.SqlParser.ArithmeticExpressionContext arithmetic)
        {
            ProcessGroupByExpression(arithmetic.expression(0));
            ProcessGroupByExpression(arithmetic.expression(1));
        }
        // 括弧付き式の場合
        else if (expression is Grammar.SqlParser.ParenthesizedExpressionContext parenthesized)
        {
            ProcessGroupByExpression(parenthesized.expression());
        }
    }

    public override object? VisitOrderByClause([NotNull] Grammar.SqlParser.OrderByClauseContext context)
    {
        foreach (var orderByItem in context.orderByItem())
        {
            var expression = orderByItem.expression();
            if (expression != null)
            {
                ProcessOrderByExpression(expression);
            }
        }
        return null;
    }

    private void ProcessOrderByExpression(Grammar.SqlParser.ExpressionContext expression)
    {
        if (expression is Grammar.SqlParser.ColumnReferenceExpressionContext colRef)
        {
            var columnName = colRef.identifier()?.GetText();
            if (!string.IsNullOrEmpty(columnName))
            {
                var col = new Column(columnName, "", "", ClauseKind.OrderBy);
                _query.ColumnOrderBy.Add(col);
            }
        }
        else if (expression is Grammar.SqlParser.QualifiedColumnExpressionContext qualifiedCol)
        {
            var identifiers = qualifiedCol.identifier();
            if (identifiers.Length == 2)
            {
                var tableName = identifiers[0].GetText();
                var columnName = identifiers[1].GetText();
                var col = new Column(columnName, tableName, "", ClauseKind.OrderBy);
                _query.ColumnOrderBy.Add(col);
            }
        }
        // 関数呼び出しの場合
        else if (expression is Grammar.SqlParser.FunctionCallExpressionContext function)
        {
            var exprList = function.expressionList();
            if (exprList != null)
            {
                foreach (var expr in exprList.expression())
                {
                    ProcessOrderByExpression(expr);
                }
            }
        }
        // 算術式の場合
        else if (expression is Grammar.SqlParser.ArithmeticExpressionContext arithmetic)
        {
            ProcessOrderByExpression(arithmetic.expression(0));
            ProcessOrderByExpression(arithmetic.expression(1));
        }
        // CASE式の場合
        else if (expression is Grammar.SqlParser.CaseExpressionContext caseExpr)
        {
            foreach (var whenClause in caseExpr.caseWhenClause())
            {
                ProcessOrderByExpression(whenClause.expression(0));
                ProcessOrderByExpression(whenClause.expression(1));
            }
            if (caseExpr.expression() != null)
            {
                ProcessOrderByExpression(caseExpr.expression());
            }
        }
        // 括弧付き式の場合
        else if (expression is Grammar.SqlParser.ParenthesizedExpressionContext parenthesized)
        {
            ProcessOrderByExpression(parenthesized.expression());
        }
    }

    public override object? VisitHavingClause([NotNull] Grammar.SqlParser.HavingClauseContext context)
    {
        var expression = context.expression();
        if (expression != null)
        {
            ProcessHavingExpression(expression);
        }
        return null;
    }

    private void ProcessHavingExpression(Grammar.SqlParser.ExpressionContext expression)
    {
        if (expression is Grammar.SqlParser.ColumnReferenceExpressionContext colRef)
        {
            var columnName = colRef.identifier()?.GetText();
            if (!string.IsNullOrEmpty(columnName))
            {
                var col = new Column(columnName, "", "", ClauseKind.Having);
                _query.ColumnHaving.Add(col);
            }
        }
        else if (expression is Grammar.SqlParser.QualifiedColumnExpressionContext qualifiedCol)
        {
            var identifiers = qualifiedCol.identifier();
            if (identifiers.Length == 2)
            {
                var tableName = identifiers[0].GetText();
                var columnName = identifiers[1].GetText();
                var col = new Column(columnName, tableName, "", ClauseKind.Having);
                _query.ColumnHaving.Add(col);
            }
        }
        // 比較式の場合
        else if (expression is Grammar.SqlParser.ComparisonExpressionContext comparison)
        {
            ProcessHavingExpression(comparison.expression(0));
            ProcessHavingExpression(comparison.expression(1));
        }
        // 論理式の場合
        else if (expression is Grammar.SqlParser.LogicalExpressionContext logical)
        {
            ProcessHavingExpression(logical.expression(0));
            ProcessHavingExpression(logical.expression(1));
        }
        // 関数呼び出しの場合 (例: COUNT(*) > 10, SUM(amount) > 1000)
        else if (expression is Grammar.SqlParser.FunctionCallExpressionContext function)
        {
            var exprList = function.expressionList();
            if (exprList != null)
            {
                foreach (var expr in exprList.expression())
                {
                    ProcessHavingExpression(expr);
                }
            }
        }
        // 算術式の場合
        else if (expression is Grammar.SqlParser.ArithmeticExpressionContext arithmetic)
        {
            ProcessHavingExpression(arithmetic.expression(0));
            ProcessHavingExpression(arithmetic.expression(1));
        }
        // 括弧付き式の場合
        else if (expression is Grammar.SqlParser.ParenthesizedExpressionContext parenthesized)
        {
            ProcessHavingExpression(parenthesized.expression());
        }
    }

    public override object? VisitWithClause([NotNull] Grammar.SqlParser.WithClauseContext context)
    {
        var cteList = context.cteList();
        if (cteList != null)
        {
            foreach (var cte in cteList.cte())
            {
                var cteName = cte.identifier()?.GetText();
                var selectStmt = cte.selectStatement();

                if (!string.IsNullOrEmpty(cteName) && selectStmt != null)
                {
                    var withQuery = new Query
                    {
                        QueryText = selectStmt.GetText(),
                        Parent = _query,
                        AltName = cteName
                    };

                    var withVisitor = new SqlVisitor(withQuery);
                    withVisitor.Visit(selectStmt);

                    _query.Withs[cteName] = withQuery;
                }
            }
        }
        return null;
    }

    public override object? VisitJoinClause([NotNull] Grammar.SqlParser.JoinClauseContext context)
    {
        // JOIN句内のテーブル参照を処理
        var tableRef = context.tableReference();
        if (tableRef != null)
        {
            ProcessTableReference(tableRef);
        }

        // ON句の条件式を処理
        var onExpression = context.expression();
        if (onExpression != null)
        {
            ProcessWhereExpression(onExpression);  // WHERE句と同様に処理
        }

        return null;
    }
}
