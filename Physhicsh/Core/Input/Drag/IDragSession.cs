using nkast.Aether.Physics2D.Dynamics;

namespace Physhicsh.Core.Input.Drag;

public interface IDragSession
{
    bool IsActive { get; }
    Body Body { get; }
    BodyType EffectiveBodyType { get; }
    void Update(AethVec2 target);
    void End();
}
