namespace Physhicsh.Physics.PhysicsObjects;

public interface ISafeClone<T> where T : PhysicsObject
{
    T SafeClone();
}
