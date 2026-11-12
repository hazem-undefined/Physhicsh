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

public class SettingsWindow(ref Common c) : IImGuiWindow
{
    private readonly Common _c = c;

    public void Draw(ImGuiUI ui)
    {
        if (!_c.ImgVault.IsOpen(ImGuiWindowState.Settings)) return;

        ui.BeginWindow("Settings", ImGuiWindowFlags.NoCollapse, () =>
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

            if (ImGui.CollapsingHeader("Spawn Settings"))
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
}
