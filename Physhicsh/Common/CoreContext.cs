using Physhicsh.Core;
using Physhicsh.Debug;
using Physhicsh.Scenes;

namespace Physhicsh.Common;

public class CoreContext
{
    public SceneSettings SceneSettings { get; } = new();
    public PhysicsWorld World { get; } = new();
    public Profiler Profiler { get; } = new();
}
