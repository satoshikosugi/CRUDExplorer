using System.Linq;
using CRUDExplorer.SqlParser.Analyzers;
using CRUDExplorer.Core.Models;
using Xunit;

namespace CRUDExplorer.Core.Tests;

/// <summary>
/// CRUD抽出テスト（テーブルレベル・カラムレベル）
/// Phase 7.1: テーブルCRUD 50ケース + カラムCRUD 50ケース
/// </summary>
public class CrudExtractionTests
{
    private readonly SqlAnalyzer _analyzer = new();

    #region Table-Level CRUD Tests (50 cases)

    [Fact]
    public void TableCRUD_01_SimpleSelect_ExtractsReadTable()
    {
        var sql = "SELECT * FROM users";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("SELECT", query.QueryKind);
        Assert.Contains("users", query.TableR.Keys);
        Assert.Empty(query.TableC);
        Assert.Empty(query.TableU);
        Assert.Empty(query.TableD);
    }

    [Fact]
    public void TableCRUD_02_SimpleInsert_ExtractsCreateTable()
    {
        var sql = "INSERT INTO users (id, name) VALUES (1, 'John')";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("INSERT", query.QueryKind);
        Assert.Contains("users", query.TableC.Keys);
        Assert.Empty(query.TableR);
        Assert.Empty(query.TableU);
        Assert.Empty(query.TableD);
    }

    [Fact]
    public void TableCRUD_03_SimpleUpdate_ExtractsUpdateTable()
    {
        var sql = "UPDATE users SET name = 'Jane' WHERE id = 1";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("UPDATE", query.QueryKind);
        Assert.Contains("users", query.TableU.Keys);
        Assert.Empty(query.TableC);
        Assert.Empty(query.TableR);
        Assert.Empty(query.TableD);
    }

    [Fact]
    public void TableCRUD_04_SimpleDelete_ExtractsDeleteTable()
    {
        var sql = "DELETE FROM users WHERE id = 1";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("DELETE", query.QueryKind);
        Assert.Contains("users", query.TableD.Keys);
        Assert.Empty(query.TableC);
        Assert.Empty(query.TableR);
        Assert.Empty(query.TableU);
    }

    [Fact]
    public void TableCRUD_05_SelectWithJoin_ExtractsMultipleReadTables()
    {
        var sql = "SELECT u.name, o.amount FROM users u JOIN orders o ON u.id = o.user_id";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("SELECT", query.QueryKind);
        Assert.Contains("users", query.TableR.Keys);
        Assert.Contains("orders", query.TableR.Keys);
        Assert.Equal(2, query.TableR.Count);
    }

    [Fact]
    public void TableCRUD_06_SelectWithLeftJoin_ExtractsMultipleReadTables()
    {
        var sql = "SELECT u.name, p.product FROM users u LEFT JOIN purchases p ON u.id = p.user_id";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("users", query.TableR.Keys);
        Assert.Contains("purchases", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_07_SelectWithRightJoin_ExtractsMultipleReadTables()
    {
        var sql = "SELECT * FROM orders o RIGHT JOIN customers c ON o.customer_id = c.id";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("orders", query.TableR.Keys);
        Assert.Contains("customers", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_08_SelectWithFullJoin_ExtractsMultipleReadTables()
    {
        var sql = "SELECT * FROM employees e FULL OUTER JOIN departments d ON e.dept_id = d.id";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("employees", query.TableR.Keys);
        Assert.Contains("departments", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_09_SelectWithCrossJoin_ExtractsMultipleReadTables()
    {
        var sql = "SELECT * FROM colors CROSS JOIN sizes";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("colors", query.TableR.Keys);
        Assert.Contains("sizes", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_10_SelectWithMultipleJoins_ExtractsAllReadTables()
    {
        var sql = @"SELECT u.name, o.amount, p.product_name
                    FROM users u
                    JOIN orders o ON u.id = o.user_id
                    JOIN products p ON o.product_id = p.id";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("users", query.TableR.Keys);
        Assert.Contains("orders", query.TableR.Keys);
        Assert.Contains("products", query.TableR.Keys);
        Assert.Equal(3, query.TableR.Count);
    }

    [Fact]
    public void TableCRUD_11_InsertSelect_ExtractsCreateAndReadTables()
    {
        var sql = "INSERT INTO backup_users (id, name) SELECT id, name FROM users";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("INSERT", query.QueryKind);
        Assert.Contains("backup_users", query.TableC.Keys);
        Assert.Contains("users", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_12_InsertSelectWithJoin_ExtractsMultipleTables()
    {
        var sql = @"INSERT INTO user_orders (user_id, order_count)
                    SELECT u.id, COUNT(o.id)
                    FROM users u
                    JOIN orders o ON u.id = o.user_id
                    GROUP BY u.id";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("user_orders", query.TableC.Keys);
        Assert.Contains("users", query.TableR.Keys);
        Assert.Contains("orders", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_13_SelectWithSubquery_ExtractsAllReadTables()
    {
        var sql = "SELECT name FROM users WHERE id IN (SELECT user_id FROM orders)";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("users", query.TableR.Keys);
        Assert.True(query.GetAllTableR().Values.Any(v => v.Contains("ORDERS", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void TableCRUD_14_SelectWithSubqueryInFrom_ExtractsAllReadTables()
    {
        var sql = "SELECT u.name FROM users u WHERE u.id IN (SELECT user_id FROM orders WHERE amount > 100)";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("users", query.TableR.Keys);
        Assert.True(query.GetAllTableR().Values.Any(v => v.Contains("ORDERS", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void TableCRUD_15_SelectWithExistsSubquery_ExtractsAllReadTables()
    {
        var sql = "SELECT name FROM users u WHERE EXISTS (SELECT 1 FROM orders o WHERE o.user_id = u.id)";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("users", query.TableR.Keys);
        Assert.True(query.GetAllTableR().Values.Any(v => v.Contains("ORDERS", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void TableCRUD_16_SelectWithUnion_ExtractsAllReadTables()
    {
        var sql = "SELECT name FROM active_users UNION SELECT name FROM inactive_users";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("active_users", query.TableR.Keys);
        Assert.Contains("inactive_users", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_17_SelectWithUnionAll_ExtractsAllReadTables()
    {
        var sql = "SELECT id, name FROM customers UNION ALL SELECT id, name FROM suppliers";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("customers", query.TableR.Keys);
        Assert.Contains("suppliers", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_18_SelectWithIntersect_ExtractsAllReadTables()
    {
        var sql = "SELECT email FROM users INTERSECT SELECT email FROM subscribers";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("users", query.TableR.Keys);
        Assert.Contains("subscribers", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_19_SelectWithMinus_ExtractsAllReadTables()
    {
        var sql = "SELECT id FROM all_products MINUS SELECT id FROM sold_products";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("all_products", query.TableR.Keys);
        Assert.Contains("sold_products", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_20_SelectWithGroupBy_ExtractsReadTable()
    {
        var sql = "SELECT department, COUNT(*) FROM employees GROUP BY department";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("employees", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_21_SelectWithHaving_ExtractsReadTable()
    {
        var sql = "SELECT category, SUM(amount) FROM sales GROUP BY category HAVING SUM(amount) > 1000";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("sales", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_22_SelectWithOrderBy_ExtractsReadTable()
    {
        var sql = "SELECT name, age FROM persons ORDER BY age DESC, name ASC";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("persons", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_23_UpdateWithSubquery_ExtractsUpdateAndReadTables()
    {
        var sql = "UPDATE users SET status = 'active' WHERE id IN (SELECT user_id FROM recent_logins)";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("users", query.TableU.Keys);
        Assert.True(query.GetAllTableR().Values.Any(v => v.Contains("RECENT_LOGINS", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void TableCRUD_24_DeleteWithSubquery_ExtractsDeleteAndReadTables()
    {
        var sql = "DELETE FROM orders WHERE user_id IN (SELECT id FROM deleted_users)";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("orders", query.TableD.Keys);
        Assert.True(query.GetAllTableR().Values.Any(v => v.Contains("DELETED_USERS", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void TableCRUD_25_SelectFromMultipleTablesCommaSeparated_ExtractsAllReadTables()
    {
        var sql = "SELECT * FROM users, orders WHERE users.id = orders.user_id";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("users", query.TableR.Keys);
        Assert.Contains("orders", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_26_SelectWithTableAlias_ExtractsReadTable()
    {
        var sql = "SELECT u.name, u.email FROM users AS u WHERE u.active = true";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("users", query.TableR.Keys);
        var tableValue = query.TableR["users"];
        Assert.Contains("u", tableValue);
    }

    [Fact]
    public void TableCRUD_27_SelectWithCTE_ExtractsReadTable()
    {
        var sql = @"WITH active_users AS (SELECT * FROM users WHERE active = true)
                    SELECT name FROM active_users";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("users", query.TableR.Keys);
        Assert.NotEmpty(query.Withs);
    }

    [Fact]
    public void TableCRUD_28_SelectWithMultipleCTE_ExtractsAllReadTables()
    {
        var sql = @"WITH
                    active_users AS (SELECT * FROM users WHERE active = true),
                    recent_orders AS (SELECT * FROM orders WHERE date > '2024-01-01')
                    SELECT u.name, o.amount
                    FROM active_users u
                    JOIN recent_orders o ON u.id = o.user_id";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("users", query.TableR.Keys);
        Assert.Contains("orders", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_29_InsertMultipleRows_ExtractsCreateTable()
    {
        var sql = "INSERT INTO categories (id, name) VALUES (1, 'Books'), (2, 'Music'), (3, 'Movies')";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("categories", query.TableC.Keys);
    }

    [Fact]
    public void TableCRUD_30_UpdateWithJoinInWhere_ExtractsUpdateTable()
    {
        var sql = "UPDATE users SET status = 'premium' WHERE id IN (SELECT user_id FROM subscriptions)";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("users", query.TableU.Keys);
        Assert.Contains("subscriptions", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_31_SelectWithSelfJoin_ExtractsReadTable()
    {
        var sql = "SELECT e1.name AS employee, e2.name AS manager FROM employees e1 LEFT JOIN employees e2 ON e1.manager_id = e2.id";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("employees", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_32_SelectWithNestedSubquery_ExtractsAllReadTables()
    {
        var sql = @"SELECT name FROM users
                    WHERE id IN (
                        SELECT user_id FROM orders
                        WHERE product_id IN (
                            SELECT id FROM products WHERE category = 'Electronics'
                        )
                    )";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("users", query.TableR.Keys);
        var allTableR = query.GetAllTableR();
        Assert.True(allTableR.Values.Any(v => v.Contains("ORDERS", StringComparison.OrdinalIgnoreCase)));
        Assert.True(allTableR.Values.Any(v => v.Contains("PRODUCTS", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void TableCRUD_33_SelectDistinct_ExtractsReadTable()
    {
        var sql = "SELECT DISTINCT country FROM customers";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("customers", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_34_SelectTop_ExtractsReadTable()
    {
        var sql = "SELECT TOP 10 name, salary FROM employees ORDER BY salary DESC";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("employees", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_35_SelectWithAggregateFunctions_ExtractsReadTable()
    {
        var sql = "SELECT COUNT(*), AVG(price), MIN(price), MAX(price), SUM(quantity) FROM products";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("products", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_36_SelectWithCase_ExtractsReadTable()
    {
        var sql = @"SELECT name,
                    CASE
                        WHEN age < 18 THEN 'Minor'
                        WHEN age >= 18 AND age < 65 THEN 'Adult'
                        ELSE 'Senior'
                    END AS age_group
                    FROM persons";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("persons", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_37_UpdateWithCase_ExtractsUpdateTable()
    {
        var sql = @"UPDATE products
                    SET discount = CASE
                        WHEN price > 1000 THEN 0.15
                        WHEN price > 500 THEN 0.10
                        ELSE 0.05
                    END";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("products", query.TableU.Keys);
    }

    [Fact]
    public void TableCRUD_38_DeleteWithJoin_ExtractsDeleteTable()
    {
        var sql = "DELETE FROM order_items WHERE order_id IN (SELECT id FROM orders WHERE status = 'cancelled')";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("order_items", query.TableD.Keys);
        Assert.True(query.GetAllTableR().Values.Any(v => v.Contains("ORDERS", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void TableCRUD_39_SelectWithBetween_ExtractsReadTable()
    {
        var sql = "SELECT * FROM sales WHERE sale_date BETWEEN '2024-01-01' AND '2024-12-31'";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("sales", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_40_SelectWithLike_ExtractsReadTable()
    {
        var sql = "SELECT name, email FROM users WHERE email LIKE '%@example.com'";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("users", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_41_SelectWithIn_ExtractsReadTable()
    {
        var sql = "SELECT * FROM products WHERE category IN ('Electronics', 'Books', 'Toys')";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("products", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_42_SelectWithIsNull_ExtractsReadTable()
    {
        var sql = "SELECT name FROM customers WHERE phone IS NULL";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("customers", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_43_SelectWithIsNotNull_ExtractsReadTable()
    {
        var sql = "SELECT name FROM employees WHERE manager_id IS NOT NULL";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("employees", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_44_UpdateWithArithmetic_ExtractsUpdateTable()
    {
        var sql = "UPDATE inventory SET quantity = quantity - 1 WHERE product_id = 100";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("inventory", query.TableU.Keys);
    }

    [Fact]
    public void TableCRUD_45_SelectWithStringFunctions_ExtractsReadTable()
    {
        var sql = "SELECT UPPER(name), LOWER(email), SUBSTRING(phone, 1, 3) FROM users";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("users", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_46_SelectWithDateFunctions_ExtractsReadTable()
    {
        var sql = "SELECT name, YEAR(birth_date), MONTH(birth_date), DAY(birth_date) FROM persons";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("persons", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_47_SelectWithCoalesce_ExtractsReadTable()
    {
        var sql = "SELECT name, COALESCE(phone, mobile, 'No contact') AS contact FROM customers";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("customers", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_48_SelectWithConcat_ExtractsReadTable()
    {
        var sql = "SELECT CONCAT(first_name, ' ', last_name) AS full_name FROM persons";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("persons", query.TableR.Keys);
    }

    [Fact]
    public void TableCRUD_49_InsertWithDefault_ExtractsCreateTable()
    {
        var sql = "INSERT INTO logs (event_type, created_at) VALUES ('login', DEFAULT)";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("logs", query.TableC.Keys);
    }

    [Fact]
    public void TableCRUD_50_DeleteAll_ExtractsDeleteTable()
    {
        var sql = "DELETE FROM temp_data";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains("temp_data", query.TableD.Keys);
    }

    #endregion

    #region Column-Level CRUD Tests (50 cases)

    [Fact]
    public void ColumnCRUD_01_SimpleSelect_ExtractsReadColumns()
    {
        var sql = "SELECT id, name, email FROM users";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal(3, query.ColumnSelect.Count);
        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "id");
        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "name");
        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "email");
    }

    [Fact]
    public void ColumnCRUD_02_SimpleInsert_ExtractsCreateColumns()
    {
        var sql = "INSERT INTO users (id, name, email) VALUES (1, 'John', 'john@example.com')";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal(3, query.ColumnInsert.Count);
        Assert.Contains(query.ColumnInsert, c => c.ColumnName == "id");
        Assert.Contains(query.ColumnInsert, c => c.ColumnName == "name");
        Assert.Contains(query.ColumnInsert, c => c.ColumnName == "email");
    }

    [Fact]
    public void ColumnCRUD_03_SimpleUpdate_ExtractsUpdateColumns()
    {
        var sql = "UPDATE users SET name = 'Jane', email = 'jane@example.com' WHERE id = 1";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal(2, query.ColumnUpdate.Count);
        Assert.Contains(query.ColumnUpdate, c => c.ColumnName == "name");
        Assert.Contains(query.ColumnUpdate, c => c.ColumnName == "email");
    }

    [Fact]
    public void ColumnCRUD_04_UpdateWithWhereColumn_ExtractsWhereColumns()
    {
        var sql = "UPDATE users SET status = 'active' WHERE id = 1 AND email = 'test@example.com'";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.NotEmpty(query.ColumnWhere);
        Assert.Contains(query.ColumnWhere, c => c.ColumnName == "id");
        Assert.Contains(query.ColumnWhere, c => c.ColumnName == "email");
    }

    [Fact]
    public void ColumnCRUD_05_SelectWithWhere_ExtractsWhereColumns()
    {
        var sql = "SELECT name FROM users WHERE id = 1 AND status = 'active'";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.NotEmpty(query.ColumnWhere);
        Assert.Contains(query.ColumnWhere, c => c.ColumnName == "id");
        Assert.Contains(query.ColumnWhere, c => c.ColumnName == "status");
    }

    [Fact]
    public void ColumnCRUD_06_SelectWithQualifiedColumns_ExtractsColumnsWithTableNames()
    {
        var sql = "SELECT u.id, u.name, o.amount FROM users u JOIN orders o ON u.id = o.user_id";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "id" && c.Table == "u");
        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "name" && c.Table == "u");
        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "amount" && c.Table == "o");
    }

    [Fact]
    public void ColumnCRUD_07_SelectWithJoinOn_ExtractsJoinColumns()
    {
        var sql = "SELECT * FROM users u JOIN orders o ON u.id = o.user_id";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.NotEmpty(query.ColumnWhere);
        Assert.Contains(query.ColumnWhere, c => c.ColumnName == "id");
        Assert.Contains(query.ColumnWhere, c => c.ColumnName == "user_id");
    }

    [Fact]
    public void ColumnCRUD_08_SelectWithGroupBy_ExtractsGroupByColumns()
    {
        var sql = "SELECT department, COUNT(*) FROM employees GROUP BY department";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.NotEmpty(query.ColumnGroupBy);
        Assert.Contains(query.ColumnGroupBy, c => c.ColumnName == "department");
    }

    [Fact]
    public void ColumnCRUD_09_SelectWithMultipleGroupBy_ExtractsAllGroupByColumns()
    {
        var sql = "SELECT department, year, COUNT(*) FROM employees GROUP BY department, year";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal(2, query.ColumnGroupBy.Count);
        Assert.Contains(query.ColumnGroupBy, c => c.ColumnName == "department");
        Assert.Contains(query.ColumnGroupBy, c => c.ColumnName == "year");
    }

    [Fact]
    public void ColumnCRUD_10_SelectWithHaving_ExtractsHavingColumns()
    {
        var sql = "SELECT department, COUNT(*) AS cnt FROM employees GROUP BY department HAVING COUNT(*) > 5";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.NotEmpty(query.ColumnHaving);
    }

    [Fact]
    public void ColumnCRUD_11_SelectWithOrderBy_ExtractsOrderByColumns()
    {
        var sql = "SELECT name, age FROM persons ORDER BY age DESC, name ASC";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal(2, query.ColumnOrderBy.Count);
        Assert.Contains(query.ColumnOrderBy, c => c.ColumnName == "age");
        Assert.Contains(query.ColumnOrderBy, c => c.ColumnName == "name");
    }

    [Fact]
    public void ColumnCRUD_12_InsertSelect_ExtractsInsertAndSelectColumns()
    {
        var sql = "INSERT INTO backup_users (id, name) SELECT id, name FROM users";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal(2, query.ColumnInsert.Count);
        Assert.NotEmpty(query.ColumnSelect);
    }

    [Fact]
    public void ColumnCRUD_13_UpdateWithArithmetic_ExtractsSetConditionColumns()
    {
        var sql = "UPDATE products SET price = price * 1.1 WHERE category = 'Electronics'";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnUpdate, c => c.ColumnName == "price");
        Assert.NotEmpty(query.ColumnSetCond);
        Assert.Contains(query.ColumnSetCond, c => c.ColumnName == "price");
    }

    [Fact]
    public void ColumnCRUD_14_SelectWithAlias_ExtractsColumnsWithAliases()
    {
        var sql = "SELECT id AS user_id, name AS user_name FROM users";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.NotEmpty(query.ColumnSelect);
        Assert.NotEmpty(query.Selects);
        Assert.Contains("user_id", query.Selects.Keys);
        Assert.Contains("user_name", query.Selects.Keys);
    }

    [Fact]
    public void ColumnCRUD_15_SelectWithAggregate_ExtractsColumns()
    {
        var sql = "SELECT COUNT(id), SUM(amount), AVG(price) FROM orders";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.NotEmpty(query.ColumnSelect);
        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "id");
        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "amount");
        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "price");
    }

    [Fact]
    public void ColumnCRUD_16_SelectWithCase_ExtractsColumnsFromCase()
    {
        var sql = @"SELECT name,
                    CASE
                        WHEN age < 18 THEN 'Minor'
                        WHEN age >= 18 THEN 'Adult'
                    END AS age_group
                    FROM persons";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "name");
        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "age");
    }

    [Fact]
    public void ColumnCRUD_17_UpdateWithCase_ExtractsColumnsFromCase()
    {
        var sql = @"UPDATE products
                    SET discount = CASE
                        WHEN price > 1000 THEN 0.15
                        ELSE 0.05
                    END";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnUpdate, c => c.ColumnName == "discount");
        Assert.Contains(query.ColumnSetCond, c => c.ColumnName == "price");
    }

    [Fact]
    public void ColumnCRUD_18_SelectWithSubqueryInWhere_ExtractsColumnsFromSubquery()
    {
        var sql = "SELECT name FROM users WHERE id IN (SELECT user_id FROM orders)";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "name");
        Assert.NotEmpty(query.ColumnWhere);
    }

    [Fact]
    public void ColumnCRUD_19_SelectWithBetween_ExtractsWhereColumns()
    {
        var sql = "SELECT * FROM sales WHERE sale_date BETWEEN '2024-01-01' AND '2024-12-31'";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnWhere, c => c.ColumnName == "sale_date");
    }

    [Fact]
    public void ColumnCRUD_20_SelectWithLike_ExtractsWhereColumns()
    {
        var sql = "SELECT name FROM users WHERE email LIKE '%@example.com'";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnWhere, c => c.ColumnName == "email");
    }

    [Fact]
    public void ColumnCRUD_21_SelectWithIn_ExtractsWhereColumns()
    {
        var sql = "SELECT * FROM products WHERE category IN ('Electronics', 'Books')";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnWhere, c => c.ColumnName == "category");
    }

    [Fact]
    public void ColumnCRUD_22_SelectWithIsNull_ExtractsWhereColumns()
    {
        var sql = "SELECT name FROM customers WHERE phone IS NULL";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnWhere, c => c.ColumnName == "phone");
    }

    [Fact]
    public void ColumnCRUD_23_SelectWithIsNotNull_ExtractsWhereColumns()
    {
        var sql = "SELECT name FROM employees WHERE manager_id IS NOT NULL";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnWhere, c => c.ColumnName == "manager_id");
    }

    [Fact]
    public void ColumnCRUD_24_SelectWithAnd_ExtractsMultipleWhereColumns()
    {
        var sql = "SELECT * FROM users WHERE status = 'active' AND age > 18 AND country = 'US'";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnWhere, c => c.ColumnName == "status");
        Assert.Contains(query.ColumnWhere, c => c.ColumnName == "age");
        Assert.Contains(query.ColumnWhere, c => c.ColumnName == "country");
    }

    [Fact]
    public void ColumnCRUD_25_SelectWithOr_ExtractsMultipleWhereColumns()
    {
        var sql = "SELECT * FROM products WHERE category = 'Electronics' OR price < 100";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnWhere, c => c.ColumnName == "category");
        Assert.Contains(query.ColumnWhere, c => c.ColumnName == "price");
    }

    [Fact]
    public void ColumnCRUD_26_SelectWithNot_ExtractsWhereColumns()
    {
        var sql = "SELECT * FROM users WHERE NOT status = 'deleted'";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnWhere, c => c.ColumnName == "status");
    }

    [Fact]
    public void ColumnCRUD_27_SelectWithArithmetic_ExtractsSelectColumns()
    {
        var sql = "SELECT price, quantity, price * quantity AS total FROM order_items";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "price");
        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "quantity");
    }

    [Fact]
    public void ColumnCRUD_28_SelectWithStringFunctions_ExtractsColumns()
    {
        var sql = "SELECT UPPER(name), LOWER(email) FROM users";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "name");
        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "email");
    }

    [Fact]
    public void ColumnCRUD_29_SelectWithConcat_ExtractsColumns()
    {
        var sql = "SELECT CONCAT(first_name, ' ', last_name) AS full_name FROM persons";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "first_name");
        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "last_name");
    }

    [Fact]
    public void ColumnCRUD_30_SelectWithCoalesce_ExtractsColumns()
    {
        var sql = "SELECT COALESCE(phone, mobile, 'No contact') AS contact FROM customers";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "phone");
        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "mobile");
    }

    [Fact]
    public void ColumnCRUD_31_SelectWithSubstringFunction_ExtractsColumns()
    {
        var sql = "SELECT SUBSTRING(email, 1, CHARINDEX('@', email) - 1) AS username FROM users";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "email");
    }

    [Fact]
    public void ColumnCRUD_32_SelectWithDateFunctions_ExtractsColumns()
    {
        var sql = "SELECT YEAR(birth_date), MONTH(birth_date), DAY(birth_date) FROM persons";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "birth_date");
    }

    [Fact]
    public void ColumnCRUD_33_UpdateWithFunction_ExtractsSetConditionColumns()
    {
        var sql = "UPDATE users SET email = LOWER(email) WHERE id = 1";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnUpdate, c => c.ColumnName == "email");
        Assert.Contains(query.ColumnSetCond, c => c.ColumnName == "email");
    }

    [Fact]
    public void ColumnCRUD_34_UpdateMultipleColumns_ExtractsAllUpdateColumns()
    {
        var sql = "UPDATE users SET name = 'John', email = 'john@example.com', status = 'active' WHERE id = 1";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal(3, query.ColumnUpdate.Count);
        Assert.Contains(query.ColumnUpdate, c => c.ColumnName == "name");
        Assert.Contains(query.ColumnUpdate, c => c.ColumnName == "email");
        Assert.Contains(query.ColumnUpdate, c => c.ColumnName == "status");
    }

    [Fact]
    public void ColumnCRUD_35_DeleteWithMultipleWhereColumns_ExtractsWhereColumns()
    {
        var sql = "DELETE FROM orders WHERE status = 'cancelled' AND created_at < '2024-01-01'";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnWhere, c => c.ColumnName == "status");
        Assert.Contains(query.ColumnWhere, c => c.ColumnName == "created_at");
    }

    [Fact]
    public void ColumnCRUD_36_SelectWithExists_ExtractsColumns()
    {
        var sql = "SELECT name FROM users u WHERE EXISTS (SELECT 1 FROM orders o WHERE o.user_id = u.id)";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "name");
        Assert.NotEmpty(query.ColumnWhere);
    }

    [Fact]
    public void ColumnCRUD_37_SelectWithNotExists_ExtractsColumns()
    {
        var sql = "SELECT name FROM users u WHERE NOT EXISTS (SELECT 1 FROM orders o WHERE o.user_id = u.id)";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "name");
    }

    [Fact]
    public void ColumnCRUD_38_SelectWithMultipleJoins_ExtractsAllJoinColumns()
    {
        var sql = @"SELECT u.name, o.order_id, p.product_name
                    FROM users u
                    JOIN orders o ON u.id = o.user_id
                    JOIN order_items oi ON o.id = oi.order_id
                    JOIN products p ON oi.product_id = p.id";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.NotEmpty(query.ColumnWhere);
        Assert.Contains(query.ColumnWhere, c => c.ColumnName == "id" || c.ColumnName == "user_id" || c.ColumnName == "order_id" || c.ColumnName == "product_id");
    }

    [Fact]
    public void ColumnCRUD_39_SelectWithLeftJoinAndWhere_ExtractsAllColumns()
    {
        var sql = "SELECT u.name, o.amount FROM users u LEFT JOIN orders o ON u.id = o.user_id WHERE u.status = 'active'";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "name");
        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "amount");
        Assert.Contains(query.ColumnWhere, c => c.ColumnName == "status");
    }

    [Fact]
    public void ColumnCRUD_40_SelectWithUnion_ExtractsColumnsFromBothQueries()
    {
        var sql = "SELECT id, name FROM active_users UNION SELECT id, name FROM inactive_users";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.NotEmpty(query.ColumnSelect);
        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "id");
        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "name");
    }

    [Fact]
    public void ColumnCRUD_41_SelectWithDistinct_ExtractsColumns()
    {
        var sql = "SELECT DISTINCT country, city FROM customers";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "country");
        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "city");
    }

    [Fact]
    public void ColumnCRUD_42_SelectWithGroupByHaving_ExtractsAllColumns()
    {
        var sql = "SELECT category, COUNT(*) AS cnt FROM products GROUP BY category HAVING COUNT(*) > 10";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnGroupBy, c => c.ColumnName == "category");
        Assert.NotEmpty(query.ColumnHaving);
    }

    [Fact]
    public void ColumnCRUD_43_UpdateWithSubqueryInSet_ExtractsColumns()
    {
        var sql = "UPDATE users SET status = (SELECT status FROM user_status WHERE user_id = users.id) WHERE id = 1";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnUpdate, c => c.ColumnName == "status");
    }

    [Fact]
    public void ColumnCRUD_44_InsertSelectWithJoin_ExtractsAllColumns()
    {
        var sql = @"INSERT INTO user_summary (user_id, total_orders)
                    SELECT u.id, COUNT(o.id)
                    FROM users u
                    JOIN orders o ON u.id = o.user_id
                    GROUP BY u.id";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal(2, query.ColumnInsert.Count);
        Assert.Contains(query.ColumnInsert, c => c.ColumnName == "user_id");
        Assert.Contains(query.ColumnInsert, c => c.ColumnName == "total_orders");
    }

    [Fact]
    public void ColumnCRUD_45_SelectWithComplexExpression_ExtractsAllColumns()
    {
        var sql = "SELECT price, tax, price + tax AS total, (price + tax) * quantity AS grand_total FROM invoices";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "price");
        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "tax");
        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "quantity");
    }

    [Fact]
    public void ColumnCRUD_46_UpdateWithComplexWhere_ExtractsWhereColumns()
    {
        var sql = "UPDATE products SET price = price * 1.1 WHERE (category = 'A' OR category = 'B') AND stock > 0";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnWhere, c => c.ColumnName == "category");
        Assert.Contains(query.ColumnWhere, c => c.ColumnName == "stock");
    }

    [Fact]
    public void ColumnCRUD_47_SelectWithCTE_ExtractsColumnsFromCTE()
    {
        var sql = @"WITH high_value_customers AS (
                        SELECT user_id, SUM(amount) AS total
                        FROM orders
                        GROUP BY user_id
                        HAVING SUM(amount) > 1000
                    )
                    SELECT u.name, hvc.total
                    FROM users u
                    JOIN high_value_customers hvc ON u.id = hvc.user_id";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.NotEmpty(query.ColumnSelect);
        Assert.NotEmpty(query.ColumnGroupBy);
    }

    [Fact]
    public void ColumnCRUD_48_SelectWithWindowFunction_ExtractsColumns()
    {
        var sql = "SELECT name, salary, RANK() OVER (ORDER BY salary DESC) AS salary_rank FROM employees";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "name");
        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "salary");
    }

    [Fact]
    public void ColumnCRUD_49_SelectWithNestedCase_ExtractsAllColumns()
    {
        var sql = @"SELECT name,
                    CASE
                        WHEN status = 'active' THEN
                            CASE
                                WHEN age < 18 THEN 'Active Minor'
                                ELSE 'Active Adult'
                            END
                        ELSE 'Inactive'
                    END AS user_status
                    FROM users";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "name");
        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "status");
        Assert.Contains(query.ColumnSelect, c => c.ColumnName == "age");
    }

    [Fact]
    public void ColumnCRUD_50_ComplexQueryWithAllClauses_ExtractsAllColumns()
    {
        var sql = @"SELECT u.name, u.email, COUNT(o.id) AS order_count, SUM(o.amount) AS total_amount
                    FROM users u
                    LEFT JOIN orders o ON u.id = o.user_id
                    WHERE u.status = 'active' AND u.created_at > '2024-01-01'
                    GROUP BY u.id, u.name, u.email
                    HAVING COUNT(o.id) > 0
                    ORDER BY total_amount DESC";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.NotEmpty(query.ColumnSelect);
        Assert.NotEmpty(query.ColumnWhere);
        Assert.NotEmpty(query.ColumnGroupBy);
        Assert.NotEmpty(query.ColumnHaving);
        Assert.NotEmpty(query.ColumnOrderBy);
    }

    #endregion
}
