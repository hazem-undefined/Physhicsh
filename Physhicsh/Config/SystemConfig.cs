using System;
using System.IO;
using System.Threading;
using Tomlyn;
using Tomlyn.Serialization;

namespace Physhicsh.Config
{
    [TomlSerializable(typeof(SystemConfig))]
    public class SystemConfig
    {
        public ScreenConfig Screen { get; set; } = new();
        public PhysicsConfig Physics { get; set; } = new();
        public ControlsConfig Controls { get; set; } = new();
        public PathConfig Paths { get; set; } = new();
        public const string PHYSHICSH_SCENE_FILE = ".psf";
        public const string PHYSHICSH_SETTINGS_FILE = ".ptf";
        public static AethVec2 ScreenCenter => new(Instance.Screen.SCREEN_WIDTH / 2,
                                                  Instance.Screen.SCREEN_HEIGHT / 2);
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
                    string toml = TomlSerializer.Serialize(defaults);
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

                SystemConfig config = TomlSerializer.Deserialize<SystemConfig>(toml);

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