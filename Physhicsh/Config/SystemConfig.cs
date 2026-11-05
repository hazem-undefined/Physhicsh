using System;
using System.IO;
using System.Threading;
using Tomlyn;

namespace Physhicsh.Config
{
    public class SystemConfig
    {
        public ScreenConfig Screen { get; set; } = new();
        public PhysicsConfig Physics { get; set; } = new();
        public ControlsConfig Controls { get; set; } = new();
        public PathConfig Paths { get; set; } = new();
        public LimitsConfig Limits { get; set; } = new();
        public const string PHYSHICSH_SCENE_FILE = ".psf";
        public const string PHYSHICSH_SETTINGS_FILE = ".ptf";
        public static AethVec2 ScreenCenter => new(Instance.Screen.ScreenWidth / 2,
                                                  Instance.Screen.ScreenHeight / 2);
        private static SystemConfig? _instance;
        private static readonly Lock _lock = new();

        public static SystemConfig Instance
        {
            get
            {
                if (_instance is null)
                {
                    lock (_lock)
                    {
                        _instance ??= LoadFromFile("config.toml");
                    }
                }
                return _instance;
            }
        }

        private static SystemConfig LoadFromFile(string path)
        {
            if (!File.Exists(path))
            {
                SystemConfig defaults = new SystemConfig();
                try
                {
                    string toml = TomlSerializer.Serialize(defaults, ConfigTomlContext.Default.SystemConfig);
                    File.WriteAllText(path, toml);
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    throw new InvalidOperationException(
                        $"Failed to create default configuration file '{path}': {ex.Message}", ex);
                }
                return defaults;
            }

            try
            {
                string toml = File.ReadAllText(path);

                SystemConfig config = TomlSerializer.Deserialize<SystemConfig>(toml, ConfigTomlContext.Default.SystemConfig);

                return config ?? throw new InvalidOperationException(
                    $"Failed to deserialize configuration from '{path}'.");
            }
            catch (Exception ex) when (ex is FormatException or IOException)
            {
                throw new InvalidOperationException(
                    $"Error parsing configuration file '{path}': {ex.Message}", ex);
            }
        }
    }
}