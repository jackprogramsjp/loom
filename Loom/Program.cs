using Loom;
using Loom.Diagnostics;
using Loom.Lexing;
using Loom.Parsing;
using Loom.SemanticAnalysis;
using Loom.TypeChecking;
using Loom.Utility;

var file = FileLoader.LoadSingle("test.loom");
var lexer = new Lexer(file);
var lexerResult = lexer.Tokenize();
var parser = new Parser(lexerResult.Tokens);
var parserResult = parser.Parse();
var astDisplayer = new ASTDisplayer(parserResult.Tree);
var resolver = new Resolver(parserResult.Tree);
var semanticModel = resolver.Resolve();
var typeChecker = new TypeChecker(semanticModel);
var typeCheckerResult = typeChecker.Check();

// DiagnosticBag.FilterSeverity = DiagnosticSeverity.Error;
var lexerDiagnostics = lexerResult.Diagnostics.ToString();
var parserDiagnostics = parserResult.Diagnostics.ToString();
var resolverDiagnostics = semanticModel.Diagnostics.ToString();
var typeCheckerDiagnostics = typeCheckerResult.Diagnostics.ToString();

Console.WriteLine("Tokens:");
foreach (var token in lexerResult.Tokens)
    Console.WriteLine(token.ToString());

Console.WriteLine();
Console.WriteLine("AST:");
astDisplayer.Display();
Console.WriteLine();
Console.WriteLine($"Rebuilt program: {parserResult.Tree}");

Console.WriteLine();
Console.WriteLine("Diagnostics:");
Console.WriteLine("-- lexer --");
Console.WriteLine(string.IsNullOrEmpty(lexerDiagnostics) ? "(none)" : lexerDiagnostics);
Console.WriteLine("-- parser --");
Console.WriteLine(string.IsNullOrEmpty(parserDiagnostics) ? "(none)" : parserDiagnostics);
Console.WriteLine("-- semantic analysis --");
Console.WriteLine(string.IsNullOrEmpty(resolverDiagnostics) ? "(none)" : resolverDiagnostics);
Console.WriteLine("-- type checker --");
Console.WriteLine(string.IsNullOrEmpty(typeCheckerDiagnostics) ? "(none)" : typeCheckerDiagnostics);