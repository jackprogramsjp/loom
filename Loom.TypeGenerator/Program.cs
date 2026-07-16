using Loom.TypeGenerator;
using Loom.TypeGenerator.Generators;

var securityLevels = new[] { "None", "PluginSecurity" };
var outputDirectory = Path.Combine(args.FirstOrDefault("."), "generated");
var dump = await Dumper.GetDump();
var metadata = await Dumper.GetReflectionMetadata();
var definedClassNames = new HashSet<string>();
for (var i = 0; i < securityLevels.Length; i++)
{
    var security = securityLevels[i];
    var filePath = Path.Combine(outputDirectory, $"{security}.loom");
    var lowerSecurity = securityLevels.ElementAtOrDefault(i - 1);
    var classGenerator = new Generator(filePath, metadata, definedClassNames, security, lowerSecurity);
    classGenerator.Generate(dump);
    Log.Info($"successfully generated '{filePath}'!");
}