using Physhicsh.Physics.PhysicsObjects;
using System.Collections.Generic;

namespace Physhicsh.Common
{
    public class ObjectRegistry
    {
        public List<PhysicsObject> PhysicsObjects { get; } = [];
        public Stack<Dictionary<PhysicsObject, AethVec2>> LatestDeletedObjects { get; } = [];
    }
}
