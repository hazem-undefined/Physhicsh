using Physhicsh.Core;
using Physhicsh.Debug;
using Physhicsh.Scenes;
using Raylib_cs;
using System;

namespace Physhicsh.Common;

public class CoreContext()
{
    public SceneSettings SceneSettings { get; } = new();
    public PhysicsWorld World { get; } = new();
    public Profiler Profiler { get; } = new();
    public SysVec2 MovedDistance { get; private set; }
    public Camera2D Camera { get; private set; }

    public const int REF_WIDTH = 1280;
    public const int REF_HEIGHT = 640;

    public void InitCamera()
    {
        SysVec2 center = new(REF_WIDTH / 2f, REF_HEIGHT / 2f);
        Camera = new(center, SysVec2.Zero, 0, 1f);
    }
    public void SetCameraZoom(float newZoom)
    {
        Camera = new(Camera.Offset, Camera.Target, Camera.Rotation, newZoom);
    }
    public void MoveCamera(SysVec2 newOffset)
    {
        MovedDistance = newOffset - Camera.Offset;
        Camera = new(Camera.Offset, Camera.Target - (newOffset / Camera.Zoom), Camera.Rotation, Camera.Zoom);
    }
    public void SetCameraTarget(SysVec2 newTarget) => Camera = new(Camera.Offset, newTarget, Camera.Rotation, Camera.Zoom);

    public Camera2D GetRenderCamera()
    {
        float w = Raylib.GetScreenWidth();
        float h = Raylib.GetScreenHeight();
        float scale = MathF.Min(w / REF_WIDTH, h / REF_HEIGHT);

        SysVec2 letterbox = new((w - (REF_WIDTH * scale)) / 2f, (h - (REF_HEIGHT * scale)) / 2f);

        return new Camera2D(
            (Camera.Offset * scale) + letterbox,
            Camera.Target,
            Camera.Rotation,
            Camera.Zoom * scale);
    }
}
