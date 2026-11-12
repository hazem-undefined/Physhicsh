using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Physics.PhysicsObjects;
using Physhicsh.Utils;

namespace Physhicsh.Core.Input;

public sealed class Selection
{
    public PhysicsObject? Current;

    public void Pick(World world, AethVec2 at)
    {
        AetherUtils.PickFixture(world, at, ref Current);
    }

    public void Set(PhysicsObject? obj) => Current = obj;
}
