grammar SqlServerDialect;

import Sql;

// SQL Server-specific extensions to the base SQL grammar

// Override SELECT statement to support SQL Server-specific features
selectStatement
    : withClause? selectClause intoClause? fromClause? whereClause? groupByClause? havingClause? orderByClause? offsetFetchClause? forClause? optionClause? (UNION | MINUS | INTERSECT) ALL? selectStatement?
    ;

// SQL Server-specific SELECT clause with TOP
selectClause
    : SELECT DISTINCT? topClause? selectList
    ;

topClause
    : TOP ('(' expression ')' | NUMBER_LITERAL) PERCENT? (WITH TIES)?
    ;

// SQL Server-specific INSERT with OUTPUT clause
insertStatement
    : INSERT (TOP ('(' expression ')' | NUMBER_LITERAL) PERCENT?)? INTO tableName ('(' columnList ')')? outputClause? (VALUES valuesList | selectStatement)
    ;

// SQL Server-specific UPDATE with OUTPUT and FROM
updateStatement
    : UPDATE (TOP ('(' expression ')' | NUMBER_LITERAL) PERCENT?)? tableName SET setClauseList outputClause? fromClause? whereClause?
    ;

// SQL Server-specific DELETE with OUTPUT
deleteStatement
    : DELETE (TOP ('(' expression ')' | NUMBER_LITERAL) PERCENT?)? FROM? tableName outputClause? fromClause? whereClause?
    ;

// SQL Server-specific clauses
intoClause
    : INTO tableName
    ;

outputClause
    : OUTPUT selectList (INTO tableName ('(' columnList ')')?)?
    ;

offsetFetchClause
    : OFFSET expression (ROW | ROWS) (FETCH (FIRST | NEXT) expression (ROW | ROWS) ONLY)?
    ;

forClause
    : FOR (BROWSE | XML (RAW | AUTO | EXPLICIT | PATH) | JSON (AUTO | PATH))
    ;

optionClause
    : OPTION '(' queryHint (',' queryHint)* ')'
    ;

queryHint
    : (HASH | ORDER | CONCAT | MERGE | LOOP) JOIN
    | FAST NUMBER_LITERAL
    | FORCE ORDER
    | MAXDOP NUMBER_LITERAL
    | OPTIMIZE FOR '(' identifier '=' literal (',' identifier '=' literal)* ')'
    | RECOMPILE
    | ROBUST PLAN
    | identifier  // Other hints
    ;

// SQL Server-specific expressions
expression
    : literal                                           # LiteralExpression
    | identifier                                        # ColumnReferenceExpression
    | identifier '.' identifier                         # QualifiedColumnExpression
    | identifier '.' identifier '.' identifier          # FullyQualifiedColumnExpression
    | identifier '.' '*'                                # TableWildcardExpression
    | functionName '(' (DISTINCT? expressionList | '*')? ')' (OVER overClause)?  # FunctionCallExpression
    | expression comparisonOperator expression          # ComparisonExpression
    | expression (AND | OR) expression                  # LogicalExpression
    | NOT expression                                    # NotExpression
    | expression IS NOT? NULL                           # IsNullExpression
    | expression NOT? IN '(' (expressionList | selectStatement) ')'  # InExpression
    | expression NOT? BETWEEN expression AND expression # BetweenExpression
    | expression NOT? LIKE expression (ESCAPE expression)?  # LikeExpression
    | expression ('+'|'-'|'*'|'/'|'%') expression       # ArithmeticExpression
    | expression ('&'|'|'|'^') expression               # BitwiseExpression
    | '(' expression ')'                                # ParenthesizedExpression
    | '(' selectStatement ')'                           # SubqueryExpression
    | CASE caseWhenClause+ (ELSE expression)? END       # CaseExpression
    | EXISTS '(' selectStatement ')'                    # ExistsExpression
    | CAST '(' expression AS sqlServerType ')'          # CastExpression
    | CONVERT '(' sqlServerType ',' expression (',' NUMBER_LITERAL)? ')'  # ConvertExpression
    | COALESCE '(' expressionList ')'                   # CoalesceExpression
    | ISNULL '(' expression ',' expression ')'          # IsNullFunctionExpression
    | NULLIF '(' expression ',' expression ')'          # NullIfExpression
    ;

overClause
    : '(' (PARTITION BY expressionList)? (ORDER BY orderByItem (',' orderByItem)*)? (rowsRangeClause)? ')'
    ;

rowsRangeClause
    : (ROWS | RANGE) (BETWEEN frameBound AND frameBound | frameBound)
    ;

frameBound
    : UNBOUNDED (PRECEDING | FOLLOWING)
    | CURRENT ROW
    | expression (PRECEDING | FOLLOWING)
    ;

// SQL Server-specific data types
sqlServerType
    : BIT
    | TINYINT | SMALLINT | INT | BIGINT
    | DECIMAL ('(' NUMBER_LITERAL (',' NUMBER_LITERAL)? ')')?
    | NUMERIC ('(' NUMBER_LITERAL (',' NUMBER_LITERAL)? ')')?
    | MONEY | SMALLMONEY
    | FLOAT ('(' NUMBER_LITERAL ')')?
    | REAL
    | DATE | TIME ('(' NUMBER_LITERAL ')')?
    | DATETIME | DATETIME2 ('(' NUMBER_LITERAL ')')?
    | SMALLDATETIME
    | DATETIMEOFFSET ('(' NUMBER_LITERAL ')')?
    | CHAR ('(' NUMBER_LITERAL ')')?
    | VARCHAR ('(' (NUMBER_LITERAL | MAX) ')')?
    | TEXT
    | NCHAR ('(' NUMBER_LITERAL ')')?
    | NVARCHAR ('(' (NUMBER_LITERAL | MAX) ')')?
    | NTEXT
    | BINARY ('(' NUMBER_LITERAL ')')?
    | VARBINARY ('(' (NUMBER_LITERAL | MAX) ')')?
    | IMAGE
    | UNIQUEIDENTIFIER
    | XML
    | SQL_VARIANT
    | HIERARCHYID
    | GEOMETRY
    | GEOGRAPHY
    | identifier  // Custom types
    ;

// SQL Server-specific keywords
TOP             : T O P ;
PERCENT         : P E R C E N T ;
TIES            : T I E S ;
OUTPUT          : O U T P U T ;
INSERTED        : I N S E R T E D ;
DELETED         : D E L E T E D ;
OFFSET          : O F F S E T ;
FETCH           : F E T C H ;
NEXT            : N E X T ;
ONLY            : O N L Y ;
FOR             : F O R ;
BROWSE          : B R O W S E ;
XML             : X M L ;
JSON            : J S O N ;
RAW             : R A W ;
AUTO            : A U T O ;
EXPLICIT        : E X P L I C I T ;
PATH            : P A T H ;
OPTION          : O P T I O N ;
HASH            : H A S H ;
MERGE           : M E R G E ;
LOOP            : L O O P ;
CONCAT          : C O N C A T ;
FAST            : F A S T ;
FORCE           : F O R C E ;
MAXDOP          : M A X D O P ;
OPTIMIZE        : O P T I M I Z E ;
RECOMPILE       : R E C O M P I L E ;
ROBUST          : R O B U S T ;
PLAN            : P L A N ;
OVER            : O V E R ;
PARTITION       : P A R T I T I O N ;
ROWS            : R O W S ;
ROW             : R O W ;
RANGE           : R A N G E ;
UNBOUNDED       : U N B O U N D E D ;
PRECEDING       : P R E C E D I N G ;
FOLLOWING       : F O L L O W I N G ;
CURRENT         : C U R R E N T ;
CAST            : C A S T ;
CONVERT         : C O N V E R T ;
COALESCE        : C O A L E S C E ;
ISNULL          : I S N U L L ;
NULLIF          : N U L L I F ;

// SQL Server data type keywords
BIT             : B I T ;
TINYINT         : T I N Y I N T ;
SMALLINT        : S M A L L I N T ;
INT             : I N T ;
BIGINT          : B I G I N T ;
DECIMAL         : D E C I M A L ;
NUMERIC         : N U M E R I C ;
MONEY           : M O N E Y ;
SMALLMONEY      : S M A L L M O N E Y ;
FLOAT           : F L O A T ;
REAL            : R E A L ;
DATE            : D A T E ;
TIME            : T I M E ;
DATETIME        : D A T E T I M E ;
DATETIME2       : D A T E T I M E '2' ;
SMALLDATETIME   : S M A L L D A T E T I M E ;
DATETIMEOFFSET  : D A T E T I M E O F F S E T ;
CHAR            : C H A R ;
VARCHAR         : V A R C H A R ;
TEXT            : T E X T ;
NCHAR           : N C H A R ;
NVARCHAR        : N V A R C H A R ;
NTEXT           : N T E X T ;
BINARY          : B I N A R Y ;
VARBINARY       : V A R B I N A R Y ;
IMAGE           : I M A G E ;
UNIQUEIDENTIFIER: U N I Q U E I D E N T I F I E R ;
SQL_VARIANT     : S Q L '_' V A R I A N T ;
HIERARCHYID     : H I E R A R C H Y I D ;
GEOMETRY        : G E O M E T R Y ;
GEOGRAPHY       : G E O G R A P H Y ;
MAX             : M A X ;
