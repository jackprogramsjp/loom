namespace Loom.TypeChecking.Types;

public abstract class Type
{
    public virtual bool IsAssignableTo(Type other) => false;
}