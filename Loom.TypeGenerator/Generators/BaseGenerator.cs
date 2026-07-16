using System.Text;

namespace Loom.TypeGenerator.Generators;

internal abstract class BaseGenerator(string filePath, ReflectionMetadataReader metadata)
{
    public readonly StringBuilder Stream = new(4096);
    
    protected readonly ReflectionMetadataReader Metadata = metadata;
    protected readonly string FilePath = filePath;

    private int _indentLevel;
    
    protected void WriteBlock(string header, Action writeBlock, bool finalNewline = true)
    {
        Write(header + " {");
        PushIndent();
        writeBlock();
        PopIndent();
        Write("}");

        if (!finalNewline) return;
        Write();
    }

    protected void WriteList<T>(IEnumerable<T> enumerable, Func<T, string> renderElement)
    {
        foreach (var item in enumerable)
            Write(renderElement(item));
    }

    protected void Write() => Write("");

    protected void Write(string line) =>
        Stream.Append('\t', _indentLevel).Append(line).Append('\n');

    protected void PushIndent() => _indentLevel++;
    protected void PopIndent() => _indentLevel--;
}