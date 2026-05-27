using Loom;
using Loom.Diagnostics;

var file = FileLoader.LoadSingle("test.loom");
var lexer = new Lexer(file);
var tokens = lexer.Tokenize();

foreach (var token in tokens)
{
    Console.WriteLine(token.Examine());
}

lexer.Diagnostics.SetSeverityFilter(DiagnosticSeverity.Error);
var diagnostics = lexer.Diagnostics.ToString();
Console.WriteLine("\nDiagnostics:");
Console.WriteLine(string.IsNullOrEmpty(diagnostics) ? "(none)" : diagnostics);