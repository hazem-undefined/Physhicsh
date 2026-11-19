using ImGuiNET;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Common;
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
    private AethVec2 _beingDraggedLocalPosition;

    public AethVec2 MousePosWorld { get; private set; }
    public AethVec2 BeginDragPosition { get; private set; }
    private readonly CoreContext _c;
    private readonly ImGuiContext _img;
    private readonly ObjectRegistry _reg;
    private readonly EditorState _ed;

    public InputController(EditorState ed, CoreContext c, ObjectRegistry reg, ImGuiContext img, SceneIO sc)
    {
        _c = c;
        _ed = ed;
        _reg = reg;
        _img = img;
        _selection = new();
        _rotation = new();
        _drag = new(c.World.CurrentWorld);
        _shortcuts = new(c, img, reg, sc, OpenQuickSpawnMenu);
    }

    public void Tick()
    {
        MousePosWorld = Raylib.GetMousePosition().ToAetherVector2().ScreenToWorld();

        bool uiWantsKeyboard = ImGui.GetIO().WantCaptureKeyboard;
        if (!uiWantsKeyboard)
            _shortcuts.Handle(MousePosWorld, _selection.Current);

        _rotation.SetTarget(uiWantsKeyboard ? null : _selection.Current?.Body);
        _rotation.Tick();

        _ed.PickedObject = _selection.Current;
    }

    public void TickMouse()
    {
        if (_img.UI.AnyPopupsOpen() || ImGui.GetIO().WantCaptureMouse)
        {
            _drag.End();
            return;
        }

        bool leftPressed = Raylib.IsMouseButtonPressed(MouseButton.Left);
        bool rightPressed = Raylib.IsMouseButtonPressed(MouseButton.Right);

        if (!_drag.IsActive && (SystemConfig.Instance.Controls.SelectOnHover || leftPressed || rightPressed))
        {
            Fixture? pick = AetherUtils.PickFixture(_c.World.CurrentWorld, MousePosWorld);
            if (pick != null)
            {
                AetherUtils.ApplyFixtureToPicked(ref _selection.Current, pick);
            }
            else if (SystemConfig.Instance.Controls.DismissOnBlank)
            {
                _selection.Pick(_c.World.CurrentWorld, MousePosWorld);
            }
        }

        if (leftPressed)
        {
            AethVec2 pressPos = MousePosWorld;
            _drag.TryBegin(ref _selection.Current, pressPos);

            if (_drag.IsActive)
            {
                BeginDragPosition = pressPos;
                Body? body = _drag.CurrentDrag?.Body;
                _beingDraggedLocalPosition = body?.GetLocalPoint(pressPos) ?? pressPos;
            }
        }
        else if (_drag.IsActive)
        {
            if (Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                _drag.Update(MousePosWorld);
                UpdateDragAnchor();
            }
            else
            {
                _drag.End();
            }
        }
    }

    private void UpdateDragAnchor()
    {
        Body? body = _drag.CurrentDrag?.Body;
        if (body != null)
            BeginDragPosition = body.GetWorldPoint(_beingDraggedLocalPosition);
    }

    public void DrawSelectionLine()
    {
        if (!SystemConfig.Instance.Graphics.ShowDragLine || !_drag.IsActive || _drag.CurrentDrag is not JointDragSession) return;

        RayColor color = SystemConfig.Instance.Graphics.DragLineColor;

        Raylib.DrawLineEx(BeginDragPosition.WorldToScreen().ToSysVec2(),
            MousePosWorld.WorldToScreen().ToSysVec2(),
            SystemConfig.Instance.Graphics.DragLineThickness,
            new(color.R, color.G, color.B, SystemConfig.Instance.Graphics.DragLineTransparency));
    }

    private void OpenQuickSpawnMenu(AethVec2 pos) =>
        _img.UI.ShowPopup("##quick_spawn", Raylib.GetMousePosition(),
            () => ImGuiStaticPopups.DrawQuickSpawnMenu(_c.World.CurrentWorld, pos, new(), _reg.PhysicsObjects));
}