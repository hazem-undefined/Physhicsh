using Physhicsh.Config;
using Physhicsh.Physics.PhysicsObjects;
using Physhicsh.Physics.Spawning;
using Raylib_cs;
using System;

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

        if (picked is null) return;

        if (IsKey(KeyboardKey.D) && picked is ICloneable c)
            Spawn.SpawnPhysicsObject((PhysicsObject)c.Clone(), _c.World.CurrentWorld, _c.PhysicsObjects, mousePos);

        if (IsKey(KeyboardKey.X)) _c.DeletePhysicsObject(picked);
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
