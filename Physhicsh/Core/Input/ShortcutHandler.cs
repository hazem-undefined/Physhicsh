using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Config;
using Physhicsh.Physics.PhysicsObjects;
using Physhicsh.Physics.Spawning;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Physhicsh.Core.Input;

public class ShortcutHandler
{
    private readonly Action<AethVec2> _openQuickSpawn;
    private Common _c;

    public ShortcutHandler(Common c, Action<AethVec2> openQuickSpawn)
    {
        _c = c;
        _openQuickSpawn = openQuickSpawn;
    }

    public void Handle(AethVec2 mousePos, PhysicsObject? picked)
    {
        SpawnShortcuts(mousePos);
        if (IsKey(KeyboardKey.F2)) SystemConfig.Reload();
        if (IsKey(KeyboardKey.Tab)) _openQuickSpawn(mousePos);
        if (IsKey(KeyboardKey.F3)) _c.ShowDebugWindow = !_c.ShowDebugWindow;

        if (IsKey(KeyboardKey.Z) && _c.LatestDeletedObjects.Count > 0)
        {
            KeyValuePair<PhysicsObject, AethVec2> returned = _c.LatestDeletedObjects.Pop().ElementAt(0);
            Spawn.SpawnPhysicsObject(returned.Key, _c.World.CurrentWorld, _c.PhysicsObjects, returned.Value);
        }

        if (picked != null)
        {
            if (IsKey(KeyboardKey.D) && picked is ICloneable c) Spawn.SpawnPhysicsObject((PhysicsObject)c.Clone(), _c.World.CurrentWorld, _c.PhysicsObjects, mousePos);

            if (IsKey(KeyboardKey.X)) _c.DeletePhysicsObject(picked);

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
            Spawn.SpawnRectangle(_c.World.CurrentWorld, mouse, _c.PhysicsObjects);
        }
        if (IsKey(KeyboardKey.C)) Spawn.SpawnCircle(_c.World.CurrentWorld, mouse, _c.PhysicsObjects);
    }

    private static bool IsKey(KeyboardKey k) =>
        Raylib.IsKeyPressed(k) || Raylib.IsKeyPressedRepeat(k);
}
