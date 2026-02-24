grammar OracleDialect;

import Sql;

// Oracle-specific extensions to the base SQL grammar

// Override SELECT statement to support Oracle-specific features
selectStatement
    : withClause? selectClause fromClause? whereClause? connectByClause? groupByClause? havingClause? modelClause? orderByClause? forUpdateClause? (UNION | MINUS | INTERSECT) ALL? selectStatement?
    ;

// Oracle-specific INSERT with RETURNING clause
insertStatement
    : INSERT INTO tableName ('(' columnList ')')? (VALUES valuesList | selectStatement) returningClause?
    ;

// Oracle-specific UPDATE with RETURNING
updateStatement
    : UPDATE tableName SET setClauseList whereClause? returningClause?
    ;

// Oracle-specific DELETE with RETURNING
deleteStatement
    : DELETE FROM? tableName whereClause? returningClause?
    ;

// Oracle-specific clauses
connectByClause
    : START WITH expression CONNECT BY NOCYCLE? expression
    | CONNECT BY NOCYCLE? expression (START WITH expression)?
    ;

modelClause
    : MODEL (cellReferenceOptions)? (returnRowsClause)? (referenceModel)* mainModel
    ;

cellReferenceOptions
    : (IGNORE | KEEP) NAV
    | UNIQUE (DIMENSION | SINGLE REFERENCE)
    ;

returnRowsClause
    : RETURN (UPDATED | ALL) ROWS
    ;

referenceModel
    : REFERENCE identifier ON '(' selectStatement ')' modelColumnClauses cellReferenceOptions?
    ;

mainModel
    : (MAIN identifier)? modelColumnClauses cellReferenceOptions? modelRulesClause
    ;

modelColumnClauses
    : (PARTITION BY '(' columnList ')')? DIMENSION BY '(' columnList ')' MEASURES '(' columnList ')'
    ;

modelRulesClause
    : RULES (UPDATE | UPSERT ALL?)? ((AUTOMATIC | SEQUENTIAL) ORDER)? modelRule (',' modelRule)*
    ;

modelRule
    : (UPDATE | UPSERT ALL?)? cellAssignment orderByClause? '=' expression
    ;

cellAssignment
    : identifier '[' (ANY | expression) (',' (ANY | expression))* ']'
    ;

forUpdateClause
    : FOR UPDATE (OF columnList)? (NOWAIT | WAIT NUMBER_LITERAL | SKIP_LOCKED)?
    ;

returningClause
    : (RETURN | RETURNING) selectList INTO variableList
    ;

variableList
    : variable (',' variable)*
    ;

variable
    : ':' identifier
    ;

// Oracle-specific expressions
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
    | expression ('+'|'-'|'*'|'/'|'||') expression      # ArithmeticExpression
    | '(' expression ')'                                # ParenthesizedExpression
    | '(' selectStatement ')'                           # SubqueryExpression
    | CASE caseWhenClause+ (ELSE expression)? END       # CaseExpression
    | EXISTS '(' selectStatement ')'                    # ExistsExpression
    | CAST '(' expression AS oracleType ')'             # CastExpression
    | expression '(' '+' ')' expression                 # OuterJoinExpression
    | ROWNUM                                            # RownumExpression
    | ROWID                                             # RowidExpression
    | LEVEL                                             # LevelExpression
    | PRIOR expression                                  # PriorExpression
    | CONNECT_BY_ROOT expression                        # ConnectByRootExpression
    | SYS_CONNECT_BY_PATH '(' expression ',' expression ')'  # SysConnectByPathExpression
    ;

overClause
    : '(' (PARTITION BY expressionList)? (ORDER BY orderByItem (',' orderByItem)*)? (windowingClause)? ')'
    ;

windowingClause
    : (ROWS | RANGE) (BETWEEN frameBound AND frameBound | frameBound)
    ;

frameBound
    : UNBOUNDED (PRECEDING | FOLLOWING)
    | CURRENT ROW
    | expression (PRECEDING | FOLLOWING)
    ;

// Oracle-specific data types
oracleType
    : CHAR ('(' NUMBER_LITERAL (BYTE | CHAR)? ')')?
    | VARCHAR2 ('(' NUMBER_LITERAL (BYTE | CHAR)? ')')?
    | NCHAR ('(' NUMBER_LITERAL ')')?
    | NVARCHAR2 ('(' NUMBER_LITERAL ')')?
    | CLOB | NCLOB
    | LONG
    | NUMBER ('(' NUMBER_LITERAL (',' NUMBER_LITERAL)? ')')?
    | FLOAT ('(' NUMBER_LITERAL ')')?
    | BINARY_FLOAT | BINARY_DOUBLE
    | DATE
    | TIMESTAMP ('(' NUMBER_LITERAL ')')? (WITH (LOCAL)? TIME ZONE)?
    | INTERVAL YEAR ('(' NUMBER_LITERAL ')')? TO MONTH
    | INTERVAL DAY ('(' NUMBER_LITERAL ')')? TO SECOND ('(' NUMBER_LITERAL ')')?
    | RAW ('(' NUMBER_LITERAL ')')?
    | LONG RAW
    | BLOB | BFILE
    | ROWID | UROWID ('(' NUMBER_LITERAL ')')?
    | XMLTYPE
    | JSON
    | SDO_GEOMETRY
    | ANYDATA | ANYTYPE | ANYDATASET
    | identifier  // Custom types
    ;

// Oracle-specific keywords
CONNECT         : C O N N E C T ;
START           : S T A R T ;
NOCYCLE         : N O C Y C L E ;
PRIOR           : P R I O R ;
ROWNUM          : R O W N U M ;
ROWID           : R O W I D ;
LEVEL           : L E V E L ;
CONNECT_BY_ROOT : C O N N E C T '_' B Y '_' R O O T ;
SYS_CONNECT_BY_PATH : S Y S '_' C O N N E C T '_' B Y '_' P A T H ;
MODEL           : M O D E L ;
DIMENSION       : D I M E N S I O N ;
MEASURES        : M E A S U R E S ;
RULES           : R U L E S ;
UPSERT          : U P S E R T ;
AUTOMATIC       : A U T O M A T I C ;
SEQUENTIAL      : S E Q U E N T I A L ;
SINGLE          : S I N G L E ;
REFERENCE       : R E F E R E N C E ;
MAIN            : M A I N ;
IGNORE          : I G N O R E ;
KEEP            : K E E P ;
NAV             : N A V ;
UNIQUE          : U N I Q U E ;
RETURN          : R E T U R N ;
RETURNING       : R E T U R N I N G ;
UPDATED         : U P D A T E D ;
FOR             : F O R ;
NOWAIT          : N O W A I T ;
WAIT            : W A I T ;
SKIP_LOCKED     : S K I P ' ' L O C K E D ;
OF              : O F ;
TO              : T O ;
PARTITION       : P A R T I T I O N ;
OVER            : O V E R ;
ROWS            : R O W S ;
RANGE           : R A N G E ;
UNBOUNDED       : U N B O U N D E D ;
PRECEDING       : P R E C E D I N G ;
FOLLOWING       : F O L L O W I N G ;
CURRENT         : C U R R E N T ;
ROW             : R O W ;
CAST            : C A S T ;
ANY             : A N Y ;

// Oracle data type keywords
CHAR            : C H A R ;
VARCHAR2        : V A R C H A R '2' ;
NCHAR           : N C H A R ;
NVARCHAR2       : N V A R C H A R '2' ;
CLOB            : C L O B ;
NCLOB           : N C L O B ;
LONG            : L O N G ;
NUMBER          : N U M B E R ;
FLOAT           : F L O A T ;
BINARY_FLOAT    : B I N A R Y '_' F L O A T ;
BINARY_DOUBLE   : B I N A R Y '_' D O U B L E ;
DATE            : D A T E ;
TIMESTAMP       : T I M E S T A M P ;
TIME            : T I M E ;
ZONE            : Z O N E ;
LOCAL           : L O C A L ;
INTERVAL        : I N T E R V A L ;
YEAR            : Y E A R ;
MONTH           : M O N T H ;
DAY             : D A Y ;
SECOND          : S E C O N D ;
RAW             : R A W ;
BLOB            : B L O B ;
BFILE           : B F I L E ;
UROWID          : U R O W I D ;
XMLTYPE         : X M L T Y P E ;
JSON            : J S O N ;
SDO_GEOMETRY    : S D O '_' G E O M E T R Y ;
ANYDATA         : A N Y D A T A ;
ANYTYPE         : A N Y T Y P E ;
ANYDATASET      : A N Y D A T A S E T ;
BYTE            : B Y T E ;
