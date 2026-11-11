using ImGuiNET;
using MoonSharp.Interpreter;
using Physhicsh.Debug;
using Physhicsh.Scenes;
using System;
using System.IO;
using System.Threading;

namespace Physhicsh.UI
{
    public class LuaHost
    {
        private Script _lua;
        private DynValue _drawFunc;
        private FileSystemWatcher _watcher;
        private string _luaUiPath = "ui.lua";

        public class UISettings
        {
            public float myValue = 50f;
            public float minLimit = 0f;
            public float maxLimit = 100f;
        }
        public UISettings Settings = new();

        public LuaHost(ImGuiVault vault, SceneManager scmgr, ImGuiUI ui)
        {
            _lua = new();

            UserData.RegisterType<UISettings>();
            UserData.RegisterType<ImGuiVault>();
            UserData.RegisterType<SceneManager>();

            _lua.Globals["settings"] = Settings;
            _lua.Globals["vault"] = vault;
            _lua.Globals["scene"] = scmgr;
            _lua.Globals["ui"] = ui;

            Table imguiTable = new(_lua);
            Table sceneTable = new(_lua);
            Table uiTable = new(_lua);

            sceneTable["Save"] =
            imguiTable["Begin"] = (Func<string, bool>)ImGui.Begin;
            imguiTable["End"] = (Action)ImGui.End;

            imguiTable["SliderFloat"] = (Func<string, float, float, float, float>)((label, value, min, max) =>
            {
                ImGui.SliderFloat(label, ref value, min, max);
                return value;
            });

            imguiTable["Button"] = (Func<string, bool>)((label) => ImGui.Button(label));
            imguiTable["SmallButton"] = (Func<string, bool>)((label) => ImGui.SmallButton(label));

            _lua.Globals["imgui"] = imguiTable;

            LoadScript();

            _watcher = new(Directory.GetCurrentDirectory())
            {
                Filter = _luaUiPath,
            };

            _watcher.Changed += _watcher_Changed;
            _watcher.EnableRaisingEvents = true;

        }

        private void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(100);
            LoadScript();
        }

        private void LoadScript()
        {
            try
            {
                string scriptCode = File.ReadAllText(_luaUiPath);
                _drawFunc = _lua.DoString(scriptCode, null, _luaUiPath);
                Log.Info("[LuaUI]: Reloaded.");
            }
            catch (Exception ex)
            {
                Log.Error($"[LuaUI]: {ex.Message}.");
            }
        }

        public void Draw()
        {
            _drawFunc?.Function.Call();
        }
    }
}
