using Physhicsh.Core;
using Physhicsh.Debug;
using Physhicsh.Physics.PhysicsObjects;
using System;
using System.Collections.Generic;
using System.IO;

namespace Physhicsh.Scenes;

public class SceneController(Common c)
{
    private readonly Common _c = c;
    public SceneManager SceneManager = new();
    public void LoadScene(string path)
    {
        Scene? newScene;
        try
        {
            newScene = SceneManager.LoadSceneFromFile(path);
        }
        catch (Exception ex)
        {
            _c.SettingsImportExportMsg = $"Couldn't load {Path.GetFileName(path)}, check log.";
            Log.Error(ex.Message);
            return;
        }

        if (newScene is null)
        {
            _c.SettingsImportExportMsg = $"Couldn't load {Path.GetFileName(path)}, check log.";
            return;
        }

        List<PhysicsObject> started = [];
        try
        {
            foreach (PhysicsObject obj in newScene.Objects)
            {
                obj.Start(_c.World.CurrentWorld);
                started.Add(obj);
            }
        }
        catch (Exception ex)
        {
            foreach (PhysicsObject obj in started)
                try { obj.Delete(); } catch (Exception sx) { Log.Error(sx.Message); }

            _c.SettingsImportExportMsg = $"Couldn't load {Path.GetFileName(path)}, check log.";
            Log.Error(ex.Message);
            return;
        }

        _c.ClearAllPhysObjects();
        _c.PhysicsObjects.AddRange(started);
        SceneManager.CurrentScene.Objects = newScene.Objects;
        _c.SettingsImportExportMsg = $"Loaded {Path.GetFileName(path)}";
    }

    public void SaveScene(string path)
    {
        SceneManager.CurrentScene?.Objects.Clear();
        SceneManager.CurrentScene?.Objects.AddRange(_c.PhysicsObjects);
        SceneManager.SaveSceneToFile(path);
        _c.SettingsImportExportMsg = $"Saved to {path + ".psf"}";
    }
}
