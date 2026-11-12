using ImGuiNET;
using Physhicsh.Core;
using Physhicsh.Physics.PhysicsObjects;
using Raylib_cs;

namespace Physhicsh.UI.ImGuiWindow;

public class DebugWindow(Common c) : IImGuiWindow
{
    private readonly Common _c = c;

    public void Draw()
    {
        if (!_c.ImgVault.IsOpen(ImGuiWindowState.Debug)) return;

        _c.UI.BeginWindow("##debug", ImGuiWindowFlags.NoTitleBar, () =>
        {
            ImGui.Text($"FPS: {Raylib.GetFPS().ToString()}");
            ImGui.Text($"Frametime: {Raylib.GetFrameTime().ToString()}");
            ImGui.Text($"Count: {_c.PhysicsObjects.Count}");
            ImGui.Text($"Physics: {_c.Profiler["PHYSICS"]}ms, Update: {_c.Profiler["UPDATE"]}ms");
            foreach (PhysicsObject item in _c.PhysicsObjects)
            {
                ImGui.Text($"{item} Position: {item.Body.Position}");
            }
            ImGui.Separator();

            if (_c.PickedObject is not null)
            {
                ImGui.Text($"Picked: {_c.PickedObject}");
                ImGui.Text($"Holding Position: {_c.PickedObject.Body.Position}");
                ImGui.Text($"BodyType: {_c.PickedObject.Body.BodyType}");
                ImGui.Text($"IsBullet: {_c.PickedObject.Body.IsBullet}");
            }

            if (_c.PropertiesObject is not null)
            {
                ImGui.Text($"Properties object: {_c.PropertiesObject}");
            }
        });
    }
}
