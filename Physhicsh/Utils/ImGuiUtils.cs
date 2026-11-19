using ImGuiNET;
using System;

namespace Physhicsh.Utils;

public static class ImGuiUtils
{
    public delegate bool RefFunc<T>(ref T value);

    private static bool Draw<T>(T value, Action<T> setter, RefFunc<T> imgui, string? tooltip = null)
    {
        bool changed = imgui(ref value);

        Tooltip(tooltip);

        if (changed) { setter(value); return true; }
        return false;
    }

    public static bool DrawSliderInt(string label, int value, int min, int max, Action<int> setter, string? tooltip = null)
    {
        bool Call(ref int v) => ImGui.SliderInt(label, ref v, min, max);
        return Draw(value, setter, Call, tooltip);
    }

    public static bool DrawSliderFloat(string label, float value, float min, float max, Action<float> setter, string? tooltip = null)
    {
        bool Call(ref float v) => ImGui.SliderFloat(label, ref v, min, max);
        return Draw(value, setter, Call, tooltip);
    }

    public static bool DrawSliderFloat2(string label, SysVec2 value, float min, float max, Action<SysVec2> setter, string? tooltip = null)
    {
        bool Call(ref SysVec2 v) => ImGui.SliderFloat2(label, ref v, min, max);
        return Draw(value, setter, Call, tooltip);
    }

    public static bool DrawCheckbox(string label, bool value, Action<bool> setter, string? tooltip = null)
    {
        bool Call(ref bool v) => ImGui.Checkbox(label, ref v);
        return Draw(value, setter, Call, tooltip);
    }

    public static bool DrawColorEdit3(string label, ImColor value, Action<ImColor> setter, string? tooltip = null)
    {
        bool Call(ref ImColor v) => ImGui.ColorEdit3(label, ref v);
        return Draw(value, setter, Call, tooltip);
    }


    public static void Tooltip(string? tooltip = null)
    {
        if (tooltip != null && ImGui.IsItemHovered(ImGuiHoveredFlags.DelayNone))
            ImGui.SetTooltip(tooltip);
    }
}
