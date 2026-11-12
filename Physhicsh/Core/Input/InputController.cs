using ImGuiNET;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Core.Input.Drag;
using Physhicsh.Extensions;
using Physhicsh.Physics.PhysicsObjects;
using Physhicsh.UI;
using Raylib_cs;
using System;
using System.Collections.Generic;

namespace Physhicsh.Core.Input;

public sealed class InputController
{
    private readonly World _world;
    private readonly ImGuiUI _ui;
    private readonly Selection _selection;
    private readonly DragController _drag;
    private readonly ShortcutHandler _shortcuts;
    private readonly RotationController _rotation;
    private List<PhysicsObject> _objs;
    public AethVec2 MousePosWorld { get; private set; }
    public PhysicsObject? PickedObject => _selection.Get();
    public bool ShowDebugWindow { get; set; }

    public InputController(World world, ImGuiUI ui, List<PhysicsObject> objs, Action<PhysicsObject> delete)
    {
        _world = world;
        _ui = ui;
        _selection = new();
        _rotation = new();
        _drag = new(world);
        _objs = objs;
        // Move ShowDebugWindow and it's look a likes into a global class
        _shortcuts = new(world, objs, ui, delete, () => ShowDebugWindow = !ShowDebugWindow, pos => OpenQuickSpawnMenu(pos));
    }

    public void Tick()
    {
        MousePosWorld = Raylib.GetMousePosition().ToAetherVector2().ScreenToWorld();

        if (ImGui.GetIO().WantCaptureKeyboard)
            _rotation.SetTarget(null);
        else
            _shortcuts.Handle(MousePosWorld, _selection.Get());

        _rotation.SetTarget(_selection.Get()?.Body);
        _rotation.Tick();
    }
    public void TickMouse()
    {
        if (_ui.AnyPopupsOpen() || ImGui.GetIO().WantCaptureMouse)
        {
            _drag.End();
            return;
        }

        if (Raylib.IsMouseButtonPressed(MouseButton.Left) || Raylib.IsMouseButtonPressed(MouseButton.Right))
        {
            _selection.Pick(_world, MousePosWorld);

            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                _drag.TryBegin(_selection.Get(), MousePosWorld);
            }
        }
        else if (_drag.IsActive && Raylib.IsMouseButtonDown(MouseButton.Left))
        {
            _drag.Update(MousePosWorld);
        }
        else if (_drag.IsActive && Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            _drag.End();
        }
    }

    private void OpenQuickSpawnMenu(AethVec2 pos) => _ui.ShowPopup("##quick_spawn", Raylib.GetMousePosition(), () => ImGuiStaticPopups.DrawQuickSpawnMenu(_world, pos, new(), _objs));
}