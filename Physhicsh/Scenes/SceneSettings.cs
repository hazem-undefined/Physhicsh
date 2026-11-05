using Tomlyn.Serialization;

namespace Physhicsh.Scenes;

[TomlSerializable(typeof(SceneSettings))]
public class SceneSettings
{
    public RayColor CurrentBgColor { get; set; } = RayColor.Black;
    public float GravityScale { get; set; } = 3;
    public SpawnRules SpawnRules { get; set; }
}
