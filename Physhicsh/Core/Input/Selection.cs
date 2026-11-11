using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Physics.PhysicsObjects;
using Physhicsh.Utils;

namespace Physhicsh.Core.Input;

public sealed class Selection
{
    private PhysicsObject? _current;

    public void Pick(World world, AethVec2 at)
    {
        AetherUtils.PickFixture(world, at, ref _current);
    }

    public void Set(PhysicsObject? obj) => _current = obj;
    public PhysicsObject? Get() => _current;
}
