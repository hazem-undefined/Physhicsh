using System.Collections.Generic;

namespace Physhicsh.UI;

public enum ImGuiWindowState
{
    None,
    Settings,
    Properties,
    Debug,
}

public class ImGuiVault
{
    private readonly HashSet<ImGuiWindowState> _open = [];
    public readonly string IMGUI_FONT_PATH = "fonts/arial.ttf";
    public bool IsOpen(ImGuiWindowState p) => _open.Contains(p);
    public void Open(ImGuiWindowState p) => _open.Add(p);
    public void Close(ImGuiWindowState p) => _open.Remove(p);
    public void Toggle(ImGuiWindowState p) { if (!_open.Add(p)) _open.Remove(p); }
    public void CloseAll() => _open.Clear();
}
