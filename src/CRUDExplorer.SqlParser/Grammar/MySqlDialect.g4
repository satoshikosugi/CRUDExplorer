grammar MySqlDialect;

import Sql;

// MySQL-specific extensions to the base SQL grammar

// Override SELECT statement to support MySQL-specific features
selectStatement
    : withClause? selectClause fromClause? whereClause? groupByClause? havingClause? orderByClause? limitClause? (UNION | MINUS | INTERSECT) ALL? selectStatement? (FOR UPDATE | LOCK IN SHARE MODE)?
    ;

// MySQL-specific INSERT with ON DUPLICATE KEY UPDATE
insertStatement
    : (INSERT | REPLACE) (LOW_PRIORITY | DELAYED | HIGH_PRIORITY)? IGNORE? INTO? tableName ('(' columnList ')')? (VALUES valuesList | selectStatement) (ON DUPLICATE KEY UPDATE setClauseList)?
    ;

// MySQL-specific UPDATE with LIMIT
updateStatement
    : UPDATE (LOW_PRIORITY)? IGNORE? tableName SET setClauseList whereClause? orderByClause? limitClause?
    ;

// MySQL-specific DELETE with LIMIT
deleteStatement
    : DELETE (LOW_PRIORITY)? QUICK? IGNORE? FROM tableName whereClause? orderByClause? limitClause?
    ;

// MySQL-specific clauses
limitClause
    : LIMIT (expression (',' expression)? | expression OFFSET expression)
    ;

// MySQL-specific expressions
expression
    : literal                                           # LiteralExpression
    | identifier                                        # ColumnReferenceExpression
    | identifier '.' identifier                         # QualifiedColumnExpression
    | identifier '.' '*'                                # TableWildcardExpression
    | functionName '(' (DISTINCT? expressionList | '*')? ')'  # FunctionCallExpression
    | expression comparisonOperator expression          # ComparisonExpression
    | expression (AND | OR | XOR) expression            # LogicalExpression
    | NOT expression                                    # NotExpression
    | expression IS NOT? NULL                           # IsNullExpression
    | expression NOT? IN '(' (expressionList | selectStatement) ')'  # InExpression
    | expression NOT? BETWEEN expression AND expression # BetweenExpression
    | expression NOT? LIKE expression (ESCAPE expression)?  # LikeExpression
    | expression NOT? REGEXP expression                 # RegexpExpression
    | expression ('+'|'-'|'*'|'/'|'%'|'DIV'|'MOD'|'||') expression  # ArithmeticExpression
    | expression ('<<'|'>>'|'&'|'|'|'^') expression     # BitwiseExpression
    | '(' expression ')'                                # ParenthesizedExpression
    | '(' selectStatement ')'                           # SubqueryExpression
    | CASE caseWhenClause+ (ELSE expression)? END       # CaseExpression
    | EXISTS '(' selectStatement ')'                    # ExistsExpression
    | BINARY expression                                 # BinaryExpression
    | CAST '(' expression AS mysqlType ')'              # CastExpression
    | CONVERT '(' expression ',' mysqlType ')'          # ConvertExpression
    | CONVERT '(' expression USING identifier ')'       # ConvertUsingExpression
    ;

// MySQL-specific data types
mysqlType
    : TINYINT ('(' NUMBER_LITERAL ')')? (UNSIGNED | SIGNED)? (ZEROFILL)?
    | SMALLINT ('(' NUMBER_LITERAL ')')? (UNSIGNED | SIGNED)? (ZEROFILL)?
    | MEDIUMINT ('(' NUMBER_LITERAL ')')? (UNSIGNED | SIGNED)? (ZEROFILL)?
    | INT ('(' NUMBER_LITERAL ')')? (UNSIGNED | SIGNED)? (ZEROFILL)?
    | INTEGER ('(' NUMBER_LITERAL ')')? (UNSIGNED | SIGNED)? (ZEROFILL)?
    | BIGINT ('(' NUMBER_LITERAL ')')? (UNSIGNED | SIGNED)? (ZEROFILL)?
    | DECIMAL ('(' NUMBER_LITERAL (',' NUMBER_LITERAL)? ')')? (UNSIGNED | SIGNED)? (ZEROFILL)?
    | NUMERIC ('(' NUMBER_LITERAL (',' NUMBER_LITERAL)? ')')? (UNSIGNED | SIGNED)? (ZEROFILL)?
    | FLOAT ('(' NUMBER_LITERAL (',' NUMBER_LITERAL)? ')')? (UNSIGNED | SIGNED)? (ZEROFILL)?
    | DOUBLE (PRECISION)? ('(' NUMBER_LITERAL ',' NUMBER_LITERAL ')')? (UNSIGNED | SIGNED)? (ZEROFILL)?
    | REAL ('(' NUMBER_LITERAL ',' NUMBER_LITERAL ')')? (UNSIGNED | SIGNED)? (ZEROFILL)?
    | BIT ('(' NUMBER_LITERAL ')')?
    | BOOLEAN | BOOL
    | CHAR ('(' NUMBER_LITERAL ')')? (BINARY)? (CHARACTER SET identifier)? (COLLATE identifier)?
    | VARCHAR ('(' NUMBER_LITERAL ')')? (BINARY)? (CHARACTER SET identifier)? (COLLATE identifier)?
    | BINARY ('(' NUMBER_LITERAL ')')?
    | VARBINARY ('(' NUMBER_LITERAL ')')?
    | TINYBLOB | BLOB | MEDIUMBLOB | LONGBLOB
    | TINYTEXT | TEXT | MEDIUMTEXT | LONGTEXT
    | ENUM '(' STRING_LITERAL (',' STRING_LITERAL)* ')'
    | SET '(' STRING_LITERAL (',' STRING_LITERAL)* ')'
    | DATE | DATETIME ('(' NUMBER_LITERAL ')')?
    | TIMESTAMP ('(' NUMBER_LITERAL ')')?
    | TIME ('(' NUMBER_LITERAL ')')?
    | YEAR ('(' NUMBER_LITERAL ')')?
    | JSON
    | GEOMETRY | POINT | LINESTRING | POLYGON
    | MULTIPOINT | MULTILINESTRING | MULTIPOLYGON | GEOMETRYCOLLECTION
    | identifier  // Custom types
    ;

// MySQL-specific keywords
REPLACE         : R E P L A C E ;
LOW_PRIORITY    : L O W '_' P R I O R I T Y ;
DELAYED         : D E L A Y E D ;
HIGH_PRIORITY   : H I G H '_' P R I O R I T Y ;
IGNORE          : I G N O R E ;
DUPLICATE       : D U P L I C A T E ;
KEY             : K E Y ;
LIMIT           : L I M I T ;
OFFSET          : O F F S E T ;
REGEXP          : R E G E X P ;
RLIKE           : R L I K E ;
DIV             : D I V ;
MOD             : M O D ;
XOR             : X O R ;
BINARY          : B I N A R Y ;
CAST            : C A S T ;
CONVERT         : C O N V E R T ;
USING           : U S I N G ;
FOR             : F O R ;
LOCK            : L O C K ;
SHARE           : S H A R E ;
MODE            : M O D E ;
QUICK           : Q U I C K ;

// MySQL data type keywords
TINYINT         : T I N Y I N T ;
SMALLINT        : S M A L L I N T ;
MEDIUMINT       : M E D I U M I N T ;
INT             : I N T ;
INTEGER         : I N T E G E R ;
BIGINT          : B I G I N T ;
DECIMAL         : D E C I M A L ;
NUMERIC         : N U M E R I C ;
FLOAT           : F L O A T ;
DOUBLE          : D O U B L E ;
REAL            : R E A L ;
PRECISION       : P R E C I S I O N ;
BIT             : B I T ;
BOOLEAN         : B O O L E A N ;
BOOL            : B O O L ;
CHAR            : C H A R ;
VARCHAR         : V A R C H A R ;
VARBINARY       : V A R B I N A R Y ;
TINYBLOB        : T I N Y B L O B ;
BLOB            : B L O B ;
MEDIUMBLOB      : M E D I U M B L O B ;
LONGBLOB        : L O N G B L O B ;
TINYTEXT        : T I N Y T E X T ;
TEXT            : T E X T ;
MEDIUMTEXT      : M E D I U M T E X T ;
LONGTEXT        : L O N G T E X T ;
ENUM            : E N U M ;
DATE            : D A T E ;
DATETIME        : D A T E T I M E ;
TIMESTAMP       : T I M E S T A M P ;
TIME            : T I M E ;
YEAR            : Y E A R ;
JSON            : J S O N ;
GEOMETRY        : G E O M E T R Y ;
POINT           : P O I N T ;
LINESTRING      : L I N E S T R I N G ;
POLYGON         : P O L Y G O N ;
MULTIPOINT      : M U L T I P O I N T ;
MULTILINESTRING : M U L T I L I N E S T R I N G ;
MULTIPOLYGON    : M U L T I P O L Y G O N ;
GEOMETRYCOLLECTION : G E O M E T R Y C O L L E C T I O N ;
UNSIGNED        : U N S I G N E D ;
SIGNED          : S I G N E D ;
ZEROFILL        : Z E R O F I L L ;
CHARACTER       : C H A R A C T E R ;
COLLATE         : C O L L A T E ;
