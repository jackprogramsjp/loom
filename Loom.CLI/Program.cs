using Loom;
using Loom.Utility;

var file = FileLoader.LoadSingle("test.loom");
var compiledFile = CompilationUnit.CompileFile(file);
var astDisplayer = new ASTDisplayer(compiledFile.Tree);

Console.WriteLine("Tokens:");
foreach (var token in compiledFile.Tokens)
    Console.WriteLine(token.ToString());

Console.WriteLine();
Console.WriteLine("AST:");
astDisplayer.Display();

Console.WriteLine();
Console.WriteLine("Rebuilt program:");
Console.WriteLine(compiledFile.Tree.ToString());
Console.WriteLine();
Console.WriteLine("Compiled Luau program:");
Console.WriteLine(compiledFile.RenderedLuau);

var diagnostics = compiledFile.Diagnostics.NotInfo().ToString();
Console.WriteLine();
Console.WriteLine("Diagnostics:");
Console.WriteLine(string.IsNullOrEmpty(diagnostics) ? "(none)" : diagnostics);
