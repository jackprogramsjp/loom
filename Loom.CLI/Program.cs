using Loom;
using Loom.Diagnostics;
using Loom.Projects;

Console.OutputEncoding = System.Text.Encoding.UTF8;
DiagnosticBag.FailFast = false;

var directory = args.ElementAtOrDefault(0) ?? ".";
var loomConfig = ConfigReader.LocateFromDirectory(directory);
if (loomConfig == null)
    throw new Exception("Could not locate Loom configuration file.");

var compilationUnit = new CompilationUnit(loomConfig);
var result = compilationUnit.Compile();
result.Files.ForEach(f => f.WriteDebugInfo(tokens: false, ast: true, debugDiagnostics: false));