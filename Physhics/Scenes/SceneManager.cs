using System;
using System.IO;
using System.Text.Json;

namespace Physhicsh.Scenes;

public static class SceneManager
{
    public static Scene? LoadSceneFromFile(string path)
    {
        if (!File.Exists(path))
            return null;

        string json = File.ReadAllText(path);

        return JsonSerializer.Deserialize<Scene>(json, SceneJsonContx.Default.Scene);
    }

    public static void SaveSceneToFile(string path, Scene scene)
    {
        ArgumentNullException.ThrowIfNull(scene);

        string json = JsonSerializer.Serialize(scene, SceneJsonContx.Default.Scene);

        File.WriteAllText(path, json);
    }
}