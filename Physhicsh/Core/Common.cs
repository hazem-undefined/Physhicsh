using Physhicsh.Physics.PhysicsObjects;
using Physhicsh.Scenes;
using Physhicsh.UI;
using System.Collections.Generic;

namespace Physhicsh.Core
{
    public class Common
    {
        public List<PhysicsObject> PhysicsObjects;
        public ImGuiVault ImgVault;
        public SceneSettings SceneSettings;
        public PhysicsWorld World;
        public PhysicsObject? PickedObject;
        public PhysicsObject? PropertiesObject;
        public string SettingsImportExportMsg = string.Empty;

        public Common(ref List<PhysicsObject> physicsObjects, ref SceneSettings scettings, ref PhysicsWorld world, ImGuiVault imgVault)
        {
            PhysicsObjects = physicsObjects;
            ImgVault = imgVault;
            SceneSettings = scettings;
            World = world;
        }
        public Common()
        {
            PhysicsObjects = [];
            ImgVault = new();
            SceneSettings = new();
            World = new();
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
                if (PickedObject != item)
                    DeletePhysicsObject(item);
            }

            PhysicsObjects.Clear();
        }
    }
}
