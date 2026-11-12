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
    private readonly Random _rng = new();
    private ImGuiUI _ui = null!;
    private List<IImGuiWindow> _imGuiWindows = [];
    private InputController _input;
    private readonly bool _vulture = true; // Cause it will never be false
    private PhysicsWorld _world = new();
    private SceneController _sceneController;
    private Common _c;
    private readonly SceneSettings _sceneSettings = new();

    public void Start()
    {
        _c = new(_ui);
        _sceneController = new(_c);

        _imGuiWindows = [new DebugWindow(_c), new PropertiesWindow(_c), new SettingsWindow(_c)];
        _c.World.Init(_sceneSettings.GravityScale);

        Raylib.SetConfigFlags(ConfigFlags.VSyncHint | ConfigFlags.Msaa4xHint);
        Raylib.InitWindow(SystemConfig.Instance.Screen.ScreenWidth, SystemConfig.Instance.Screen.ScreenHeight, "Physhicsh");
        Raylib.SetWindowFocused();
        Raylib.SetTargetFPS(SystemConfig.Instance.Screen.MaxFPS);

        LoadFont();

        rlImGui.Setup(true);

        _input = new(_c.World.CurrentWorld, _ui, _c.PhysicsObjects, _c.DeletePhysicsObject);

        _sceneController.LoadScene("demo");

        while (!Raylib.WindowShouldClose())
        {
            Update();
            GraphicsUpdate();
        }

        Raylib.CloseWindow();
    }

    private void LoadFont()
    {
        // Try to load Arial, if doesn't exist, load whatever font in there
        rlImGui.SetupUserFonts += io =>
        {
            if (File.Exists(_c.ImgVault.IMGUI_FONT_PATH))
            {
                _ui = new(io.Fonts.AddFontFromFileTTF(
                _c.ImgVault.IMGUI_FONT_PATH,
                18.0f));
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
                        _ui = new(io.Fonts.AddFontFromFileTTF(
                        foundFile,
                        18.0f));
                        break;
                    }
                }
            }

        };
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

            _ui.ShowPopup("##world_context", Raylib.GetMousePosition(), mb =>
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
        _c.PickedObject = _input.PickedObject;

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

        _ui.BeginFrame();

        foreach (IImGuiWindow window in _imGuiWindows)
        {
            window.Draw(_ui);
        }

        if (_input.ShowDebugWindow)
        {
            _c.ImgVault.Open(ImGuiWindowState.Debug);
        }
        else
        {
            _c.ImgVault.Close(ImGuiWindowState.Debug);
        }

        _ui.EndFrame();

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
        }
    }

}