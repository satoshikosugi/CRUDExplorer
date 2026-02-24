grammar PostgreSqlDialect;

import Sql;

// PostgreSQL-specific extensions to the base SQL grammar

// Override SELECT statement to support PostgreSQL-specific features
selectStatement
    : withClause? selectClause intoClause? fromClause? whereClause? groupByClause? havingClause? orderByClause? limitClause? offsetClause? (UNION | MINUS | INTERSECT) ALL? selectStatement?
    ;

// PostgreSQL-specific INSERT with RETURNING clause
insertStatement
    : INSERT INTO tableName ('(' columnList ')')? (VALUES valuesList | selectStatement) (ON CONFLICT onConflictClause)? returningClause?
    ;

// PostgreSQL-specific UPDATE with FROM and RETURNING
updateStatement
    : UPDATE tableName SET setClauseList fromClause? whereClause? returningClause?
    ;

// PostgreSQL-specific DELETE with USING and RETURNING
deleteStatement
    : DELETE FROM tableName (USING tableReference (',' tableReference)*)? whereClause? returningClause?
    ;

// PostgreSQL-specific clauses
intoClause
    : INTO tableName
    ;

limitClause
    : LIMIT (expression | ALL)
    ;

offsetClause
    : OFFSET expression (ROW | ROWS)?
    ;

returningClause
    : RETURNING selectList
    ;

onConflictClause
    : '(' columnList ')' DO (NOTHING | UPDATE SET setClauseList whereClause?)
    ;

// PostgreSQL-specific expressions
expression
    : literal                                           # LiteralExpression
    | identifier                                        # ColumnReferenceExpression
    | identifier '.' identifier                         # QualifiedColumnExpression
    | identifier '.' '*'                                # TableWildcardExpression
    | functionName '(' (DISTINCT? expressionList | '*')? ')'  # FunctionCallExpression
    | expression comparisonOperator expression          # ComparisonExpression
    | expression (AND | OR) expression                  # LogicalExpression
    | NOT expression                                    # NotExpression
    | expression IS NOT? NULL                           # IsNullExpression
    | expression NOT? IN '(' (expressionList | selectStatement) ')'  # InExpression
    | expression NOT? BETWEEN expression AND expression # BetweenExpression
    | expression NOT? LIKE expression (ESCAPE expression)?  # LikeExpression
    | expression NOT? ILIKE expression (ESCAPE expression)?  # ILikeExpression
    | expression ('+'|'-'|'*'|'/'|'%'|'||') expression  # ArithmeticExpression
    | expression '->' expression                        # JsonAccessExpression
    | expression '->>' expression                       # JsonTextAccessExpression
    | expression '#>' expression                        # JsonPathAccessExpression
    | expression '#>>' expression                       # JsonPathTextAccessExpression
    | expression '@>' expression                        # JsonContainsExpression
    | expression '<@' expression                        # JsonContainedByExpression
    | expression '?' expression                         # JsonExistsExpression
    | expression '?|' expression                        # JsonExistsAnyExpression
    | expression '?&' expression                        # JsonExistsAllExpression
    | expression '::' postgresType                      # TypeCastExpression
    | CAST '(' expression AS postgresType ')'           # CastExpression
    | '(' expression ')'                                # ParenthesizedExpression
    | '(' selectStatement ')'                           # SubqueryExpression
    | CASE caseWhenClause+ (ELSE expression)? END       # CaseExpression
    | EXISTS '(' selectStatement ')'                    # ExistsExpression
    | ARRAY '[' expressionList? ']'                     # ArrayLiteralExpression
    | ROW '(' expressionList ')'                        # RowExpression
    | expression arraySubscript                         # ArraySubscriptExpression
    ;

arraySubscript
    : '[' expression (':' expression)? ']'
    ;

// PostgreSQL-specific data types
postgresType
    : SMALLINT | INTEGER | INT | BIGINT
    | DECIMAL ('(' NUMBER_LITERAL (',' NUMBER_LITERAL)? ')')?
    | NUMERIC ('(' NUMBER_LITERAL (',' NUMBER_LITERAL)? ')')?
    | REAL | DOUBLE PRECISION
    | SERIAL | BIGSERIAL | SMALLSERIAL
    | MONEY
    | CHAR ('(' NUMBER_LITERAL ')')?
    | VARCHAR ('(' NUMBER_LITERAL ')')?
    | TEXT
    | BYTEA
    | TIMESTAMP (WITH | WITHOUT)? TIME? ZONE?
    | DATE
    | TIME (WITH | WITHOUT)? TIME? ZONE?
    | INTERVAL
    | BOOLEAN | BOOL
    | UUID
    | JSON | JSONB
    | XML
    | ARRAY
    | HSTORE
    | POINT | LINE | LSEG | BOX | PATH | POLYGON | CIRCLE
    | CIDR | INET | MACADDR
    | BIT ('(' NUMBER_LITERAL ')')?
    | VARBIT ('(' NUMBER_LITERAL ')')?
    | identifier  // Custom types
    ;

// PostgreSQL-specific keywords
RETURNING   : R E T U R N I N G ;
CONFLICT    : C O N F L I C T ;
NOTHING     : N O T H I N G ;
LIMIT       : L I M I T ;
OFFSET      : O F F S E T ;
ILIKE       : I L I K E ;
CAST        : C A S T ;
ARRAY       : A R R A Y ;
ROW         : R O W ;
ROWS        : R O W S ;

// PostgreSQL data type keywords
SMALLINT    : S M A L L I N T ;
INTEGER     : I N T E G E R ;
INT         : I N T ;
BIGINT      : B I G I N T ;
DECIMAL     : D E C I M A L ;
NUMERIC     : N U M E R I C ;
REAL        : R E A L ;
DOUBLE      : D O U B L E ;
PRECISION   : P R E C I S I O N ;
SERIAL      : S E R I A L ;
BIGSERIAL   : B I G S E R I A L ;
SMALLSERIAL : S M A L L S E R I A L ;
MONEY       : M O N E Y ;
CHAR        : C H A R ;
VARCHAR     : V A R C H A R ;
TEXT        : T E X T ;
BYTEA       : B Y T E A ;
TIMESTAMP   : T I M E S T A M P ;
DATE        : D A T E ;
TIME        : T I M E ;
ZONE        : Z O N E ;
INTERVAL    : I N T E R V A L ;
BOOLEAN     : B O O L E A N ;
BOOL        : B O O L ;
UUID        : U U I D ;
JSON        : J S O N ;
JSONB       : J S O N B ;
XML         : X M L ;
HSTORE      : H S T O R E ;
POINT       : P O I N T ;
LINE        : L I N E ;
LSEG        : L S E G ;
BOX         : B O X ;
PATH        : P A T H ;
POLYGON     : P O L Y G O N ;
CIRCLE      : C I R C L E ;
CIDR        : C I D R ;
INET        : I N E T ;
MACADDR     : M A C A D D R ;
BIT         : B I T ;
VARBIT      : V A R B I T ;
DO          : D O ;
USING       : U S I N G ;
WITHOUT     : W I T H O U T ;
