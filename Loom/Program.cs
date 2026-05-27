using Loom;
using Loom.Diagnostics;
using Loom.Lexing;
using Loom.Parsing;
using Loom.Parsing.AST.Traversal;

var file = FileLoader.LoadSingle("test.loom");
var lexer = new Lexer(file);
var lexerResult = lexer.Tokenize();
var parser = new Parser(lexerResult.Tokens);
var parserResult = parser.Parse();
var astDisplayer = new ASTDisplayer();

DiagnosticBag.FilterSeverity = DiagnosticSeverity.Error;
var lexerDiagnostics = lexerResult.Diagnostics.ToString();
var parserDiagnostics = parserResult.Diagnostics.ToString();

Console.WriteLine("Tokens:");
foreach (var token in lexerResult.Tokens)
{
    Console.WriteLine(token.ToString());
}
Console.WriteLine();
Console.WriteLine("AST:");
astDisplayer.Display(parserResult.Tree);
Console.WriteLine();
Console.WriteLine($"Rebuilt program: {parserResult.Tree}");

Console.WriteLine();
Console.WriteLine("Diagnostics:");
Console.WriteLine("-- lexer --");
Console.WriteLine(string.IsNullOrEmpty(lexerDiagnostics) ? "(none)" : lexerDiagnostics);
Console.WriteLine("-- parser --");
Console.WriteLine(string.IsNullOrEmpty(parserDiagnostics) ? "(none)" : parserDiagnostics);