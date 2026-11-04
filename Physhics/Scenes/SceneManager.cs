using Newtonsoft.Json;
using Physhicsh.Json;
using Physhicsh.PConfig;
using Physhicsh.PhysicsObjects;
using System;
using System.IO;

namespace Physhicsh.Scenes;

public class SceneManager
{
    public Scene? CurrentScene { get; private set; }

    public SceneManager()
    {
        CurrentScene = new();
    }

    public Scene? LoadSceneFromFile(string path)
    {
        if (!File.Exists(FormatPath(path)))
            return null;

        string json = File.ReadAllText(FormatPath(path));

        CurrentScene = JsonConvert.DeserializeObject<Scene>(json, JsonSerializerConfig.Settings);

        return CurrentScene;
    }

    public void SaveSceneToFile(string path)
    {
        ArgumentNullException.ThrowIfNull(CurrentScene);

        foreach (PhysicsObject obj in CurrentScene.Objects)
            obj.CaptureForSave();

        string json = JsonConvert.SerializeObject(CurrentScene, JsonSerializerConfig.Settings);

        File.WriteAllText(FormatPath(path), json);
    }

    public static string FormatPath(string path)
    {
        const string ext = Config.PHYSHICSH_SCENE_FILE;
        string scenes = Config.Instance.Paths.SCENES_PATH;

        if (Path.IsPathRooted(path) && path.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
            return path;

        if (!path.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
            path += ext;

        return Path.Combine(scenes, path);
    }
}