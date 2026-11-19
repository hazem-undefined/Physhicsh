using ImGuiNET;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Common;
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

    private readonly CoreContext _coreContext = new();
    private readonly EditorState _editorState = new();
    private readonly ImGuiContext _imGuiContext = new();
    private readonly ObjectRegistry _objectReg = new();
    private SceneIO _sceneIO;

    private readonly SceneSettings _sceneSettings = new();
    private readonly bool _vulture = true; // Cause it will never be false

    public void Start()
    {
        _coreContext.World.Init(_sceneSettings.GravityScale);
        _sceneIO = new(_coreContext.World.CurrentWorld, _editorState, _objectReg);
        _imGuiWindows = [
            new DebugWindow(_coreContext, _imGuiContext, _objectReg, _editorState),
            new PropertiesWindow(_imGuiContext, _editorState),
            new SettingsWindow(_coreContext, _imGuiContext, _sceneIO)];
        Raylib.SetConfigFlags(ConfigFlags.VSyncHint | ConfigFlags.Msaa4xHint);
        Raylib.InitWindow(SystemConfig.Instance.Graphics.ScreenWidth, SystemConfig.Instance.Graphics.ScreenHeight, "Physhicsh");
        Raylib.SetWindowFocused();
        Raylib.SetTargetFPS(SystemConfig.Instance.Graphics.MaxFPS);

        InitializeImGui();

        _input = new(_editorState, _coreContext, _objectReg, _imGuiContext, _sceneIO);

        _sceneIO.LoadScene("demo");

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

            if (File.Exists(_imGuiContext.ImgVault.IMGUI_FONT_PATH))
            {
                font = io.Fonts.AddFontFromFileTTF(_imGuiContext.ImgVault.IMGUI_FONT_PATH, 18.0f);

                _imGuiContext.UI = new(font);
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
                        _imGuiContext.UI = new(io.Fonts.AddFontFromFileTTF(
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
        _objectReg.PhysicsObjects.Add(new CirclePhysicsObject(0, 10, 0.5f, Color.Green));
        _objectReg.PhysicsObjects.Add(new RectanglePhysicsObject(0, 0, 1, 1, Color.DarkGray, 1));
        _objectReg.PhysicsObjects.Add(new RectanglePhysicsObject(5, 0, 1, 1, Color.DarkGray, 1));
        _objectReg.PhysicsObjects.Add(new RectanglePhysicsObject(0, -5, 10, 2, Color.SkyBlue, bodyType: BodyType.Static));
        _objectReg.PhysicsObjects.Add(new RectanglePhysicsObject(0, -10, 100, 2, Color.SkyBlue, bodyType: BodyType.Static));
        _objectReg.PhysicsObjects.Add(new RectanglePhysicsObject(20, -10, 1, 100, Color.SkyBlue, bodyType: BodyType.Static));
        _objectReg.PhysicsObjects.Add(new RectanglePhysicsObject(-20, -10, 1, 100, Color.SkyBlue, bodyType: BodyType.Static));
    }


    private void Update()
    {
        _coreContext.Profiler.Measure("PHYSICS", () => _coreContext.World.StepPhysics(Raylib.GetFrameTime()));
        _coreContext.Profiler.Measure("UPDATE", () => UpdatePhysicsObjects());

        if (Raylib.IsMouseButtonPressed(MouseButton.Right))
        {
            AethVec2 worldPos = _input.MousePosWorld;

            _imGuiContext.UI.ShowPopup("##world_context", Raylib.GetMousePosition(), mb =>
            {
                mb.Item("Settings", () => _imGuiContext.ImgVault.Open(ImGuiWindowState.Settings));

                mb.Separator();

                mb.SubMenu("Quick spawn", (x) => ImGuiStaticPopups.DrawQuickSpawnMenu(_coreContext.World.CurrentWorld, worldPos, x, _objectReg.PhysicsObjects));

                if (_editorState.PickedObject is not null)
                {
                    mb.Separator();
                    mb.Item("Delete", () => _sceneIO.DeletePhysicsObject(_editorState.PickedObject));
                    mb.Item("Properties", () =>
                    {
                        if (_editorState.PickedObject != null)
                            _editorState.PropertiesObject = _editorState.PickedObject;
                        _imGuiContext.ImgVault.Open(ImGuiWindowState.Properties);
                    });
                }

            });
        }

        foreach (PhysicsObject item in _objectReg.PhysicsObjects.ToArray())
        {
            if (IsOutOfBounds(item))
            {
                _sceneIO.DeletePhysicsObject(item);
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
        Raylib.ClearBackground(_coreContext.SceneSettings.CurrentBgColor);

        rlImGui.Begin();

        _imGuiContext.UI.BeginFrame();

        foreach (IImGuiWindow window in _imGuiWindows)
        {
            window.Draw();
        }

        if (_imGuiContext.ShowDebugWindow)
        {
            _imGuiContext.ImgVault.Open(ImGuiWindowState.Debug);
        }
        else
        {
            _imGuiContext.ImgVault.Close(ImGuiWindowState.Debug);
        }

        _imGuiContext.UI.EndFrame();

        DrawPhysicsObjects();

        if (_input != null && _editorState.PickedObject != null)
        {
            _input.DrawSelectionLine();
        }

        rlImGui.End();
        Raylib.EndDrawing();
    }

    private void UpdatePhysicsObjects()
    {
        foreach (PhysicsObject item in _objectReg.PhysicsObjects)
        {
            item.PhysicsUpdate(_coreContext.World.CurrentWorld);
            item.Gay = SystemConfig.Instance.Graphics.Gay;
        }
    }

    private void DrawPhysicsObjects()
    {
        foreach (PhysicsObject obj in _objectReg.PhysicsObjects)
        {
            obj.Draw();
            if (obj == _editorState.PickedObject && !obj.Locked && SystemConfig.Instance.Graphics.ShowOutline)
            {
                obj.DrawOutline();
            }
        }
    }

}