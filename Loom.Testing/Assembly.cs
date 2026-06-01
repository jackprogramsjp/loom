using Loom.Diagnostics;
using GC = System.GC;

namespace Loom.Testing;

public class AssemblyFixture : IDisposable
{
    public AssemblyFixture()
    {
        DiagnosticBag.ImmediatelyTerminateOnError = false;
    }
    
    public void Dispose() => GC.SuppressFinalize(this);
}

[CollectionDefinition("Assembly")]
public class AssemblyCollection : ICollectionFixture<AssemblyFixture>;