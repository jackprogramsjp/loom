using Loom;
using Loom.Diagnostics;
using Loom.Projects;

Console.OutputEncoding = System.Text.Encoding.UTF8;
// DiagnosticBag.FailFast = false;

var directory = args.ElementAtOrDefault(0) ?? ".";
var loomConfig = ConfigReader.LocateFromDirectory(directory);
if (loomConfig == null)
    throw new ArgumentException($"Could not locate Loom configuration file in directory '{directory}'.");

var compilationUnit = new CompilationUnit(loomConfig);
var result = compilationUnit.Compile();
var debugInfo = result.Files
    .FindAll(f => !f.SourceFile.IsDeclaration)
    .ConvertAll(f => f.GetDebugInfo(rebuilt: false, debugDiagnostics: false));

Console.WriteLine(string.Join(Environment.NewLine, debugInfo));