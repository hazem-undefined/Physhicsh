using Newtonsoft.Json;
using Physhicsh.Data;
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
        string resolved = Format(path);
        if (!File.Exists(resolved))
            return null;

        string json = File.ReadAllText(resolved);

        Scene? loadedScene;
        try
        {
            loadedScene = JsonConvert.DeserializeObject<Scene>(json, JsonSerializerConfig.Settings);
        }
        catch (JsonException ex)
        {
            throw new FileLoadException($"{path} is not a valid scene file.", ex);
        }

        Validate(loadedScene, path);
        return loadedScene;
    }

    private static void Validate(Scene? scene, string path)
    {
        if (scene is null) throw new FileLoadException($"{path}: not a scene file.");
        if (scene.Objects is null) throw new FileLoadException($"{path}: missing Objects.");

        for (int i = 0; i < scene.Objects.Count; i++)
        {
            if (scene.Objects[i] is null)
            {
                throw new FileLoadException($"{path}: null object at index {i}.");
            }
        }
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
        return FormatUtils.FormatPath(path, Config.SystemConfig.PHYSHICSH_SCENE_FILE, Config.SystemConfig.Instance.Paths.ScenesPath);
    }

}