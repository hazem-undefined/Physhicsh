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
namespace Physhicsh.UI.ImGuiWindows;

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
            if (ImGui.IsItemHovered(ImGuiHoveredFlags.DelayNone))
            {
                ImGui.SetTooltip("Write all current settings to the config file on disk.\nRun this before closing or your changes will be lost.");
            }
            ImGui.SameLine();
            if (ImGui.Button("Reload Config"))
            {
                SystemConfig.LoadFromFile(SystemConfig.CONFIG_FILE_PATH);
            }
            if (ImGui.IsItemHovered(ImGuiHoveredFlags.DelayNone))
            {
                ImGui.SetTooltip("Discard unsaved changes and reload settings from the config file.");
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
                if (ImGui.IsItemHovered(ImGuiHoveredFlags.DelayNone))
                {
                    ImGui.SetTooltip("Immediately deletes every physics object in the scene.\nThere is no undo — this cannot be reversed.");
                }
            }

            if (ImGui.Button("Restore default"))
            {
                SystemConfig.RestoreToDefault();
            }
            if (ImGui.IsItemHovered(ImGuiHoveredFlags.DelayNone))
            {
                ImGui.SetTooltip("Reset every setting back to its factory default.\nDoes not save automatically — press Save Config afterwards to keep it.");
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
        if (ImGui.IsItemHovered(ImGuiHoveredFlags.DelayNone))
        {
            ImGui.SetTooltip("Export the current scene (objects, positions, joints) to a .psf file.");
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
        if (ImGui.IsItemHovered(ImGuiHoveredFlags.DelayNone))
        {
            ImGui.SetTooltip("Load a .psf scene file. This replaces everything currently in the scene.");
        }
        ImGui.SameLine();
        ImGui.TextColored(Color.Green.ToVector4(), $"{_sc.SettingsImportExportMsg}");

        ImGuiUtils.DrawColorEdit3("Background Color", _c.SceneSettings.CurrentBgColor.ToSysVector3(), (x) => _c.SceneSettings.CurrentBgColor = x.ToColor(),
            "The backdrop color drawn behind all physics objects.");

        ImGuiUtils.DrawSliderInt("Gravity scale", (int)_c.SceneSettings.GravityScale, -SystemConfig.Instance.Limits.GravityScaleLimit, SystemConfig.Instance.Limits.GravityScaleLimit, (x) =>
        {
            _c.SceneSettings.GravityScale = x;
            _c.World.CurrentWorld.Gravity = new(0, -9.81f * _c.SceneSettings.GravityScale);
        }, "Global gravity multiplier. 0 = zero-G, 1 = normal, negative values flip gravity.\nHow far you can push the slider is set by Limits → Gravity Scale Limit.");

        ImGuiUtils.DrawCheckbox("GAY", SystemConfig.Instance.Graphics.Gay, (x) => SystemConfig.Instance.Graphics.Gay = x,
            "Cycles every object's color through a continuous rainbow animation.");

        if (SystemConfig.Instance.Graphics.Gay)
        {
            ImGuiUtils.DrawSliderFloat("Gay Speed", SystemConfig.Instance.Graphics.GaySpeed, 0.02f, 5f, v => SystemConfig.Instance.Graphics.GaySpeed = v,
                "How quickly the rainbow cycling advances.\nLower = faster color changes, higher = slower and smoother.");
        }
    }

    private static void DrawSpawnSettings()
    {
        SysVec2 circleLimit = SystemConfig.Instance.Limits.CircleSpawnRadiusLimit;
        SysVec2 rectLimit = SystemConfig.Instance.Limits.RectangleSpawnSizeLimit;

        ImGuiUtils.DrawSliderFloat2("Circle radius", SpawnRules.Instance.CirclesRadius, circleLimit.X, circleLimit.Y, (x) => SpawnRules.Instance.CirclesRadius = x,
            "Radius (world units) of newly spawned circles.\nThe min/max of this slider is controlled by Limits → Circle Spawn Radius Limit.");

        ImGuiUtils.DrawSliderFloat2("Rectangles size", SpawnRules.Instance.RectSize, rectLimit.X, rectLimit.Y, (x) => SpawnRules.Instance.RectSize = x,
            "Width (X) and height (Y) of newly spawned rectangles, in world units.\nThe slider range is controlled by Limits → Rectangle Spawn Size Limit.");

        ImGuiUtils.DrawCheckbox("Random Colors", SpawnRules.Instance.UseRandomColors, (x) => SpawnRules.Instance.UseRandomColors = x,
            "Give each spawned object a random color instead of using the fixed Spawn Color below.");

        if (!SpawnRules.Instance.UseRandomColors)
        {
            ImGuiUtils.DrawColorEdit3("Spawn Color", SpawnRules.Instance.SpawnColor.ToSysVector3(), (x) => SpawnRules.Instance.SpawnColor = x.ToAetherVector3().ToColor(),
                "The color applied to every newly spawned object when Random Colors is disabled.");
        }
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

        ImGuiUtils.DrawSliderFloat2("Circle Spawn Radius Limit", l.CircleSpawnRadiusLimit, 0f, 1000f, v => l.CircleSpawnRadiusLimit = v,
            "Min/max radius allowed for circle spawning.\nConstrains the 'Circle radius' slider in the Spawn section.");

        ImGuiUtils.DrawSliderFloat2("Rectangle Spawn Size Limit", l.RectangleSpawnSizeLimit, 0f, 1000f, v => l.RectangleSpawnSizeLimit = v,
            "Min/max width and height allowed for rectangle spawning.\nConstrains the 'Rectangles size' slider in the Spawn section.");

        ImGuiUtils.DrawSliderFloat2("Bounds Limit", l.BoundsLimit, 10f, 5000f, v => l.BoundsLimit = v,
            "Min/max range for the world boundary (wall) dimensions.\nRaise the upper bound if you want bigger play areas.");

        ImGuiUtils.DrawSliderInt("Gravity Scale Limit", l.GravityScaleLimit, 1, 100, v => l.GravityScaleLimit = v,
            "How far the Gravity scale slider in the Scene section can go in either direction (positive value used for ±range).\nIncrease for stronger/reversed-gravity experiments.");
    }

    private void DrawGraphicsConfig(GraphicsConfig g)
    {
        bool changed = false;

        changed |= ImGuiUtils.DrawSliderInt("Screen Width", g.ScreenWidth, 320, 3840, v => g.ScreenWidth = v,
            "Window width in pixels. Changes apply after restarting the application.");

        changed |= ImGuiUtils.DrawSliderInt("Screen Height", g.ScreenHeight, 240, 2160, v => g.ScreenHeight = v,
            "Window height in pixels. Changes apply after restarting the application.");

        changed |= ImGuiUtils.DrawCheckbox("Use Vsync", g.UseVsync, v => g.UseVsync = v,
            "Synchronize frame rendering with your monitor's refresh rate.\nPrevents screen tearing but caps FPS to the display's refresh rate.");

        changed |= ImGuiUtils.DrawSliderInt("Max FPS", g.MaxFPS, 30, 360, v => g.MaxFPS = v,
            "Upper frame rate limit (only effective when Vsync is off).\nLower = less GPU load; higher = smoother motion.");

        changed |= ImGuiUtils.DrawSliderFloat("Pixels Per Meter", g.PPM, 8f, 128f, v => g.PPM = v,
            "Zoom level: how many screen pixels represent one world meter.\nHigher = zoomed in, lower = zoomed out.");

        ImGuiUtils.DrawCheckbox("Show Outline", g.ShowOutline, v => g.ShowOutline = v,
            "Draw an outline around each physics object.\nMakes shapes easier to distinguish against similar-colored backgrounds.");

        if (changed) _graphicsNeedsRestart = true;

        if (_graphicsNeedsRestart)
        {
            ImGui.TextColored(new(1f, 0.78f, 0.2f, 1f), "Requires restart");
        }
    }

    private void DrawControlsConfig(ControlsConfig c)
    {
        ImGuiUtils.DrawCheckbox("Auto Select Spawned Object", c.AutoSelectSpawnedObject, v => c.AutoSelectSpawnedObject = v,
            "Automatically select a newly spawned object so you can grab or edit it right away.");

        ImGuiUtils.DrawCheckbox("Selects On Hover", c.SelectOnHover, v => c.SelectOnHover = v,
            "Select objects just by moving the mouse over them, without clicking.");

        ImGuiUtils.DrawCheckbox("Dismiss On Blank", c.DismissOnBlank, v => c.DismissOnBlank = v,
            "Deselects when the mouse pointer is on an empty space.");

        ImGuiUtils.DrawSliderFloat("Rotation Speed", c.RotationSpeed, 0f, 50f, v => c.RotationSpeed = v,
            "How fast objects rotate when you apply rotation input.");

        ImGuiUtils.DrawSliderFloat("Kinematic Stiffness", c.KinematicStiffness, 0f, 100f, v => c.KinematicStiffness = v);

        ImGuiUtils.DrawSliderFloat("Joint Max Force / kg", c.JointMaxForcePerKg, 0f, 5000f, v => c.JointMaxForcePerKg = v,
            "Maximum force the drag joint can apply per kilogram of object mass.\nHigher = heavier objects get pulled more firmly; lower = springier, easier to break.");

        ImGuiUtils.DrawSliderFloat("Max Pull", c.MaxPull, 0f, 100f, v => c.MaxPull = v,
            "Maximum distance the cursor can pull an object before the joint starts to stretch or snap.");

        ImGuiUtils.DrawSliderFloat("Max Speed", c.MaxSpeed, 0f, 200f, v => c.MaxSpeed = v,
            "Speed cap applied to objects while being dragged.");

        ImGuiUtils.DrawSliderFloat("Mass Addition On Pick", c.MassAdditionOnPick, 0f, 500f, v => c.MassAdditionOnPick = v,
            "Extra virtual mass added to an object while you're dragging it.");

        ImGuiUtils.DrawSliderFloat("Joint Frequency", c.JointFrequency, 0f, 50f, v => c.JointFrequency = v,
            "Spring oscillation frequency of the drag joint.\nHigher = stiffer, springier response; lower = floaty, softer feel.");
    }
}