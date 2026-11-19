using ImGuiNET;
using Physhicsh.Common;
using Raylib_cs;
using System.Collections.Generic;

namespace Physhicsh.UI.ImGuiWindows;

public class DebugWindow(CoreContext c, ImGuiContext imgContx, ObjectRegistry reg, EditorState ed) : IImGuiWindow
{
    private readonly CoreContext _c = c;
    private readonly ImGuiContext _img = imgContx;
    private readonly ObjectRegistry _reg = reg;
    private readonly EditorState _ed = ed;

    public void Draw()
    {
        if (!_img.ImgVault.IsOpen(ImGuiWindowState.Debug)) return;

        _img.UI.BeginWindow("##debug", ImGuiWindowFlags.NoTitleBar, () =>
        {
            ImGui.Text($"FPS: {Raylib.GetFPS().ToString()}");
            ImGui.Text($"Frametime: {Raylib.GetFrameTime().ToString()}");
            ImGui.Text($"Count: {_reg.PhysicsObjects.Count}");
            ImGui.Text($"Physics: {_c.Profiler["PHYSICS"]}ms, Update: {_c.Profiler["UPDATE"]}ms");
            foreach (KeyValuePair<string, object> dtext in _c.Profiler.DebugTexts)
            {
                ImGui.Text($"{dtext.Key}: {dtext.Value}");
            }
            ImGui.Text($"Picked: {_ed.PickedObject}");
            ImGui.Text($"Properties: {_ed.PropertiesObject}");

            ImGui.Separator();

            if (_ed.PickedObject?.Body is not null)
            {
                ImGui.Text($"Picked: {_ed.PickedObject}");
                ImGui.Text($"Holding Position: {_ed.PickedObject.Body.Position}");
                ImGui.Text($"BodyType: {_ed.PickedObject.Body.BodyType}");
                ImGui.Text($"IsBullet: {_ed.PickedObject.Body.IsBullet}");
            }

            if (_ed.PropertiesObject is not null)
            {
                ImGui.Text($"Properties object: {_ed.PropertiesObject}");
            }
        });
    }
}
