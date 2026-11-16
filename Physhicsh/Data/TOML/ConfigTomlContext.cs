using Physhicsh.Config;
using Physhicsh.Data.TOML;
using Tomlyn;
using Tomlyn.Serialization;

[TomlSerializable(typeof(SystemConfig))]
[TomlSourceGenerationOptions(
    WriteIndented = true,
    IndentSize = 2,
    DefaultIgnoreCondition = TomlIgnoreCondition.WhenWritingNull,
    Converters = [typeof(ColorTomlConverter)])]
internal sealed partial class ConfigTomlContext : TomlSerializerContext;