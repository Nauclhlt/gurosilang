namespace Gurosi;

public static class ErrorProvider
{
    public static class Parser
    {
        public static string InvalidToken(string received, string expected)
            => $"[E001] Invalid token '{received}', expected '{expected}'.";
        public static string InvalidType()
            =>  "[E002] Invalid type.";
        public static string InvalidExpressionToken(string tokenValue)
            => $"[E003] Invalid token in expression '{tokenValue}'.";
        public static string RunBlockDuplicate()
            => $"[E004] Run block is already defined.";
        public static string ModuleNameDuplicate()
            => $"[E005] Module name is already declared.";
        public static string InvalidContinue()
            => $"[E006] Invalid continue out of loops.";
        public static string InvalidBreak()
            => $"[E007] Invalid break out of loops.";
        public static string InvalidScopebreak()
            => $"[E008] Invalid scopebreak out of scopes.";
        public static string ConstructorStatic()
            => $"[E009] Constructor needs to be non-static.";
        public static string ModuleNameMissing()
            => $"[E010] Module name is not declared.";
        public static string InvalidGenericType()
            => $"[E011] Invalid generic type.";
        public static string InvalidSimpleMacro()
            => $"[E012] Invalid macro.";
        public static string InvalidTemplateMacro()
             => $"[E013] Invalid argumented macro.";
        public static string ModuleNameNotAvailable(string moduleName)
        => $"[E014] Module name '{moduleName}' cannot be used.";
        public static string MacroNameNotFound(string name)
            => $"[E015] Argumented macro named '{name}' not found.";
        public static string ConstValueMissing()
            => $"[E016] A constant must have an initial value.";
        public static string InvalidPointerTarget()
            => $"[E017] Cannot retrieve the pointer for the specified value.";
        public static string InvalidMemorySize(int value)
            => $"[E018] '{value}' cannot be used as the memory size. Memory size needs to be a number not less than 32.";
    }

    public static string InvalidType(TypePath type)
        => $"[C001] Type '{type.ToString()}' not found.";
    public static string RunBlockDuplicate()
        => $"[C002] Source code must only contain one run block.";
    public static string RunBlockMissing()
        => $"[C003] Source code must contain a run block.";
    public static string LetTypeMismatch(string localName, TypePath supposedType)
        => $"[C004] Initial value for local '{localName}' is not compatible with the type '{supposedType.ToString()}'.";
    public static string LocalAssignTypeMismatch(string localName, TypePath supposedType)
        => $"[C005] Value assigned to local '{localName}' is not compatible with the type '{supposedType.ToString()}'.";
    public static string FieldAssignTypeMismatch(string fieldName, TypePath supposedType)
        => $"[C006] Value assigned to field '{fieldName}' is not compatible with the type '{supposedType.ToString()}'.";
    public static string LocalNameDuplicate(string name)
        => $"[C007] Local named '{name}' is already declared in the scope.";
    public static string SymbolNotFoundInScope(string symbolName)
        => $"[C008] Symbol '{symbolName}' is not found in the scope.";
    public static string ReturnTypeMismatch(TypePath returnType)
        => $"[C009] Value of the type '{returnType}' needs to be returned.";
    public static string OperatorTypeMismatch(TypePath left, TypePath right, Operator op)
        => $"[C010] Cannot apply the operator '{op}' betweeen '{left}' and '{right}'";
    public static string ClassSymbolNotFound(string symbolName, TypePath source)
        => $"[C011] Symbol '{symbolName}' is not found in the type '{source}'.";
    public static string ImportNotFound(string libName)
        => $"[C012] Cannot import the library '{libName}'. (Not found.)";
    public static string ArgumentTypeMismatch(int argumentIndex, TypePath supposedType)
        => $"[C013] Type of the passed argument is not compatible. {(argumentIndex + 1).ToOrdinal()} argument needs to be '{supposedType}'";
    public static string InvalidFunction()
        => $"[C014] Invalid function expression.";
    public static string SingleOperatorTypeMismatch(TypePath type, string op)
        => $"[C015] Cannot apply the operator '{op}' on the type '{type}'.";
    public static string InvalidBreak()
        => $"[C016] 'break' cannot be used here.";
    public static string InvalidContinue()
        => $"[C017] 'continue' cannot be used here.";
    public static string InvalidScopebreak()
        => $"[C018] 'scopebreak' cannot be used here.";
    public static string ReturnValueMissing()
        => $"[C019] Return value required.";
    public static string ReturnValueNotRequired()
        => $"[C020] Do not return a value.";
    public static string NotAllPathReturns()
        => $"[C021] There is one or more code path(s) which return(s) no value.";
    public static string ArrayLengthNotInt()
        => $"[C022] Length of an array needs to be an integer.";
    public static string IndexTargetNotArray()
        => $"[C023] Indexer can only be applied on arrays.";
    public static string InvalidFunctionOverload()
        => $"[C024] Matching function overload not found.";
    public static string NotInstantiatableType(TypePath failedType)
        => $"[C025] Cannot create an instance of type '{failedType}'";
    public static string InvalidConstructor()
        => $"[C026] Matching constructor overload not found.";
    public static string InvalidConditionType()
        => $"[C027] Condition needs to be 'sys::boolean' or 'sys::int'.";
    public static string LocalNameFieldConflict(string name)
        => $"[C028] The local name '{name}' cannot be used here.";
    public static string NotAssignableExpression()
        => $"[C029] Not assignable expression.";
    public static string NotAccessible(string symbolName)
        => $"[C030] '{symbolName}' is not accessible here.";
    public static string ConstructorNotAccessible()
        => $"[C031] Constructor not accessible here.";
    public static string NotArrayIndexed()
        => $"[C032] Cannot apply indexer on non-array symbol.";
    public static string ArrayTypeMismatch(TypePath supposedType)
        => $"[C033] Only '{supposedType}' can be assigned to the specified array.";
    public static string InvalidPrimitiveCast(TypePath fromType, TypePath toType)
        => $"[C034] '{fromType}' cannot be casted to '{toType}'.";
    public static string InvalidInheritance(TypePath sourceType)
        => $"[C035] Cannot inherit from '{sourceType}'.";
    public static string InvalidConstAssignment()
        => $"[C036] Cannot rewrite the value of constant.";
    public static string InvalidErrorType()
        => $"[C037] Error codes and error messages need to be string.";

    private static string ToOrdinal(this int self)
    {
        const string First = "1st";
        const string Second = "2nd";
        const string Third = "3rd";
        return self switch
        {
            1 => First,
            2 => Second,
            3 => Third,
            _ => self + "th"
        };
    }
}