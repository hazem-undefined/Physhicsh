using Physhicsh.Config;
using Physhicsh.Core;
using Physhicsh.Debug;
using Physhicsh.Scenes;
using Raylib_cs;

namespace Physhicsh.Common;

public class CoreContext()
{
    public SceneSettings SceneSettings { get; } = new();
    public PhysicsWorld World { get; } = new();
    public Profiler Profiler { get; } = new();
    public Camera2D Camera { get; set; } = new();
    public void InitCamera()
    {
        SysVec2 center = new(SystemConfig.Instance.Graphics.ScreenWidth / 2, SystemConfig.Instance.Graphics.ScreenHeight / 2);
        Camera = new(center, center, 0, 1f);
    }
    public void SetCameraZoom(float newZoom)
    {
        Camera = new(Camera.Offset, Camera.Target, Camera.Rotation, newZoom);
    }
    public void SetCameraOffset(SysVec2 newOffset) => Camera = new(newOffset, Camera.Target, Camera.Rotation, Camera.Zoom);
    public void SetCameraTarget(SysVec2 newTarget) => Camera = new(Camera.Offset, newTarget, Camera.Rotation, Camera.Zoom);
}
