using ImGuiNET;
using Physhicsh.Core;

namespace Physhicsh.UI.ImGuiWindow;

public class PropertiesWindow(ref Common c) : IImGuiWindow
{
    private readonly Common _c = c;

    public void Draw(ImGuiUI ui)
    {
        if (!_c.ImgVault.IsOpen(ImGuiWindowState.Properties)) return;
        _c.PropertiesObject = _c.PickedObject;

        using (ui.UseFont())
        {
            if (ImGui.Begin("Properties", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar))
            {
                if (ImGui.SmallButton("X"))
                {
                    _c.ImgVault.Close(ImGuiWindowState.Properties);
                    _c.PropertiesObject = null;
                    ImGui.End();
                    return;
                }
                if (_c.PropertiesObject is not null)
                {
                    _c.PropertiesObject.Properties(ui);
                }
                else
                {
                    ImGui.Text("No object.");
                }

                ImGui.End();
            }

        }
    }
}
