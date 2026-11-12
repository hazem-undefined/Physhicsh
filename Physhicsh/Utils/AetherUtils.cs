using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Physics.PhysicsObjects;

namespace Physhicsh.Utils;

public static class AetherUtils
{
    public static Fixture? PickFixture(World world, AethVec2 mouseWorld, ref PhysicsObject? pickedObject)
    {
        Fixture fixture = world.TestPoint(mouseWorld);

        pickedObject = ApplyFixtureToPicked(ref pickedObject, fixture);

        return fixture == null ? null : fixture;
    }

    public static PhysicsObject? ApplyFixtureToPicked(ref PhysicsObject? pickedObject, Fixture fixture)
    {
        if (pickedObject?.Body != null)
        {
            pickedObject.Body.IsBullet = false;
        }

        if (fixture != null)
            pickedObject = fixture.Tag as PhysicsObject;
        else
            pickedObject = null;
        return pickedObject;
    }

    public static Fixture? PickFixture(World world, AethVec2 mouseWorld)
    {
        Fixture fixture = world.TestPoint(mouseWorld);

        return fixture == null ? null : fixture;
    }
}
