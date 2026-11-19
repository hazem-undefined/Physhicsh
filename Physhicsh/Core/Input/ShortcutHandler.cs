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
        if (IsKey(KeyboardKey.F2)) SystemConfig.Reload();
        if (IsKey(KeyboardKey.Tab)) _openQuickSpawn(mousePos);
        if (IsKey(KeyboardKey.F3)) _img.ShowDebugWindow = !_img.ShowDebugWindow;
        if (IsKey(KeyboardKey.F4)) _img.ImgVault.Toggle(ImGuiWindowState.Settings);

        if (IsKey(KeyboardKey.Z) && _reg.LatestDeletedObjects.Count > 0)
        {
            KeyValuePair<PhysicsObject, AethVec2> returned = _reg.LatestDeletedObjects.Pop().ElementAt(0);
            Spawn.SpawnPhysicsObject(returned.Key, _c.World.CurrentWorld, _reg.PhysicsObjects, returned.Value);
        }

        if (picked != null)
        {
            if (IsKey(KeyboardKey.D) && picked is ICloneable c) Spawn.SpawnPhysicsObject((PhysicsObject)c.Clone(), _c.World.CurrentWorld, _reg.PhysicsObjects, mousePos);

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
