using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Config;

namespace Physhicsh.Core.Input.Drag;

/// <summary>
/// Controls how static bodies get dragged
/// </summary>
public sealed class KinematicDragSession : IDragSession
{
    private readonly World _world;
    private readonly Body _body;
    private readonly BodyType _originalType;
    private readonly AethVec2 _originalVelocity;
    private bool _active = true;

    public bool IsActive => _active;
    public Body Body => _body;
    public BodyType EffectiveBodyType { get; private set; }

    public KinematicDragSession(World world, Body body, AethVec2 target)
    {
        _world = world;
        _body = body;
        _originalType = body.BodyType;
        _originalVelocity = body.LinearVelocity;
        EffectiveBodyType = body.BodyType;

        body.BodyType = BodyType.Kinematic;
        body.Enabled = true;
        body.Awake = true;
        Update(target);
    }

    public void SetEffectiveBodyType(BodyType type)
    {
        EffectiveBodyType = type;
    }

    public void Update(AethVec2 target)
    {
        float stiffness = SystemConfig.Instance.Controls.KinematicStiffness;
        float maxSpeed = SystemConfig.Instance.Controls.KinematicMaxSpeed;

        AethVec2 desired = (target - _body.Position) * stiffness;
        if (desired.LengthSquared() > maxSpeed * maxSpeed)
        {
            desired *= maxSpeed;
            desired.Normalize();
        }
        _body.LinearVelocity = desired;
        _body.Awake = true;
    }

    public void End()
    {
        _body.LinearVelocity = _originalVelocity;
        _body.BodyType = _originalType;
        _active = false;
    }
}
