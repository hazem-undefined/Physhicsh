using ImGuiNET;
using NativeFileDialogSharp;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Config;
using Physhicsh.Debug;
using Physhicsh.Extensions;
using Physhicsh.Physics.PhysicsObjects;
using Physhicsh.Physics.Spawning;
using Physhicsh.Scenes;
using Physhicsh.UI;
using Physhicsh.Utils;
using Raylib_cs;
using rlImGui_cs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Physhicsh.Core;

public class MainWindow
{
    private List<PhysicsObject> _physicsObjects = [];
    private readonly Random _rng = new();
    private readonly ImGuiVault _imgVault = new();
    private World _world = null!;
    private float _accumulator;
    private PhysicsObject _pickedObject = null!;
    private PhysicsObject? _propertiesObject;
    private TextureLoader _textureLoader = new();
    private ImGuiUI _ui = null!;
    private List<Action> _imGuiWindows = [];
    private InputController _input;
    private SolverIterations _solver = new();
    private readonly HashSet<Timer> _timers = [];
    private Stopwatch _sw;
    private double _physicsTime;
    private double _inputTime;
    private double _updateObjTime;
    private SceneManager _scmgr = new();
    private SceneSettings _scettings = new();
    private string _settingsImportExportMsg = string.Empty;
    private bool _vulture = true;

    public void Start()
    {
        _sw = Stopwatch.StartNew();
        _imGuiWindows = [DrawPropertiesWindow, DrawSettingsWindow];

        Raylib.SetConfigFlags(ConfigFlags.VSyncHint | ConfigFlags.Msaa4xHint);
        Raylib.InitWindow(Config.SystemConfig.Instance.Screen.ScreenWidth, Config.SystemConfig.Instance.Screen.ScreenHeight, "Physhicsh");
        Raylib.SetWindowFocused();
        Raylib.SetTargetFPS(Config.SystemConfig.Instance.Screen.MaxFPS);

        LoadFont();

        rlImGui.Setup(true);

        _world = new World(new AethVec2(0, -9.81f * _scettings.GravityScale));

        _world.ContactManager.VelocityConstraintsMultithreadThreshold = 128;
        _world.ContactManager.PositionConstraintsMultithreadThreshold = 128;

        _input = new(_world, _ui, _physicsObjects, DeletePhysicsObject);
        _solver = new()
        {
            VelocityIterations = 8,
            PositionIterations = 16,
        };

        LoadScene("demo");

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
            if (File.Exists(_imgVault.IMGUI_FONT_PATH))
            {
                _ui = new(io.Fonts.AddFontFromFileTTF(
                _imgVault.IMGUI_FONT_PATH,
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
        _physicsObjects.Add(new CirclePhysicsObject(0, 10, 0.5f, Color.Green));
        _physicsObjects.Add(new RectanglePhysicsObject(0, 0, 1, 1, Color.DarkGray, 1));
        _physicsObjects.Add(new RectanglePhysicsObject(5, 0, 1, 1, Color.DarkGray, 1));
        _physicsObjects.Add(new RectanglePhysicsObject(0, -5, 10, 2, Color.SkyBlue, bodyType: BodyType.Static));
        _physicsObjects.Add(new RectanglePhysicsObject(0, -10, 100, 2, Color.SkyBlue, bodyType: BodyType.Static));
        _physicsObjects.Add(new RectanglePhysicsObject(20, -10, 1, 100, Color.SkyBlue, bodyType: BodyType.Static));
        _physicsObjects.Add(new RectanglePhysicsObject(-20, -10, 1, 100, Color.SkyBlue, bodyType: BodyType.Static));
    }


    private void Update()
    {
        _sw.Restart();
        StepPhysics(Raylib.GetFrameTime());
        _physicsTime = _sw.Elapsed.TotalMilliseconds;

        _sw.Restart();

        _input.Tick();
        _input.TickMouse();

        if (Raylib.IsKeyPressed(KeyboardKey.S))
        {
            _scmgr.CurrentScene.Objects = _physicsObjects;
            _scmgr.SaveSceneToFile("demo");
        }

        _inputTime = _sw.Elapsed.TotalMilliseconds;

        _sw.Restart();
        UpdatePhysicsObjects();
        _updateObjTime = _sw.Elapsed.TotalMilliseconds;


        if (Raylib.IsMouseButtonPressed(MouseButton.Right))
        {
            AethVec2 worldPos = _input.MousePosWorld;

            _ui.ShowPopup("##world_context", Raylib.GetMousePosition(), mb =>
            {
                mb.Item("Settings", () => _imgVault.Open(ImGuiWindow.Settings));

                mb.Separator();

                mb.SubMenu("Quick spawn", (x) => ImGuiStaticPopups.DrawQuickSpawnMenu(_world, worldPos, x, _physicsObjects));

                if (_pickedObject is not null)
                {
                    mb.Separator();
                    mb.Item("Delete", () => DeletePhysicsObject(_pickedObject));
                    mb.Item("Properties", () =>
                    {
                        _propertiesObject = _pickedObject;
                        _imgVault.Open(ImGuiWindow.Properties);
                    });
                }

            });
        }

        foreach (PhysicsObject item in _physicsObjects.ToArray())
        {
            if (IsOutOfBounds(item))
            {
                DeletePhysicsObject(item);
            }
        }
    }

    private static bool IsOutOfBounds(PhysicsObject item)
    {
        return MathF.Abs(item.Body.Position.X) > SystemConfig.Instance.Limits.BoundsLimit.X || MathF.Abs(item.Body.Position.Y) > SystemConfig.Instance.Limits.BoundsLimit.Y;
    }

    private void GraphicsUpdate()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(_scettings.CurrentBgColor);

        rlImGui.Begin();

        _ui.BeginFrame();

        foreach (Action window in _imGuiWindows)
        {
            window();
        }

        if (_input.ShowDebugWindow)
            DrawDebugWindow();

        _ui.EndFrame();

        DrawPhysicsObjects();


        rlImGui.End();
        Raylib.EndDrawing();
    }

    private void LoadScene(string path)
    {
        Scene? newScene;
        try
        {
            newScene = _scmgr.LoadSceneFromFile(path);
        }
        catch (Exception ex)
        {
            _settingsImportExportMsg = $"Couldn't load {Path.GetFileName(path)}, check log.";
            Log.Error(ex.Message);
            return;
        }

        if (newScene is null)
        {
            _settingsImportExportMsg = $"Couldn't load {Path.GetFileName(path)}, check log.";
            return;
        }

        List<PhysicsObject> started = [];
        try
        {
            foreach (PhysicsObject obj in newScene.Objects)
            {
                obj.Start(_world);
                started.Add(obj);
            }
        }
        catch (Exception ex)
        {
            foreach (PhysicsObject obj in started)
                try { obj.Delete(); } catch (Exception sx) { Log.Error(sx.Message); }

            _settingsImportExportMsg = $"Couldn't load {Path.GetFileName(path)}, check log.";
            Log.Error(ex.Message);
            return;
        }

        ClearAllPhysObjects();
        _physicsObjects.AddRange(started);
        _scmgr.CurrentScene.Objects = newScene.Objects;
        _settingsImportExportMsg = $"Loaded {Path.GetFileName(path)}";
    }

    private void SaveScene(string path)
    {
        _scmgr.CurrentScene?.Objects.Clear();
        _scmgr.CurrentScene?.Objects.AddRange(_physicsObjects);
        _scmgr.SaveSceneToFile(path);
        _settingsImportExportMsg = $"Saved to {path + ".psf"}";
    }


    private void DrawSettingsWindow()
    {
        if (!_imgVault.IsOpen(ImGuiWindow.Settings)) return;

        _ui.BeginWindow("Settings", ImGuiWindowFlags.NoCollapse, () =>
        {
            if (ImGui.SmallButton("X"))
            {
                _imgVault.Close(ImGuiWindow.Settings);
                return;
            }

            if (ImGui.Button("Save Scene"))
            {
                DialogResult savePath = Dialog.FileSave("psf", Path.Combine(AppContext.BaseDirectory, SystemConfig.Instance.Paths.ScenesPath));
                if (savePath.Path != null)
                {
                    SaveScene(savePath.Path);
                }
            }
            ImGui.SameLine();
            if (ImGui.Button("Select Scene"))
            {
                DialogResult selected = Dialog.FileOpen("psf", Path.Combine(AppContext.BaseDirectory, SystemConfig.Instance.Paths.ScenesPath));
                if (selected.Path != null)
                {
                    LoadScene(selected.Path);
                }
            }
            ImGui.SameLine();
            ImGui.TextColored(Color.Green.ToVector4(), $"{_settingsImportExportMsg}");


            ImColor currentBgColor = _scettings.CurrentBgColor.ToSysVector3();

            if (ImGui.ColorEdit3("Background Color", ref currentBgColor))
            {
                _scettings.CurrentBgColor = currentBgColor.ToColor();
            }

            int gravityRefSlider = (int)_scettings.GravityScale;

            if (ImGui.SliderInt("Gravity scale", ref gravityRefSlider, -SystemConfig.Instance.Limits.GravityScaleLimit, SystemConfig.Instance.Limits.GravityScaleLimit))
            {
                _scettings.GravityScale = gravityRefSlider;
                _world.Gravity = new(0, -9.81f * _scettings.GravityScale);
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
                    ClearAllPhysObjects();
                }
            }
        });
    }

    private void ClearAllPhysObjects()
    {
        foreach (PhysicsObject item in _physicsObjects.ToArray())
        {
            if (_pickedObject != item)
                DeletePhysicsObject(item);
        }
        _physicsObjects.Clear();
    }

    private void DeletePhysicsObject(PhysicsObject obj)
    {
        obj.Delete();
        _physicsObjects.Remove(obj);

        if (ReferenceEquals(_propertiesObject, obj))
            _propertiesObject = null;
        if (ReferenceEquals(_pickedObject, obj))
            _pickedObject = null;
    }



    private void DrawPropertiesWindow()
    {
        if (!_imgVault.IsOpen(ImGuiWindow.Properties)) return;
        _propertiesObject = _pickedObject;

        using (_ui.UseFont())
        {
            if (ImGui.Begin("Properties", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar))
            {
                if (ImGui.SmallButton("X"))
                {
                    _imgVault.Close(ImGuiWindow.Properties);
                    _propertiesObject = null;
                    ImGui.End();
                    return;
                }
                if (_propertiesObject is not null)
                {
                    _propertiesObject.Properties(_ui);
                }
                else
                {
                    ImGui.Text("No object.");
                }

                ImGui.End();
            }

        }

    }

    private void StepPhysics(float frameTime)
    {
        frameTime = Math.Min(frameTime, Config.SystemConfig.Instance.Physics.MaxFrameTime);
        _accumulator += frameTime;

        int stepCount = 0;

        while (_accumulator >= Config.SystemConfig.Instance.Physics.FixedDt && stepCount < Config.SystemConfig.Instance.Physics.MaxSubSteps)
        {
            _world.Step(Config.SystemConfig.Instance.Physics.FixedDt, ref _solver);
            _accumulator -= Config.SystemConfig.Instance.Physics.FixedDt;
            stepCount++;
        }

        // if while loop blocked
        if (_accumulator >= Config.SystemConfig.Instance.Physics.FixedDt)
        {
            _accumulator = 0f;
        }
    }

    private void UpdatePhysicsObjects()
    {
        foreach (PhysicsObject item in _physicsObjects)
            item.PhysicsUpdate(_world);
    }

    private void DrawDebugWindow()
    {
        _ui.BeginWindow("##debug", ImGuiWindowFlags.NoTitleBar, () =>
        {
            ImGui.Text($"FPS: {Raylib.GetFPS().ToString()}");
            ImGui.Text($"Frametime: {Raylib.GetFrameTime().ToString()}");
            ImGui.Text($"Accumulator: {_accumulator}");
            ImGui.Text($"Count: {_physicsObjects.Count}");
            ImGui.Text($"Physics: {_physicsTime}ms, Update: {_updateObjTime}ms, Input: {_inputTime}ms");
            foreach (PhysicsObject item in _physicsObjects)
            {
                ImGui.Text($"{item} Position: {item.Body.Position}");

            }
            ImGui.Separator();

            if (_pickedObject is not null)
            {
                ImGui.Text($"Picked: {_pickedObject}");
                ImGui.Text($"Holding Position: {_pickedObject.Body.Position}");
                ImGui.Text($"BodyType: {_pickedObject.Body.BodyType}");
                ImGui.Text($"IsBullet: {_pickedObject.Body.IsBullet}");
            }

            if (_propertiesObject is not null)
            {
                ImGui.Text($"Properties object: {_propertiesObject}");
            }
        });
    }

    private void DrawPhysicsObjects()
    {
        foreach (PhysicsObject obj in _physicsObjects)
        {
            obj.Draw();
        }
    }

}