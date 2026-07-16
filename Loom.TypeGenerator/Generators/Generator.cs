using System.Reflection;
using System.Text;
using Loom.TypeGenerator.ApiTypes;

namespace Loom.TypeGenerator.Generators;

internal class Generator(
    string filePath,
    ReflectionMetadataReader metadata,
    HashSet<string> definedClassNames,
    string security,
    string? lowerSecurity = null
)
    : BaseGenerator(filePath, metadata)
{
    public void Generate(Dump dump)
    {
        try
        {
            Write("## This is an automatically generated file and it should not be edited manually.");
            
            var enumGenerator = new EnumGenerator(FilePath, Metadata);
            enumGenerator.Generate(dump.Enums);

            var classGenerator = new ClassGenerator(FilePath, Metadata, definedClassNames, security, lowerSecurity);
            classGenerator.Generate(dump.Classes);

            var generatorDirectory = Path.GetDirectoryName(
                Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)))
            );
            
            if (generatorDirectory == null)
                Log.Fatal("could not find type generator project directory");

            Stream.Append(enumGenerator.Stream);
            var filesToCopy = Directory.GetFiles(generatorDirectory, "*.loom", SearchOption.TopDirectoryOnly);
            foreach (var file in filesToCopy)
            {
                Stream.AppendLine(File.ReadAllText(file));
                Write();
                Log.Info($"wrote contents of '{Path.GetRelativePath(generatorDirectory, file)}'");
            }
            
            Stream.Append(classGenerator.Stream);
            WriteFile();
        }
        catch (Exception e)
        {
            Log.Fatal($"unexpected error: {e.Message}");
        }
    }

    private void WriteFile()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        File.WriteAllText(FilePath, Stream.ToString());
    }
}