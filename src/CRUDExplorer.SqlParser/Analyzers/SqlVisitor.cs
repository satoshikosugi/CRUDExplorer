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
        if (context.UNION() != null)
        {
            _query.QueryKind = "UNION";
        }
        else if (context.MINUS() != null)
        {
            _query.QueryKind = "MINUS";
        }
        else if (context.INTERSECT() != null)
        {
            _query.QueryKind = "INTERSECT";
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
                    var col = new Column(columnName, tableName ?? "", "", ClauseKind.Update);
                    _query.ColumnUpdate.Add(col);
                    _query.ColumnU[$"{tableName}.{columnName}"] = col;
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
    }

    private void ProcessTableReference(Grammar.SqlParser.TableReferenceContext tableRef)
    {
        // テーブル名の取得
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

        // サブクエリの処理
        var subquery = tableRef.selectStatement();
        if (subquery != null)
        {
            ProcessSubquery(subquery);
        }

        // JOIN句の処理
        if (tableRef.joinClause() != null)
        {
            VisitJoinClause(tableRef.joinClause());
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
