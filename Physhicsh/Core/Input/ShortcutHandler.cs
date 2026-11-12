using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Config;
using Physhicsh.Physics.PhysicsObjects;
using Physhicsh.Physics.Spawning;
using Physhicsh.UI;
using Raylib_cs;
using System;
using System.Collections.Generic;

namespace Physhicsh.Core.Input;

public class ShortcutHandler
{
    private readonly World _world;
    private readonly List<PhysicsObject> _objs;
    private readonly ImGuiUI _ui;
    private readonly Action<PhysicsObject> _delete;
    private readonly Action _toggleDebug;
    private readonly Action<AethVec2> _openQuickSpawn;

    public ShortcutHandler(World world, List<PhysicsObject> objects, ImGuiUI ui, Action<PhysicsObject> delete, Action toggleDebug, Action<AethVec2> openQuickSpawn)
    {
        _world = world;
        _objs = objects;
        _ui = ui;
        _delete = delete;
        _toggleDebug = toggleDebug;
        _openQuickSpawn = openQuickSpawn;
    }

    public void Handle(AethVec2 mousePos, PhysicsObject? picked)
    {
        SpawnShortcuts(mousePos);
        if (IsKey(KeyboardKey.F2)) SystemConfig.Reload();
        if (IsKey(KeyboardKey.Tab)) _openQuickSpawn(mousePos);
        if (IsKey(KeyboardKey.F3))
            _toggleDebug();

        if (picked is null) return;

        if (IsKey(KeyboardKey.D) && picked is ICloneable c)
            Spawn.SpawnPhysicsObject((PhysicsObject)c.Clone(), _world, _objs, mousePos);

        if (IsKey(KeyboardKey.X)) _delete(picked);
    }

    private void SpawnShortcuts(AethVec2 mouse)
    {
        if (IsKey(KeyboardKey.R))
        {
            Spawn.SpawnRectangle(_world, mouse, _objs);
        }
        if (IsKey(KeyboardKey.C)) Spawn.SpawnCircle(_world, mouse, _objs);
    }

    private static bool IsKey(KeyboardKey k) =>
        Raylib.IsKeyPressed(k) || Raylib.IsKeyPressedRepeat(k);
}
