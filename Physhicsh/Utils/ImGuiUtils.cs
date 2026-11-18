using ImGuiNET;
using System;

namespace Physhicsh.Utils;

public static class ImGuiUtils
{
    public static bool DrawSliderInt(string label, int value, int min, int max, Action<int> setter)
    {
        int v = value;
        if (ImGui.SliderInt(label, ref v, min, max)) { setter(v); return true; }
        return false;
    }

    public static bool DrawSliderFloat(string label, float value, float min, float max, Action<float> setter)
    {
        float v = value;
        if (ImGui.SliderFloat(label, ref v, min, max)) { setter(v); return true; }
        return false;
    }

    public static bool DrawSliderFloat2(string label, SysVec2 value, float min, float max, Action<SysVec2> setter)
    {
        SysVec2 v = value;
        if (ImGui.SliderFloat2(label, ref v, min, max)) { setter(v); return true; }
        return false;
    }

    public static bool DrawCheckbox(string label, bool value, Action<bool> setter)
    {
        bool v = value;
        if (ImGui.Checkbox(label, ref v)) { setter(v); return true; }
        return false;
    }

    public static bool DrawColorEdit3(string label, ImColor value, Action<ImColor> setter)
    {
        ImColor v = value;
        if (ImGui.ColorEdit3(label, ref v)) { setter(v); return true; }
        return false;
    }
}
