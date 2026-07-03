using Loom;
using Loom.Debug;
using Loom.Lexing;
using Loom.Parsing;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var arguments = args;
var toolName = arguments.ElementAtOrDefault(0);
var tools = new Dictionary<string, Action>
{
    ["ast"] = () =>
    {
        var filePath = arguments.ElementAtOrDefault(1);
        if (string.IsNullOrEmpty(filePath))
        {
            Console.WriteLine("Usage: loomtools ast <file>");
            return;
        }

        var file = FileManager.LoadSingle(filePath);
        var lexer = new Lexer(file);
        var result = lexer.Tokenize();
        var parser = new Parser(result);
        var tree = parser.Parse().Tree;
        Console.WriteLine(AstInspector.Inspect(tree));
    },
    ["generate-ast-snapshots"] = () =>
    {
        var snapshotsDir = arguments.ElementAtOrDefault(1);
        var skipExisting = bool.Parse(arguments.ElementAtOrDefault(2) ?? "true");
        if (string.IsNullOrEmpty(snapshotsDir))
        {
            Console.WriteLine("Usage: loomtools generate-ast-snapshots <snapshots-directory>");
            return;
        }

        if (!Directory.Exists(snapshotsDir))
        {
            Console.WriteLine($"Directory not found: {snapshotsDir}");
            return;
        }

        var loomFiles = Directory.GetFiles(snapshotsDir, "*.loom");
        if (loomFiles.Length == 0)
        {
            Console.WriteLine($"No .loom files found in {snapshotsDir}");
            return;
        }

        var skipped = 0;
        foreach (var loomFile in loomFiles)
        {
            var baseName = Path.GetFileNameWithoutExtension(loomFile);
            var outputFile = Path.Combine(snapshotsDir, $"{baseName}.ast");
            if (skipExisting && File.Exists(outputFile))
            {
                Console.WriteLine($"Skipping {Path.GetFileName(loomFile)}.");
                skipped++;
                continue;
            }
            
            Console.WriteLine($"Processing: {Path.GetFileName(loomFile)} -> {baseName}.ast");
            try
            {
                var file = FileManager.LoadSingle(loomFile);
                var lexer = new Lexer(file);
                var result = lexer.Tokenize();
                var parser = new Parser(result);
                var tree = parser.Parse().Tree;
                var astString = AstInspector.Inspect(tree);
                File.WriteAllText(outputFile, astString, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error processing {loomFile}: {ex.Message}");
            }
        }

        Console.WriteLine($"Done. Processed {loomFiles.Length - skipped} files.");
    }
};

if (!string.IsNullOrEmpty(toolName) && tools.TryGetValue(toolName, out var runTool))
{
    runTool();
    return;
}

Console.WriteLine($"Usage: loomtools [{string.Join('|', tools.Keys)}]");