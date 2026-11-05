using Physhicsh.Config;
using System.Text.Json.Serialization;
using Tomlyn;
using Tomlyn.Serialization;

[TomlSerializable(typeof(SystemConfig))]
[TomlSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace,
    WriteIndented = true,
    IndentSize = 2,
    DefaultIgnoreCondition = TomlIgnoreCondition.WhenWritingNull)]
internal sealed partial class ConfigTomlContext : TomlSerializerContext;