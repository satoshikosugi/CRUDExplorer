grammar Sql;

// Parser Rules

// Main SQL statement
sqlStatement
    : selectStatement
    | insertStatement
    | updateStatement
    | deleteStatement
    | EOF
    ;

// SELECT Statement
selectStatement
    : withClause? selectClause fromClause? whereClause? groupByClause? havingClause? orderByClause? (UNION | MINUS | INTERSECT) ALL? selectStatement?
    ;

withClause
    : WITH RECURSIVE? cteList
    ;

cteList
    : cte (',' cte)*
    ;

cte
    : identifier AS '(' selectStatement ')'
    ;

selectClause
    : SELECT DISTINCT? selectList
    ;

selectList
    : selectItem (',' selectItem)*
    | '*'
    ;

selectItem
    : expression (AS? identifier)?
    ;

fromClause
    : FROM tableReference (',' tableReference)*
    ;

tableReference
    : tableName (AS? tableAlias)?
    | '(' selectStatement ')' (AS? tableAlias)?
    | tableReference joinClause
    ;

joinClause
    : (INNER | LEFT | RIGHT | FULL | CROSS)? OUTER? JOIN tableReference (ON expression)?
    | NATURAL (INNER | LEFT | RIGHT | FULL)? JOIN tableReference
    ;

whereClause
    : WHERE expression
    ;

groupByClause
    : GROUP BY expression (',' expression)*
    ;

havingClause
    : HAVING expression
    ;

orderByClause
    : ORDER BY orderByItem (',' orderByItem)*
    ;

orderByItem
    : expression (ASC | DESC)? (NULLS (FIRST | LAST))?
    ;

// INSERT Statement
insertStatement
    : INSERT INTO tableName ('(' columnList ')')? (VALUES valuesList | selectStatement)
    ;

columnList
    : identifier (',' identifier)*
    ;

valuesList
    : '(' expressionList ')' (',' '(' expressionList ')')*
    ;

expressionList
    : expression (',' expression)*
    ;

// UPDATE Statement
updateStatement
    : UPDATE tableName SET setClauseList whereClause?
    ;

setClauseList
    : setClause (',' setClause)*
    ;

setClause
    : identifier '=' expression
    ;

// DELETE Statement
deleteStatement
    : DELETE FROM tableName whereClause?
    ;

// Expressions
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
    | expression ('+'|'-'|'*'|'/'|'%'|'||') expression  # ArithmeticExpression
    | '(' expression ')'                                # ParenthesizedExpression
    | '(' selectStatement ')'                           # SubqueryExpression
    | CASE caseWhenClause+ (ELSE expression)? END       # CaseExpression
    | EXISTS '(' selectStatement ')'                    # ExistsExpression
    ;

caseWhenClause
    : WHEN expression THEN expression
    ;

comparisonOperator
    : '=' | '<>' | '!=' | '<' | '<=' | '>' | '>='
    ;

// Identifiers and Names
tableName
    : identifier ('.' identifier)?
    ;

tableAlias
    : identifier
    ;

functionName
    : identifier
    ;

identifier
    : IDENTIFIER
    | QUOTED_IDENTIFIER
    | keyword
    ;

keyword
    : SELECT | FROM | WHERE | INSERT | UPDATE | DELETE | INTO | VALUES
    | SET | AS | AND | OR | NOT | IN | BETWEEN | LIKE | IS | NULL
    | JOIN | INNER | LEFT | RIGHT | FULL | OUTER | CROSS | NATURAL | ON
    | GROUP | BY | HAVING | ORDER | ASC | DESC | DISTINCT | ALL
    | UNION | MINUS | INTERSECT | WITH | RECURSIVE | CASE | WHEN | THEN
    | ELSE | END | EXISTS | ESCAPE | NULLS | FIRST | LAST
    ;

// Literals
literal
    : STRING_LITERAL
    | NUMBER_LITERAL
    | NULL
    | TRUE
    | FALSE
    ;

// Lexer Rules

// Keywords (case-insensitive)
SELECT      : S E L E C T ;
FROM        : F R O M ;
WHERE       : W H E R E ;
INSERT      : I N S E R T ;
UPDATE      : U P D A T E ;
DELETE      : D E L E T E ;
INTO        : I N T O ;
VALUES      : V A L U E S ;
SET         : S E T ;
AS          : A S ;
AND         : A N D ;
OR          : O R ;
NOT         : N O T ;
IN          : I N ;
BETWEEN     : B E T W E E N ;
LIKE        : L I K E ;
IS          : I S ;
NULL        : N U L L ;
JOIN        : J O I N ;
INNER       : I N N E R ;
LEFT        : L E F T ;
RIGHT       : R I G H T ;
FULL        : F U L L ;
OUTER       : O U T E R ;
CROSS       : C R O S S ;
NATURAL     : N A T U R A L ;
ON          : O N ;
GROUP       : G R O U P ;
BY          : B Y ;
HAVING      : H A V I N G ;
ORDER       : O R D E R ;
ASC         : A S C ;
DESC        : D E S C ;
DISTINCT    : D I S T I N C T ;
ALL         : A L L ;
UNION       : U N I O N ;
MINUS       : M I N U S ;
INTERSECT   : I N T E R S E C T ;
WITH        : W I T H ;
RECURSIVE   : R E C U R S I V E ;
CASE        : C A S E ;
WHEN        : W H E N ;
THEN        : T H E N ;
ELSE        : E L S E ;
END         : E N D ;
EXISTS      : E X I S T S ;
ESCAPE      : E S C A P E ;
NULLS       : N U L L S ;
FIRST       : F I R S T ;
LAST        : L A S T ;
TRUE        : T R U E ;
FALSE       : F A L S E ;

// Identifiers
IDENTIFIER
    : [a-zA-Z_][a-zA-Z0-9_]*
    ;

QUOTED_IDENTIFIER
    : '"' (~["\r\n])* '"'
    | '`' (~[`\r\n])* '`'
    | '[' (~[\]\r\n])* ']'
    ;

// Literals
STRING_LITERAL
    : '\'' (~'\'' | '\'\'')* '\''
    ;

NUMBER_LITERAL
    : [0-9]+ ('.' [0-9]+)? ([eE][+-]?[0-9]+)?
    ;

// Operators and Punctuation
// Already defined in parser rules

// Whitespace and Comments
WS
    : [ \t\r\n]+ -> skip
    ;

LINE_COMMENT
    : '--' ~[\r\n]* -> skip
    ;

BLOCK_COMMENT
    : '/*' .*? '*/' -> skip
    ;

// Case-insensitive fragments
fragment A : [aA] ;
fragment B : [bB] ;
fragment C : [cC] ;
fragment D : [dD] ;
fragment E : [eE] ;
fragment F : [fF] ;
fragment G : [gG] ;
fragment H : [hH] ;
fragment I : [iI] ;
fragment J : [jJ] ;
fragment K : [kK] ;
fragment L : [lL] ;
fragment M : [mM] ;
fragment N : [nN] ;
fragment O : [oO] ;
fragment P : [pP] ;
fragment Q : [qQ] ;
fragment R : [rR] ;
fragment S : [sS] ;
fragment T : [tT] ;
fragment U : [uU] ;
fragment V : [vV] ;
fragment W : [wW] ;
fragment X : [xX] ;
fragment Y : [yY] ;
fragment Z : [zZ] ;
