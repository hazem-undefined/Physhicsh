using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Config;
using Raylib_cs;

namespace Physhicsh.Core.Input.Drag;

public sealed class RotationController
{
    private Body? _body;
    private bool _wasResetting;
    public bool IsRotating { get; private set; }

    public void SetTarget(Body? body) => _body = body;

    public void Tick()
    {
        if (_body is null) return;

        IsRotating = Raylib.IsKeyDown(KeyboardKey.Q) || Raylib.IsKeyDown(KeyboardKey.E);

        if (IsRotating)
        {
            float speed = Raylib.IsKeyDown(KeyboardKey.LeftShift)
                ? SystemConfig.Instance.Controls.RotationSpeed / 2f
                : SystemConfig.Instance.Controls.RotationSpeed;
            float dir = Raylib.IsKeyDown(KeyboardKey.Q) ? 1f : -1f;

            _body.IsBullet = true;
            _body.AngularVelocity = dir * speed;
            _body.Awake = true;
            _wasResetting = false;
        }
        else if (!_wasResetting || !IsRotating)
        {
            _body.AngularVelocity = 0f;
            _wasResetting = true;
        }
    }
}
