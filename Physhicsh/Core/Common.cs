using Physhicsh.Debug;
using Physhicsh.Physics.PhysicsObjects;
using Physhicsh.Scenes;
using Physhicsh.UI;
using System.Collections.Generic;

namespace Physhicsh.Core
{
    public class Common
    {
        public List<PhysicsObject> PhysicsObjects { get; }
        public ImGuiVault ImgVault { get; }
        public SceneSettings SceneSettings { get; }
        public PhysicsWorld World { get; }
        public Profiler Profiler { get; }

        public PhysicsObject? PickedObject { get; set; }
        public PhysicsObject? PropertiesObject { get; set; }
        public string SettingsImportExportMsg { get; set; } = string.Empty;

        public ImGuiUI UI { get; set; } = null!;
        public bool ShowDebugWindow { get; set; } = false;


        public Common()
        {
            ImgVault = new();
            PhysicsObjects = [];
            SceneSettings = new();
            World = new();
            Profiler = new();
        }

        public void DeletePhysicsObject(PhysicsObject obj)
        {
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
    }
}
