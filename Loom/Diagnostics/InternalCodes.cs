namespace Loom.Diagnostics;

public static class InternalCodes
{
    public const string Unknown = "???";
    public const string NotImplemented = "L000";
    public const string CompilerError = "L001";
    
    public const string UnexpectedCharacter = "L101";

    public const string UnexpectedToken = "L201";
    public const string UnexpectedEof = "L202";

    public const string CannotFindName = "L301";
    public const string DuplicateName = "L302";
    public const string UseOfUnassigned = "L303";
    public const string MustHaveInitializer = "L304";
    public const string InfiniteType = "L305";
    public const string TypeMismatch = "L306";
    public const string CannotFindSymbol = "L307";
    public const string NotGeneric = "L308";
    public const string GenericArity = "L309";
    public const string InvalidUnaryOp = "L310";
    public const string InvalidBinaryOp = "L311";
}