using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Debug;
using Physhicsh.Physics.PhysicsObjects;
using Physhicsh.Scenes;
using System;
using System.Collections.Generic;
using System.IO;

namespace Physhicsh.Common;

public class SceneIO
{
    public SceneManager SceneManager { get; } = new();
    public string SettingsImportExportMsg { get; set; } = string.Empty;

    private readonly World _currentWorld;
    private readonly EditorState _edState;
    private readonly ObjectRegistry _reg;

    public SceneIO(World currentWorld, EditorState edState, ObjectRegistry reg)
    {
        _currentWorld = currentWorld;
        _edState = edState;
        _reg = reg;
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
                obj.Start(_currentWorld);
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
        _reg.PhysicsObjects.AddRange(started);
        SceneManager?.CurrentScene?.Objects = newScene.Objects;
        SettingsImportExportMsg = $"Loaded {Path.GetFileName(path)}";
    }

    public void SaveScene(string path)
    {
        SceneManager.CurrentScene?.Objects.Clear();
        SceneManager.CurrentScene?.Objects.AddRange(_reg.PhysicsObjects);
        SceneManager.SaveSceneToFile(path);
        SettingsImportExportMsg = $"Saved to {path + ".psf"}";
    }

    public void ClearAllPhysObjects()
    {
        if (_edState.PropertiesObject != null) _edState.PropertiesObject = null;
        if (_edState.PickedObject != null) _edState.PickedObject = null;

        foreach (PhysicsObject item in _reg.PhysicsObjects.ToArray())
        {
            DeletePhysicsObject(item);
        }

        _reg.PhysicsObjects.Clear();
    }

    public void DeletePhysicsObject(PhysicsObject obj, bool storeInStack = false)
    {
        if (obj.Body == null || !_reg.PhysicsObjects.Contains(obj))
            return;

        if (obj is ICloneable c && storeInStack)
        {
            Dictionary<PhysicsObject, AethVec2> d = [];
            d.Add((PhysicsObject)c.Clone(), obj.Body.Position);
            _reg.LatestDeletedObjects.Push(d);
        }

        obj.Delete();
        _reg.PhysicsObjects.Remove(obj);

        if (ReferenceEquals(_edState.PropertiesObject, obj))
            _edState.PropertiesObject = null;
        if (ReferenceEquals(_edState.PickedObject, obj))
            _edState.PickedObject = null;
    }
}
