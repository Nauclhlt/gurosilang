namespace Gurosi;

public enum TokenType {
    Ident,
    Mine,  // Used to import external modules or built-in modules.
    Shorten,
    Module,  // Used to define the name of the module for the current program.
    Run,  // Used to note the entry point.
    If,  // IF
    Else,  // ELSE
    For, // FOR
    While, // WHILE
    How,  // Used to define a function.
    Class,  // Used to define a class.
    Field,  // Used to define a field inside structures or classes.
    Public,  // The public access identifier.
    Private,  // The private access identifier.
    Moduled,  // The moduled access identifier.
    Static,  // The static access identifier.
    String,  // The string type.
    Int, // The int type.
    Float, // The float type.
    Double, // The double type.
    Boolean, // The boolean type.
    Void, // The void type.
    FuncPtr, // The function pointer type.
    Any,  // The any type.
    Array, // The array type marker.
    Arrbase, // The array base type marker.
    Alloc, // Used to allocate an array.
    New,  // Used to create an instance of a class.
    Return, // Used to return a value from subroutines.
    Error, // Used to throw a runtime error.
    Let, // Used to declare a local variable.
    Const, // Used to declare a local constant.
    To,
    Continue,
    Break,
    Scopebreak,
    Init,
    NvCall,
    NvRetv,
    Self,  // the self keyword.
    Memsize, // Used to specify the size of the runtime memory.

    Plus,
    Minus,
    Asterisk,
    Slash,
    Percent,
    Increment,
    Decrement,
    Power,

    Assign,
    AddAssign,
    SubAssign,
    MultAssign,
    DivAssign,
    ModAssign,
    
    Equal,
    NotEqual,
    GreaterThan,
    LessThan,
    GreaterThanEqual,
    LessThanEqual,
    DotOperator,
    Excl,
    LogAnd,
    LogOr,
    As,
    Inherit,
    Macro,
    Extend,

    OpenParen,
    CloseParen,
    OpenBrace,
    CloseBrace,
    OpenBracket,
    CloseBracket,

    AtSign,
    Semicolon,
    Colon,
    BackSlash,
    Underscore,
    Dollar,
    Pipe,
    And,
    Question,
    Comma,

    IntLiteral,
    FloatLiteral,
    DoubleLiteral,
    StringLiteral,
    True,
    False,
    Null,


    MacroTemplate
}