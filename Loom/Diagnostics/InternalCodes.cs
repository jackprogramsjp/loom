namespace Loom.Diagnostics;

internal static class InternalCodes
{
    public const string UnexpectedCharacter = "L001";
    
    public const string UnexpectedToken = "L101";
    public const string UnexpectedEof = "L102";
    public const string UnexpectedType = "L103";
    public const string RedundantOptionalType = "L104";
    
    public const string CannotFindName = "L201";
    public const string UseOfUnassigned = "L202";
    public const string MustHaveInitializer = "L203";
}