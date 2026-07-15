using System.Diagnostics.CodeAnalysis;
using Loom.TypeGenerator;
using Loom.TypeGenerator.Generators;

var securityLevels = new[] { "None", "PluginSecurity" };
var outputDirectory = args.FirstOrDefault();
if (string.IsNullOrEmpty(outputDirectory))
    Log.Fatal("missing output directory argument");

var dump = await Dumper.GetDump();
var reflectionMetadata = await Dumper.GetReflectionMetadata();

var enumsFilePath = Path.Combine(outputDirectory, "Generated", "enums.loom");
var enumGenerator = new EnumGenerator(enumsFilePath, reflectionMetadata);
enumGenerator.Generate(dump.Enums);
Log.Info($"Successfully generated '{enumsFilePath}'!");

// var definedClassNames = new HashSet<string>();
// for (var i = 0; i < securityLevels.Length; i++)
// {
//     var classesFilePath = Path.Combine(outputDirectory, "Generated", $"{securityLevels[i]}.loom");
//     var classGenerator = new ClassGenerator(classesFilePath,
//         reflectionMetadata,
//         definedClassNames,
//         securityLevels[i],
//         securityLevels.ElementAtOrDefault(i - 1));
//
//     classGenerator.Generate(dump.Classes.ToList());
//     Log.Info($"Successfully generated '{classesFilePath}'!");
// }