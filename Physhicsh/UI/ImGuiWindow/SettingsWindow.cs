using ImGuiNET;
using NativeFileDialogSharp;
using Physhicsh.Common;
using Physhicsh.Config;
using Physhicsh.Extensions;
using Physhicsh.Physics.Spawning;
using Physhicsh.Utils;
using Raylib_cs;
using System;
using System.IO;

namespace Physhicsh.UI.ImGuiWindow;

public class SettingsWindow(CoreContext c, ImGuiContext imgContx, SceneIO sc) : IImGuiWindow
{
    private readonly CoreContext _c = c;
    private readonly ImGuiContext _img = imgContx;
    private readonly SceneIO _sc = sc;
    private bool _graphicsNeedsRestart;

    public void Draw()
    {
        if (!_img.ImgVault.IsOpen(ImGuiWindowState.Settings)) return;

        _img.UI.BeginWindow("Settings", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.Modal, () =>
        {
            if (ImGui.SmallButton("X"))
            {
                _img.ImgVault.Close(ImGuiWindowState.Settings);
                return;
            }
            ImGui.SameLine();
            ImGui.SmallButton("?");
            if (ImGui.IsItemHovered(ImGuiHoveredFlags.DelayNone))
            {
                ImGui.SetTooltip("Ctrl + Left Click a slider to type an exact value.");
            }

            if (ImGui.Button("Save Config"))
            {
                SystemConfig.SerializeSystemConfig(SystemConfig.CONFIG_FILE_PATH, SystemConfig.Instance);
            }
            ImGui.SameLine();
            if (ImGui.Button("Reload Config"))
            {
                SystemConfig.LoadFromFile(SystemConfig.CONFIG_FILE_PATH);
            }

            DrawConfigSection("Scene", DrawSceneConfig);
            DrawConfigSection("Spawn", DrawSpawnSettings);
            DrawConfigSection("Graphics", SystemConfig.Instance.Graphics, DrawGraphicsConfig);
            DrawConfigSection("Controls", SystemConfig.Instance.Controls, DrawControlsConfig);
            DrawConfigSection("Limits", SystemConfig.Instance.Limits, DrawLimitsConfig);

            if (ImGui.CollapsingHeader("VERY DANGEROUS !!!"))
            {
                ImGui.TextColored(new(255, 0, 0, 255), "All buttons here doesn't have confirmation.");
                if (ImGui.Button("Clear All"))
                {
                    _sc.ClearAllPhysObjects();
                }
            }

            if (ImGui.Button("Restore default"))
            {
                SystemConfig.RestoreToDefault();
            }
        });
    }

    private void DrawSceneConfig()
    {
        if (ImGui.Button("Save Scene"))
        {
            DialogResult savePath = Dialog.FileSave("psf", Path.Combine(AppContext.BaseDirectory, SystemConfig.Instance.Paths.ScenesPath));
            if (savePath.Path != null)
            {
                _sc.SaveScene(savePath.Path);
            }
        }
        ImGui.SameLine();
        if (ImGui.Button("Select Scene"))
        {
            DialogResult selected = Dialog.FileOpen("psf", Path.Combine(AppContext.BaseDirectory, SystemConfig.Instance.Paths.ScenesPath));
            if (selected.Path != null)
            {
                _sc.LoadScene(selected.Path);
            }
        }
        ImGui.SameLine();
        ImGui.TextColored(Color.Green.ToVector4(), $"{_sc.SettingsImportExportMsg}");

        ImGuiUtils.DrawColorEdit3("Background Color", _c.SceneSettings.CurrentBgColor.ToSysVector3(), (x) => _c.SceneSettings.CurrentBgColor = x.ToColor());
        ImGuiUtils.DrawSliderInt("Gravity scale", (int)_c.SceneSettings.GravityScale, -SystemConfig.Instance.Limits.GravityScaleLimit, SystemConfig.Instance.Limits.GravityScaleLimit, (x) =>
        {
            _c.SceneSettings.GravityScale = x;
            _c.World.CurrentWorld.Gravity = new(0, -9.81f * _c.SceneSettings.GravityScale);
        });
        ImGuiUtils.DrawCheckbox("GAY", SystemConfig.Instance.Graphics.Gay, (x) => SystemConfig.Instance.Graphics.Gay = x);
        if (SystemConfig.Instance.Graphics.Gay)
        {
            ImGuiUtils.DrawSliderFloat("Gay Speed", SystemConfig.Instance.Graphics.GaySpeed, 0.02f, 5f, v => SystemConfig.Instance.Graphics.GaySpeed = v);
        }
    }

    private static void DrawSpawnSettings()
    {
        SysVec2 circleLimit = SystemConfig.Instance.Limits.CircleSpawnRadiusLimit;
        SysVec2 rectLimit = SystemConfig.Instance.Limits.RectangleSpawnSizeLimit;
        ImGuiUtils.DrawSliderFloat2("Circle radius", SpawnRules.Instance.CirclesRadius, circleLimit.X, circleLimit.Y, (x) => SpawnRules.Instance.CirclesRadius = x);
        ImGuiUtils.DrawSliderFloat2("Rectangles size", SpawnRules.Instance.RectSize, rectLimit.X, rectLimit.Y, (x) => SpawnRules.Instance.RectSize = x);
        ImGuiUtils.DrawCheckbox("Random Colors", SpawnRules.Instance.UseRandomColors, (x) => SpawnRules.Instance.UseRandomColors = x);
        if (!SpawnRules.Instance.UseRandomColors) ImGuiUtils.DrawColorEdit3("Spawn Color", SpawnRules.Instance.SpawnColor.ToSysVector3(), (x) => SpawnRules.Instance.SpawnColor = x.ToAetherVector3().ToColor());
    }

    // ---------- Beware: Junk underneath ----------

    private static void DrawConfigSection<T>(string name, T config, Action<T> drawFields, string? tooltip = null)
    {
        if (ImGui.CollapsingHeader(name))
        {
            ImGui.Indent();
            drawFields(config);
            ImGui.Unindent();
        }
    }
    private static void DrawConfigSection(string name, Action drawFields, string? tooltip = null)
    {
        if (ImGui.CollapsingHeader(name))
        {
            ImGui.Indent();
            drawFields();
            ImGui.Unindent();
        }

        ImGuiUtils.Tooltip(tooltip);
    }

    private void DrawLimitsConfig(LimitsConfig l)
    {
        ImGui.Text("Controls the limits of the sliders (eg. Gravity scale).");
        ImGui.Spacing();
        ImGuiUtils.DrawSliderFloat2("Circle Spawn Radius Limit", l.CircleSpawnRadiusLimit, 0f, 1000f, v => l.CircleSpawnRadiusLimit = v);
        ImGuiUtils.DrawSliderFloat2("Rectangle Spawn Size Limit", l.RectangleSpawnSizeLimit, 0f, 1000f, v => l.RectangleSpawnSizeLimit = v);
        ImGuiUtils.DrawSliderFloat2("Bounds Limit", l.BoundsLimit, 10f, 5000f, v => l.BoundsLimit = v);
        ImGuiUtils.DrawSliderInt("Gravity Scale Limit", l.GravityScaleLimit, 1, 100, v => l.GravityScaleLimit = v);
    }

    private void DrawGraphicsConfig(GraphicsConfig g)
    {
        bool changed = false;
        changed |= ImGuiUtils.DrawSliderInt("Screen Width", g.ScreenWidth, 320, 3840, v => g.ScreenWidth = v);
        changed |= ImGuiUtils.DrawSliderInt("Screen Height", g.ScreenHeight, 240, 2160, v => g.ScreenHeight = v);
        changed |= ImGuiUtils.DrawCheckbox("Use Vsync", g.UseVsync, v => g.UseVsync = v);
        changed |= ImGuiUtils.DrawSliderInt("Max FPS", g.MaxFPS, 30, 360, v => g.MaxFPS = v);
        changed |= ImGuiUtils.DrawSliderFloat("Pixels Per Meter", g.PPM, 8f, 128f, v => g.PPM = v);

        ImGuiUtils.DrawCheckbox("Show Outline", g.ShowOutline, v => g.ShowOutline = v);

        if (changed) _graphicsNeedsRestart = true;

        if (_graphicsNeedsRestart)
        {
            ImGui.TextColored(new(1f, 0.78f, 0.2f, 1f), "Requires restart");
        }
    }

    private void DrawControlsConfig(ControlsConfig c)
    {
        ImGuiUtils.DrawCheckbox("Auto Select Spawned Object", c.AutoSelectSpawnedObject, v => c.AutoSelectSpawnedObject = v);
        ImGuiUtils.DrawCheckbox("Selects On Hover", c.SelectOnHover, v => c.SelectOnHover = v);
        ImGuiUtils.DrawCheckbox("Dismiss On Blank", c.DismissOnBlank, v => c.DismissOnBlank = v);
        ImGuiUtils.DrawSliderFloat("Rotation Speed", c.RotationSpeed, 0f, 50f, v => c.RotationSpeed = v);
        ImGuiUtils.DrawSliderFloat("Kinematic Stiffness", c.KinematicStiffness, 0f, 100f, v => c.KinematicStiffness = v);
        ImGuiUtils.DrawSliderFloat("Joint Max Force / kg", c.JointMaxForcePerKg, 0f, 5000f, v => c.JointMaxForcePerKg = v);
        ImGuiUtils.DrawSliderFloat("Max Pull", c.MaxPull, 0f, 100f, v => c.MaxPull = v);
        ImGuiUtils.DrawSliderFloat("Max Speed", c.MaxSpeed, 0f, 200f, v => c.MaxSpeed = v);
        ImGuiUtils.DrawSliderFloat("Mass Addition On Pick", c.MassAdditionOnPick, 0f, 500f, v => c.MassAdditionOnPick = v);
        ImGuiUtils.DrawSliderFloat("Joint Frequency", c.JointFrequency, 0f, 50f, v => c.JointFrequency = v);
    }


}