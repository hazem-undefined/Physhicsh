using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Physics.PhysicsObjects;

namespace Physhicsh.Core.Input.Drag;

public sealed class DragController
{
    private readonly World _world;
    private IDragSession? _session;

    public bool IsActive => _session?.IsActive == true;
    public IDragSession? CurrentDrag => _session;

    public DragController(World world) => _world = world;

    public void TryBegin(ref PhysicsObject? picked, AethVec2 target)
    {
        if (picked is null || picked.Locked) return;

        picked.Body.IsBullet = true;

        _session = picked.Body.BodyType == BodyType.Dynamic ? new JointDragSession(_world, picked.Body, target)
                                                            : new KinematicDragSession(_world, picked.Body, target);
    }

    public void Update(AethVec2 target)
    {
        if (_session?.IsActive == true) _session.Update(target);
    }

    public void End()
    {
        if (_session is null) return;

        _session.End();
        _session = null;
    }
}
