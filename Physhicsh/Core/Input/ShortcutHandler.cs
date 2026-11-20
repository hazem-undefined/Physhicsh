using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Common;
using Physhicsh.Config;
using Physhicsh.Physics.PhysicsObjects;
using Physhicsh.Physics.Spawning;
using Physhicsh.UI;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Physhicsh.Core.Input;

public class ShortcutHandler(CoreContext c, ImGuiContext imgContx, ObjectRegistry reg, SceneIO sc, Action<AethVec2> openQuickSpawn)
{
    private readonly Action<AethVec2> _openQuickSpawn = openQuickSpawn;
    private readonly CoreContext _c = c;
    private readonly ImGuiContext _img = imgContx;
    private readonly ObjectRegistry _reg = reg;
    private readonly SceneIO _sc = sc;

    public void Handle(AethVec2 mousePos, PhysicsObject? picked)
    {
        SpawnShortcuts(mousePos);
        CameraMovement();

        if (IsKey(KeyboardKey.F2)) SystemConfig.Reload();
        if (IsKey(KeyboardKey.Tab)) _openQuickSpawn(mousePos);
        if (IsKey(KeyboardKey.F3)) _img.ShowDebugWindow = !_img.ShowDebugWindow;
        if (IsKey(KeyboardKey.F4)) _img.ImgVault.Toggle(ImGuiWindowState.Settings);
        if (Raylib.IsKeyDown(KeyboardKey.Minus))
        {
            _c.SetCameraZoom(_c.Camera.Zoom - SystemConfig.Instance.Controls.ZoomSpeed);
        }
        if (Raylib.IsKeyDown(KeyboardKey.Equal))
        {
            _c.SetCameraZoom(_c.Camera.Zoom + SystemConfig.Instance.Controls.ZoomSpeed);
        }
        if (Raylib.GetMouseWheelMove() != 0)
        {
            float mWheel = Raylib.GetMouseWheelMove();
            _c.SetCameraZoom(_c.Camera.Zoom + (mWheel * SystemConfig.Instance.Controls.ZoomSpeed));
        }

        if (IsKey(KeyboardKey.Z) && _reg.LatestDeletedObjects.Count > 0)
        {
            KeyValuePair<PhysicsObject, AethVec2> returned = _reg.LatestDeletedObjects.Pop().ElementAt(0);
            Spawn.SpawnPhysicsObject(returned.Key, _c.World.CurrentWorld, _reg.PhysicsObjects, returned.Value);
        }

        if (picked != null)
        {
            if (IsKey(KeyboardKey.H) && picked is ICloneable c)
            {
                Spawn.SpawnPhysicsObject((PhysicsObject)c.Clone(), _c.World.CurrentWorld, _reg.PhysicsObjects, mousePos, picked.Body.Rotation);
            }

            if (IsKey(KeyboardKey.X)) _sc.DeletePhysicsObject(picked, true);

            if (Raylib.IsKeyPressed(KeyboardKey.V))
            {
                if (picked?.Body.BodyType == BodyType.Kinematic || picked?.Body.BodyType == BodyType.Static)
                {
                    picked?.Body.BodyType = BodyType.Dynamic;
                }
                else
                {
                    picked?.Body.BodyType = BodyType.Static;
                }
            }
        }
    }

    private void CameraMovement()
    {
        float speed = Raylib.IsKeyDown(KeyboardKey.LeftShift) ? SystemConfig.Instance.Controls.CameraSpeed / 2 : SystemConfig.Instance.Controls.CameraSpeed;

        float dx = 0;
        float dy = 0;

        if (Raylib.IsKeyDown(KeyboardKey.W)) dy += speed;
        if (Raylib.IsKeyDown(KeyboardKey.S)) dy -= speed;
        if (Raylib.IsKeyDown(KeyboardKey.A)) dx += speed;
        if (Raylib.IsKeyDown(KeyboardKey.D)) dx -= speed;

        if (dx != 0 || dy != 0)
        {
            _c.MoveCamera(new(dx, dy));
        }
    }

    private void SpawnShortcuts(AethVec2 mouse)
    {
        if (IsKey(KeyboardKey.R))
        {
            Spawn.SpawnRectangle(_c.World.CurrentWorld, mouse, _reg.PhysicsObjects);
        }
        if (IsKey(KeyboardKey.C)) Spawn.SpawnCircle(_c.World.CurrentWorld, mouse, _reg.PhysicsObjects);
    }

    private static bool IsKey(KeyboardKey k) =>
        Raylib.IsKeyPressed(k) || Raylib.IsKeyPressedRepeat(k);
}
