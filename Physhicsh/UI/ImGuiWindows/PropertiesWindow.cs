using ImGuiNET;
using Physhicsh.Common;

namespace Physhicsh.UI.ImGuiWindows;

public class PropertiesWindow(ImGuiContext imgContx, EditorState ed) : IImGuiWindow
{
    private readonly ImGuiContext _img = imgContx;
    private readonly EditorState _ed = ed;

    public void Draw()
    {
        if (!_img.ImgVault.IsOpen(ImGuiWindowState.Properties)) return;

        _img.UI.BeginWindow("Properties", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar, () =>
        {
            if (ImGui.SmallButton("X"))
            {
                _img.ImgVault.Close(ImGuiWindowState.Properties);
                _ed.PropertiesObject = null;
                return;
            }
            if (_ed.PropertiesObject is not null)
            {
                _ed.PropertiesObject.Properties(_img.UI);
            }
            else
            {
                ImGui.Text("No object.");
            }
        });

    }
}
