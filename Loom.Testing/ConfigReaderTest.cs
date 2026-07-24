using Loom.Config;
using Tomlyn;

namespace Loom.Testing;

public class ConfigReaderTest
{
    [Fact]
    public void Debug_DefaultsToFalse()
    {
        var config = TomlSerializer.Deserialize<LoomConfig>("project_type = \"game\"");
        Assert.NotNull(config);
        Assert.False(config.Debug);
    }

    [Fact]
    public void Debug_ParsesTrue()
    {
        var config = TomlSerializer.Deserialize<LoomConfig>("project_type = \"game\"\ndebug = true");
        Assert.NotNull(config);
        Assert.True(config.Debug);
    }
}
