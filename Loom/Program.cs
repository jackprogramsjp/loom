using Loom;
using Loom.Diagnostics;
using Loom.Lexing;
using Loom.Parsing;

var file = FileLoader.LoadSingle("test.loom");
var lexer = new Lexer(file);
var lexerResult = lexer.Tokenize();
var parser = new Parser(lexerResult.Tokens);
var parserResult = parser.Parse();

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
Console.WriteLine(parserResult.Tree);

Console.WriteLine();
Console.WriteLine("Diagnostics:");
Console.WriteLine("-- lexer --");
Console.WriteLine(string.IsNullOrEmpty(lexerDiagnostics) ? "(none)" : lexerDiagnostics);
Console.WriteLine("-- parser --");
Console.WriteLine(string.IsNullOrEmpty(parserDiagnostics) ? "(none)" : parserDiagnostics);