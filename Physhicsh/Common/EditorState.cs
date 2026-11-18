using Physhicsh.Physics.PhysicsObjects;

namespace Physhicsh.Common;

public class EditorState
{
    public PhysicsObject? PickedObject { get; set; }
    public PhysicsObject? PropertiesObject { get; set; }
    public bool ShowDebugWindow { get; set; }
}
