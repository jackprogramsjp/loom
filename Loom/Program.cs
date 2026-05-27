using Loom;

var file = FileLoader.LoadSingle("test.loom");
var lexer = new Lexer(file);
var tokens = lexer.Tokenize();

foreach (var token in tokens)
{
    Console.WriteLine(token.Examine());
}