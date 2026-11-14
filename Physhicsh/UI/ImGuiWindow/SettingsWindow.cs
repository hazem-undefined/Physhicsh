using ImGuiNET;
using NativeFileDialogSharp;
using Physhicsh.Config;
using Physhicsh.Core;
using Physhicsh.Extensions;
using Physhicsh.Physics.Spawning;
using Raylib_cs;
using System;
using System.IO;

namespace Physhicsh.UI.ImGuiWindow;

public class SettingsWindow(Common c) : IImGuiWindow
{
    private readonly Common _c = c;
    private bool _graphicsNeedsRestart;

    public void Draw()
    {
        if (!_c.ImgVault.IsOpen(ImGuiWindowState.Settings)) return;

        _c.UI.BeginWindow("Settings", ImGuiWindowFlags.NoCollapse, () =>
        {
            if (ImGui.SmallButton("X"))
            {
                _c.ImgVault.Close(ImGuiWindowState.Settings);
                return;
            }

            if (ImGui.Button("Save Scene"))
            {
                DialogResult savePath = Dialog.FileSave("psf", Path.Combine(AppContext.BaseDirectory, SystemConfig.Instance.Paths.ScenesPath));
                if (savePath.Path != null)
                {
                    //_sceneController.SaveScene(savePath.Path);
                }
            }
            ImGui.SameLine();
            if (ImGui.Button("Select Scene"))
            {
                DialogResult selected = Dialog.FileOpen("psf", Path.Combine(AppContext.BaseDirectory, SystemConfig.Instance.Paths.ScenesPath));
                if (selected.Path != null)
                {
                    //_sceneController.LoadScene(selected.Path);
                }
            }
            ImGui.SameLine();
            ImGui.TextColored(Color.Green.ToVector4(), $"{_c.SettingsImportExportMsg}");

            ImColor currentBgColor = _c.SceneSettings.CurrentBgColor.ToSysVector3();
            if (ImGui.ColorEdit3("Background Color", ref currentBgColor))
            {
                _c.SceneSettings.CurrentBgColor = currentBgColor.ToColor();
            }

            int gravityRefSlider = (int)_c.SceneSettings.GravityScale;
            if (ImGui.SliderInt("Gravity scale", ref gravityRefSlider, -SystemConfig.Instance.Limits.GravityScaleLimit, SystemConfig.Instance.Limits.GravityScaleLimit))
            {
                _c.SceneSettings.GravityScale = gravityRefSlider;
                _c.World.CurrentWorld.Gravity = new(0, -9.81f * _c.SceneSettings.GravityScale);
            }

            DrawSpawnSettings();
            DrawConfigSection("Graphics", SystemConfig.Instance.Graphics, DrawGraphicsConfig);
            DrawConfigSection("Controls", SystemConfig.Instance.Controls, DrawControlsConfig);
            DrawConfigSection("Limits", SystemConfig.Instance.Limits, DrawLimitsConfig);

            if (ImGui.CollapsingHeader("VERY DANGEROUS !!!"))
            {
                ImGui.TextColored(new(255, 0, 0, 255), "All buttons here doesn't have confirmation.");
                if (ImGui.Button("Clear All"))
                {
                    _c.ClearAllPhysObjects();
                }
            }
        });
    }

    private static void DrawSpawnSettings()
    {
        if (ImGui.CollapsingHeader("Spawn"))
        {
            SysVec2 circlesRadiusRefSlider = SpawnRules.Instance.CirclesRadius;
            SysVec2 rectanglesSizeRefSlider = SpawnRules.Instance.RectSize;
            bool useRandomColorsRefBool = SpawnRules.Instance.UseRandomColors;
            ImColor spawnColorRefColor = SpawnRules.Instance.SpawnColor.ToSysVector3();
            SysVec2 circleLimit = SystemConfig.Instance.Limits.CircleSpawnRadiusLimit;
            SysVec2 rectLimit = SystemConfig.Instance.Limits.RectangleSpawnSizeLimit;

            if (ImGui.SliderFloat2("Circles radius", ref circlesRadiusRefSlider, circleLimit.X, circleLimit.Y))
            {
                SpawnRules.Instance.CirclesRadius = circlesRadiusRefSlider;
            }

            if (ImGui.SliderFloat2("Rectangles size", ref rectanglesSizeRefSlider, rectLimit.X, rectLimit.Y))
            {
                SpawnRules.Instance.RectSize = rectanglesSizeRefSlider;
            }

            if (ImGui.Checkbox("Random Colors", ref useRandomColorsRefBool))
            {
                SpawnRules.Instance.UseRandomColors = useRandomColorsRefBool;
            }

            if (!SpawnRules.Instance.UseRandomColors && ImGui.ColorEdit3("Spawn Color", ref spawnColorRefColor))
            {
                SpawnRules.Instance.SpawnColor = spawnColorRefColor.ToAetherVector3().ToColor();
            }
        }
    }

    // ---------- Config sections ----------

    private static void DrawConfigSection<T>(string name, T config, Action<T> drawFields)
    {
        if (ImGui.CollapsingHeader(name))
        {
            ImGui.Indent();
            drawFields(config);
            ImGui.Unindent();
        }
    }

    private void DrawLimitsConfig(LimitsConfig l)
    {
        ImGui.Text("Controls the limits of the sliders (eg. Gravity scale).");
        ImGui.Spacing();
        DrawSliderFloat2("Circle Spawn Radius Limit", l.CircleSpawnRadiusLimit, 0f, 10f, v => l.CircleSpawnRadiusLimit = v);
        DrawSliderFloat2("Rectangle Spawn Size Limit", l.RectangleSpawnSizeLimit, 0f, 10f, v => l.RectangleSpawnSizeLimit = v);
        DrawSliderFloat2("Bounds Limit", l.BoundsLimit, 10f, 500f, v => l.BoundsLimit = v);
        DrawSliderInt("Gravity Scale Limit", l.GravityScaleLimit, 1, 100, v => l.GravityScaleLimit = v);
    }

    private void DrawGraphicsConfig(GraphicsConfig g)
    {
        bool changed = false;
        changed |= DrawSliderInt("Screen Width", g.ScreenWidth, 320, 3840, v => g.ScreenWidth = v);
        changed |= DrawSliderInt("Screen Height", g.ScreenHeight, 240, 2160, v => g.ScreenHeight = v);
        changed |= DrawCheckbox("Use Vsync", g.UseVsync, v => g.UseVsync = v);
        changed |= DrawSliderInt("Max FPS", g.MaxFPS, 30, 360, v => g.MaxFPS = v);
        changed |= DrawSliderFloat("Pixels Per Meter", g.PPM, 8f, 128f, v => g.PPM = v);

        // Show Outline does NOT require restart
        DrawCheckbox("Show Outline", g.ShowOutline, v => g.ShowOutline = v);

        if (changed) _graphicsNeedsRestart = true;

        if (_graphicsNeedsRestart)
        {
            ImGui.TextColored(new(1f, 0.78f, 0.2f, 1f), "Requires restart");
        }
    }

    private void DrawControlsConfig(ControlsConfig c)
    {
        DrawCheckbox("Auto Select Spawned Object", c.AutoSelectSpawnedObject, v => c.AutoSelectSpawnedObject = v);
        DrawCheckbox("Selects On Hover", c.SelectOnHover, v => c.SelectOnHover = v);
        DrawCheckbox("Dismiss On Blank", c.DismissOnBlank, v => c.DismissOnBlank = v);
        DrawSliderFloat("Rotation Speed", c.RotationSpeed, 0f, 50f, v => c.RotationSpeed = v);
        DrawSliderFloat("Kinematic Stiffness", c.KinematicStiffness, 0f, 100f, v => c.KinematicStiffness = v);
        DrawSliderFloat("Joint Max Force / kg", c.JointMaxForcePerKg, 0f, 5000f, v => c.JointMaxForcePerKg = v);
        DrawSliderFloat("Max Pull", c.MaxPull, 0f, 100f, v => c.MaxPull = v);
        DrawSliderFloat("Max Speed", c.MaxSpeed, 0f, 200f, v => c.MaxSpeed = v);
        DrawSliderFloat("Mass Addition On Pick", c.MassAdditionOnPick, 0f, 500f, v => c.MassAdditionOnPick = v);
        DrawSliderFloat("Joint Frequency", c.JointFrequency, 0f, 50f, v => c.JointFrequency = v);
    }

    private static bool DrawSliderInt(string label, int value, int min, int max, Action<int> setter)
    {
        int v = value;
        if (ImGui.SliderInt(label, ref v, min, max)) { setter(v); return true; }
        return false;
    }

    private static bool DrawSliderFloat(string label, float value, float min, float max, Action<float> setter)
    {
        float v = value;
        if (ImGui.SliderFloat(label, ref v, min, max)) { setter(v); return true; }
        return false;
    }

    private static bool DrawSliderFloat2(string label, SysVec2 value, float min, float max, Action<SysVec2> setter)
    {
        SysVec2 v = value;
        if (ImGui.SliderFloat2(label, ref v, min, max)) { setter(v); return true; }
        return false;
    }

    private static bool DrawCheckbox(string label, bool value, Action<bool> setter)
    {
        bool v = value;
        if (ImGui.Checkbox(label, ref v)) { setter(v); return true; }
        return false;
    }
}