using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Joints;
using Physhicsh.Config;

namespace Physhicsh.Core.Input.Drag;

/// <summary>
/// Controls how dynamic bodies get dragged
/// </summary>
public sealed class JointDragSession : IDragSession
{
    private readonly World _world;
    private readonly FixedMouseJoint _joint;
    private bool _active = true;

    public bool IsActive => _active;

    public Body Body => _joint.BodyA;

    public BodyType EffectiveBodyType => Body.BodyType;

    public JointDragSession(World world, Body body, AethVec2 target)
    {
        _world = world;
        _joint = new(body, target)
        {
            MaxForce = SystemConfig.Instance.Controls.JointMaxForcePerKg * body.Mass,
            Frequency = SystemConfig.Instance.Controls.JointFrequency,
            DampingRatio = 1,
        };

        world.Add(_joint);
    }

    public void End()
    {
        Body.IsBullet = false;
        _world.Remove(_joint);
        _active = false;
    }

    public void Update(AethVec2 target)
    {
        AethVec2 delta = target - Body.Position;

        if (delta.Length() > SystemConfig.Instance.Controls.MaxPull)
        {
            delta.Normalize();
            target = Body.Position + (delta * SystemConfig.Instance.Controls.MaxPull);
        }

        _joint.WorldAnchorB = target;

        float speed = Body.LinearVelocity.Length();
        if (speed > SystemConfig.Instance.Controls.MaxSpeed)
            Body.LinearVelocity *= SystemConfig.Instance.Controls.MaxSpeed / speed;
    }
}
