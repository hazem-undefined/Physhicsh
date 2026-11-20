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

    public const int REF_WDITH = 1280;
    public const int REF_HEIGHT = 640;

    public void InitCamera()
    {
        SysVec2 center = new(REF_WDITH / 2, REF_HEIGHT / 2);
        Camera = new(center * 2, center, 0, 1f);
    }
    public void SetCameraZoom(float newZoom)
    {
        Camera = new(Camera.Offset, Camera.Target, Camera.Rotation, newZoom);
    }
    public void SetCameraOffset(SysVec2 newOffset)
    {
        MovedDistance = newOffset - Camera.Offset;
        Camera = new(newOffset, Camera.Target, Camera.Rotation, Camera.Zoom);
    }
    public void SetCameraTarget(SysVec2 newTarget) => Camera = new(Camera.Offset, newTarget, Camera.Rotation, Camera.Zoom);

    public Camera2D GetRenderCamera()
    {
        float w = Raylib.GetScreenWidth();
        float h = Raylib.GetScreenHeight();
        float scale = MathF.Min(w / REF_WDITH, h / REF_HEIGHT);

        SysVec2 letterbox = new((w - REF_WDITH * scale) / 2f, (h - REF_HEIGHT * scale) / 2f);

        return new Camera2D(
            (Camera.Offset * scale) + letterbox,
            Camera.Target,
            Camera.Rotation,
            Camera.Zoom * scale);
    }
}
