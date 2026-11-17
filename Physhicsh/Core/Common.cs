using Physhicsh.Debug;
using Physhicsh.Physics.PhysicsObjects;
using Physhicsh.Scenes;
using Physhicsh.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace Physhicsh.Core;

public class Common
{
    public List<PhysicsObject> PhysicsObjects { get; }
    public Stack<Dictionary<PhysicsObject, AethVec2>> LatestDeletedObjects { get; }
    public ImGuiVault ImgVault { get; }
    public SceneSettings SceneSettings { get; }
    public PhysicsWorld World { get; }
    public Profiler Profiler { get; }

    public PhysicsObject? PickedObject { get; set; }
    public PhysicsObject? PropertiesObject { get; set; }
    public string SettingsImportExportMsg { get; set; } = string.Empty;
    public ImGuiUI UI { get; set; } = null!;
    public bool ShowDebugWindow { get; set; } = false;
    public SceneManager SceneManager = new();

    public Common()
    {
        ImgVault = new();
        PhysicsObjects = [];
        SceneSettings = new();
        World = new();
        Profiler = new();
        LatestDeletedObjects = [];
    }

    public void DeletePhysicsObject(PhysicsObject obj, bool storeInStack = false)
    {
        if (obj.Body == null || !PhysicsObjects.Contains(obj))
            return;

        if (obj is ICloneable c && storeInStack)
        {
            Dictionary<PhysicsObject, AethVec2> d = [];
            d.Add((PhysicsObject)c.Clone(), obj.Body.Position);
            LatestDeletedObjects.Push(d);
        }

        obj.Delete();
        PhysicsObjects.Remove(obj);

        if (ReferenceEquals(PropertiesObject, obj))
            PropertiesObject = null;
        if (ReferenceEquals(PickedObject, obj))
            PickedObject = null;
    }

    public void ClearAllPhysObjects()
    {
        if (PropertiesObject != null) PropertiesObject = null;
        if (PickedObject != null) PickedObject = null;

        foreach (PhysicsObject item in PhysicsObjects.ToArray())
        {
            DeletePhysicsObject(item);
        }

        PhysicsObjects.Clear();
    }

    public void LoadScene(string path)
    {
        Scene? newScene;
        try
        {
            newScene = SceneManager.LoadSceneFromFile(path);
        }
        catch (Exception ex)
        {
            SettingsImportExportMsg = $"Couldn't load {Path.GetFileName(path)}, check log.";
            Log.Error(ex.Message);
            return;
        }

        if (newScene is null)
        {
            SettingsImportExportMsg = $"Couldn't load {Path.GetFileName(path)}, check log.";
            return;
        }

        List<PhysicsObject> started = [];
        try
        {
            foreach (PhysicsObject obj in newScene.Objects)
            {
                obj.Start(World.CurrentWorld);
                started.Add(obj);
            }
        }
        catch (Exception ex)
        {
            foreach (PhysicsObject obj in started)
                try { obj.Delete(); } catch (Exception sx) { Log.Error(sx.Message); }

            SettingsImportExportMsg = $"Couldn't load {Path.GetFileName(path)}, check log.";
            Log.Error(ex.Message);
            return;
        }

        ClearAllPhysObjects();
        PhysicsObjects.AddRange(started);
        SceneManager.CurrentScene.Objects = newScene.Objects;
        SettingsImportExportMsg = $"Loaded {Path.GetFileName(path)}";
    }

    public void SaveScene(string path)
    {
        SceneManager.CurrentScene?.Objects.Clear();
        SceneManager.CurrentScene?.Objects.AddRange(PhysicsObjects);
        SceneManager.SaveSceneToFile(path);
        SettingsImportExportMsg = $"Saved to {path + ".psf"}";
    }
}
