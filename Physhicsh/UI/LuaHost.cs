using ImGuiNET;
using MoonSharp.Interpreter;
using Physhicsh.Debug;
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
        public UISettings Settings = new UISettings();

        public LuaHost()
        {
            _lua = new();

            UserData.RegisterType<UISettings>();

            _lua.Globals["settings"] = Settings;

            Table imguiTable = new(_lua);
            imguiTable["Begin"] = (Func<string, bool>)ImGui.Begin;
            imguiTable["End"] = (Action)ImGui.End;

            imguiTable["SliderFloat"] = (Func<string, float, float, float, float>)((label, value, min, max) =>
            {
                ImGui.SliderFloat(label, ref value, min, max);
                return value;
            });

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
