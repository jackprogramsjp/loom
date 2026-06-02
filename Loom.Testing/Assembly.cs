using Loom.Diagnostics;
using GC = System.GC;

namespace Loom.Testing;

// ReSharper disable once ClassNeverInstantiated.Global
public class AssemblyFixture : IDisposable
{
    public static readonly string TestFiles = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}TestFiles";
    
    public AssemblyFixture() => DiagnosticBag.FailFast = false;

    public void Dispose() => GC.SuppressFinalize(this);
}

[CollectionDefinition("Assembly")]
public class AssemblyCollection : ICollectionFixture<AssemblyFixture>;