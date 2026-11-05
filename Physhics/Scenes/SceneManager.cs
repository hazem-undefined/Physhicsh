using Newtonsoft.Json;
using Physhicsh.Json;
using Physhicsh.PhysicsObjects;
using Physhicsh.Utils;
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
        if (!File.Exists(Format(path)))
            return null;

        string json = File.ReadAllText(Format(path));

        CurrentScene = JsonConvert.DeserializeObject<Scene>(json, JsonSerializerConfig.Settings);

        return CurrentScene;
    }

    public void SaveSceneToFile(string path)
    {
        ArgumentNullException.ThrowIfNull(CurrentScene);

        foreach (PhysicsObject obj in CurrentScene.Objects)
            obj.CaptureForSave();

        string json = JsonConvert.SerializeObject(CurrentScene, JsonSerializerConfig.Settings);

        File.WriteAllText(Format(path), json);
    }

    private static string Format(string path)
    {
        return FormatUtils.FormatPath(path, Config.SystemConfig.PHYSHICSH_SCENE_FILE, Config.SystemConfig.Instance.Paths.SCENES_PATH);
    }

}