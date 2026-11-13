using ImGuiNET;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Config;
using Physhicsh.Core.Input;
using Physhicsh.Physics.PhysicsObjects;
using Physhicsh.Scenes;
using Physhicsh.UI;
using Physhicsh.UI.ImGuiWindow;
using Raylib_cs;
using rlImGui_cs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Physhicsh.Core;

public class MainWindow
{
    private List<IImGuiWindow> _imGuiWindows = [];
    private InputController? _input;
    private SceneController? _sceneController;
    private Common _c = null!;
    private readonly SceneSettings _sceneSettings = new();
    private readonly bool _vulture = true; // Cause it will never be false

    public void Start()
    {
        _c = new();

        _sceneController = new(_c);

        _imGuiWindows = [new DebugWindow(_c), new PropertiesWindow(_c), new SettingsWindow(_c)];
        _c.World.Init(_sceneSettings.GravityScale);

        Raylib.SetConfigFlags(ConfigFlags.VSyncHint | ConfigFlags.Msaa4xHint);
        Raylib.InitWindow(SystemConfig.Instance.Screen.ScreenWidth, SystemConfig.Instance.Screen.ScreenHeight, "Physhicsh");
        Raylib.SetWindowFocused();
        Raylib.SetTargetFPS(SystemConfig.Instance.Screen.MaxFPS);

        InitializeImGui();

        _input = new(_c);

        _sceneController.LoadScene("demo");

        while (!Raylib.WindowShouldClose())
        {
            Update();
            GraphicsUpdate();
        }

        Raylib.CloseWindow();
    }

    private void InitializeImGui()
    {
        ImFontPtr font = null;

        // Try to load Arial, if doesn't exist, load whatever font in there
        rlImGui.SetupUserFonts += io =>
        {

            if (File.Exists(_c.ImgVault.IMGUI_FONT_PATH))
            {
                font = io.Fonts.AddFontFromFileTTF(_c.ImgVault.IMGUI_FONT_PATH, 18.0f);

                _c.UI = new(font);
            }
            else
            {
                if (!Directory.Exists("fonts")) throw new FileNotFoundException("No fonts folder were found");

                IEnumerable<string> fontsDir = Directory.EnumerateFiles("fonts");

                if (!fontsDir.Any()) throw new FileNotFoundException("Empty fonts folder");

                for (int i = 0; i < fontsDir.Count(); i++)
                {
                    string foundFile = fontsDir.ElementAt(i);
                    if (foundFile.EndsWith(".ttf"))
                    {
                        _c.UI = new(io.Fonts.AddFontFromFileTTF(
                        foundFile,
                        18.0f));
                        break;
                    }
                }


            }
        };

        rlImGui.Setup(true);
    }

    // :)
    private void SpawnDemoObjects()
    {
        _c.PhysicsObjects.Add(new CirclePhysicsObject(0, 10, 0.5f, Color.Green));
        _c.PhysicsObjects.Add(new RectanglePhysicsObject(0, 0, 1, 1, Color.DarkGray, 1));
        _c.PhysicsObjects.Add(new RectanglePhysicsObject(5, 0, 1, 1, Color.DarkGray, 1));
        _c.PhysicsObjects.Add(new RectanglePhysicsObject(0, -5, 10, 2, Color.SkyBlue, bodyType: BodyType.Static));
        _c.PhysicsObjects.Add(new RectanglePhysicsObject(0, -10, 100, 2, Color.SkyBlue, bodyType: BodyType.Static));
        _c.PhysicsObjects.Add(new RectanglePhysicsObject(20, -10, 1, 100, Color.SkyBlue, bodyType: BodyType.Static));
        _c.PhysicsObjects.Add(new RectanglePhysicsObject(-20, -10, 1, 100, Color.SkyBlue, bodyType: BodyType.Static));
    }


    private void Update()
    {
        _c.Profiler.Measure("PHYSICS", () => _c.World.StepPhysics(Raylib.GetFrameTime()));
        _c.Profiler.Measure("UPDATE", () => UpdatePhysicsObjects());

        if (Raylib.IsMouseButtonPressed(MouseButton.Right))
        {
            AethVec2 worldPos = _input.MousePosWorld;

            _c.UI.ShowPopup("##world_context", Raylib.GetMousePosition(), mb =>
            {
                mb.Item("Settings", () => _c.ImgVault.Open(ImGuiWindowState.Settings));

                mb.Separator();

                mb.SubMenu("Quick spawn", (x) => ImGuiStaticPopups.DrawQuickSpawnMenu(_c.World.CurrentWorld, worldPos, x, _c.PhysicsObjects));

                if (_c.PickedObject is not null)
                {
                    mb.Separator();
                    mb.Item("Delete", () => _c.DeletePhysicsObject(_c.PickedObject));
                    mb.Item("Properties", () =>
                    {
                        if (_c.PickedObject != null)
                            _c.PropertiesObject = _c.PickedObject;
                        _c.ImgVault.Open(ImGuiWindowState.Properties);
                    });
                }

            });
        }

        foreach (PhysicsObject item in _c.PhysicsObjects.ToArray())
        {
            if (IsOutOfBounds(item))
            {
                _c.DeletePhysicsObject(item);
            }
        }

        _input.Tick();
        _input.TickMouse();
    }

    private static bool IsOutOfBounds(PhysicsObject item)
    {
        return MathF.Abs(item.Body.Position.X) > SystemConfig.Instance.Limits.BoundsLimit.X || MathF.Abs(item.Body.Position.Y) > SystemConfig.Instance.Limits.BoundsLimit.Y;
    }

    private void GraphicsUpdate()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(_c.SceneSettings.CurrentBgColor);

        rlImGui.Begin();

        _c.UI.BeginFrame();

        foreach (IImGuiWindow window in _imGuiWindows)
        {
            window.Draw();
        }

        if (_c.ShowDebugWindow)
        {
            _c.ImgVault.Open(ImGuiWindowState.Debug);
        }
        else
        {
            _c.ImgVault.Close(ImGuiWindowState.Debug);
        }

        _c.UI.EndFrame();

        DrawPhysicsObjects();

        rlImGui.End();
        Raylib.EndDrawing();
    }

    private void UpdatePhysicsObjects()
    {
        foreach (PhysicsObject item in _c.PhysicsObjects)
            item.PhysicsUpdate(_c.World.CurrentWorld);
    }

    private void DrawPhysicsObjects()
    {
        foreach (PhysicsObject obj in _c.PhysicsObjects)
        {
            obj.Draw();
            if (obj == _c.PickedObject && !obj.Locked)
            {
                obj.DrawOutline();
            }
        }
    }

}