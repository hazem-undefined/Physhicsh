using Physhicsh.Config;
using Tomlyn;
using Tomlyn.Serialization;

[TomlSerializable(typeof(SystemConfig))]
[TomlSourceGenerationOptions(
    WriteIndented = true,
    IndentSize = 2,
    DefaultIgnoreCondition = TomlIgnoreCondition.WhenWritingNull)]
internal sealed partial class ConfigTomlContext : TomlSerializerContext;