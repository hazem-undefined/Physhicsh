using ImGuiNET;
using Physhicsh.Core;

namespace Physhicsh.UI.ImGuiWindow;

public class PropertiesWindow(Common c) : IImGuiWindow
{
    private readonly Common _c = c;

    public void Draw()
    {
        if (!_c.ImgVault.IsOpen(ImGuiWindowState.Properties)) return;

        _c.UI.BeginWindow("Properties", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar, () =>
        {
            if (ImGui.SmallButton("X"))
            {
                _c.ImgVault.Close(ImGuiWindowState.Properties);
                _c.PropertiesObject = null;
                return;
            }
            if (_c.PropertiesObject is not null)
            {
                _c.PropertiesObject.Properties(_c.UI);
            }
            else
            {
                ImGui.Text("No object.");
            }
        });

    }
}
