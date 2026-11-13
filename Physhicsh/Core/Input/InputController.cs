using ImGuiNET;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Config;
using Physhicsh.Core.Input.Drag;
using Physhicsh.Extensions;
using Physhicsh.UI;
using Physhicsh.Utils;
using Raylib_cs;

namespace Physhicsh.Core.Input;

public sealed class InputController
{
    private readonly Selection _selection;
    private readonly DragController _drag;
    private readonly ShortcutHandler _shortcuts;
    private readonly RotationController _rotation;
    public AethVec2 MousePosWorld { get; private set; }
    private readonly Common _c;
    public InputController(Common c)
    {
        _c = c;
        _selection = new();
        _rotation = new();
        _drag = new(c.World.CurrentWorld);
        _shortcuts = new(_c, pos => OpenQuickSpawnMenu(pos));
    }

    public void Tick()
    {
        MousePosWorld = Raylib.GetMousePosition().ToAetherVector2().ScreenToWorld();

        if (ImGui.GetIO().WantCaptureKeyboard)
            _rotation.SetTarget(null);
        else
            _shortcuts.Handle(MousePosWorld, _selection.Current);

        if (_selection.Current?.Body != null)
            _rotation.SetTarget(_selection.Current?.Body);

        _rotation.Tick();

        _c.PickedObject = _selection.Current;
    }
    public void TickMouse()
    {
        if (_c.UI.AnyPopupsOpen() || ImGui.GetIO().WantCaptureMouse)
        {
            _drag.End();
            return;
        }

        if (SystemConfig.Instance.Controls.SelectOnHover || Raylib.IsMouseButtonPressed(MouseButton.Left) || Raylib.IsMouseButtonPressed(MouseButton.Right))
        {
            Fixture? pick = AetherUtils.PickFixture(_c.World.CurrentWorld, MousePosWorld);
            if (pick != null)
            {
                AetherUtils.ApplyFixtureToPicked(ref _selection.Current, pick);
            }
            else if (pick == null && SystemConfig.Instance.Controls.DismissOnBlank)
            {
                _selection.Pick(_c.World.CurrentWorld, MousePosWorld);
            }
        }

        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            _drag.TryBegin(ref _selection.Current, MousePosWorld);
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

    private void OpenQuickSpawnMenu(AethVec2 pos) => _c.UI.ShowPopup("##quick_spawn", Raylib.GetMousePosition(), () => ImGuiStaticPopups.DrawQuickSpawnMenu(_c.World.CurrentWorld, pos, new(), _c.PhysicsObjects));
}