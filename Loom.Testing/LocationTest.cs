using Loom.Core.Text;

namespace Loom.Testing;

public class LocationTest
{
    // Line 1 is "let x: number = 5;" (18 chars) followed by '\n', so position 19 is the
    // start of line 2. Positions 0 and 19 share the same column (0) but differ in line.
    private static readonly SourceFile _testFile = new("test.loom", "let x: number = 5;\nlet y = x + 10;\nprint(y);");

    [Fact]
    public void Equals_ComparesPosition_NotColumn()
    {
        var firstLineStart = new Location(_testFile, 0);
        var secondLineStart = new Location(_testFile, 19);

        // Sanity: same column, different line.
        Assert.Equal(firstLineStart.Character, secondLineStart.Character);
        Assert.NotEqual(firstLineStart.Line, secondLineStart.Line);

        Assert.NotEqual(firstLineStart, secondLineStart);
        Assert.False(firstLineStart.Equals(secondLineStart));
    }

    [Fact]
    public void Equals_And_GetHashCode_AgreeForSamePosition()
    {
        var a = new Location(_testFile, 19);
        var b = new Location(_testFile, 19);

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DiffersForDifferentPositions_SameColumn()
    {
        var firstLineStart = new Location(_testFile, 0);
        var secondLineStart = new Location(_testFile, 19);

        Assert.NotEqual(firstLineStart.GetHashCode(), secondLineStart.GetHashCode());
    }
}
