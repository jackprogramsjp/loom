using Loom;
using Loom.Diagnostics;
using Loom.Projects;
using Loom.Utility;

Console.OutputEncoding = System.Text.Encoding.UTF8;
DiagnosticBag.FailFast = true;

var diagnostics = new DiagnosticBag();
var directory = args.ElementAtOrDefault(0) ?? ".";
var loomConfig = ConfigReader.LocateFromDirectory(directory);
if (loomConfig == null)
    throw new Exception("Could not locate Loom configuration file.");

var compilationUnit = new CompilationUnit(loomConfig);
var result = compilationUnit.Compile();
result.Files.ForEach(f => debugFile(f));

return;

void debugFile(CompiledFile compiledFile, bool tokens = true, bool ast = true, bool rebuilt = true, bool luau = true, bool showDiagnostics = true)
{
    var astDisplayer = new ASTDisplayer(compiledFile.Tree);

    if (tokens)
    {
        Console.WriteLine("Tokens:");
        foreach (var token in compiledFile.Tokens)
            Console.WriteLine(token.ToString());
    }

    if (ast)
    {
        Console.WriteLine();
        Console.WriteLine("AST:");
        astDisplayer.Display();
    }

    if (rebuilt)
    {
        Console.WriteLine();
        Console.WriteLine("Rebuilt program:");
        Console.WriteLine(compiledFile.Tree.ToString());
    }

    if (luau)
    {
        Console.WriteLine();
        Console.WriteLine("Compiled Luau program:");
        Console.WriteLine(compiledFile.RenderedLuau);
    }

    if (!showDiagnostics) return;
    var compilerDiagnostics = compiledFile.Diagnostics.ToString();
    Console.WriteLine();
    Console.WriteLine("Diagnostics:");
    Console.WriteLine(string.IsNullOrEmpty(compilerDiagnostics) ? "(none)" : diagnostics);
}
