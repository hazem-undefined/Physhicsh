using ImGuiNET;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Config;
using Physhicsh.Extensions;
using Physhicsh.UI;
using Raylib_cs;
using System;

namespace Physhicsh.Physics.PhysicsObjects;

public class CirclePhysicsObject : PhysicsObject
{
    private float _initialRadius;
    private readonly float _density;

    public CirclePhysicsObject(float x, float y, float radius, RayColor color, float density = 1f, BodyType bodyType = BodyType.Dynamic)
        : base(x, y, color, bodyType)
    {
        _initialRadius = radius;
        _density = density;
    }

    private CirclePhysicsObject(CirclePhysicsObject source) : base(source)
    {
        _initialRadius = source._initialRadius;
        _density = source._density;
    }

    public float Radius
    {
        get => Fixture?.Shape.Radius ?? _initialRadius; set
        {
            if (Fixture != null) Fixture.Shape.Radius = value;
            else _initialRadius = value;
        }
    }

    private float _screenRadius => Radius * SystemConfig.Instance.Graphics.PPM;

    protected override PhysicsObject CreateCopy() => new CirclePhysicsObject(this);

    public override void Start(World world)
    {
        base.Start(world);

        if (Fixture?.Shape == null)
        {
            Fixture = Body.CreateCircle(_initialRadius, _density);
            Fixture.Restitution = 0.3f;
            Fixture.Friction = 0.5f;
        }

        Fixture.Tag = this;

        ApplyInitialState();
    }
    public override void Draw()
    {
        base.Draw();



        Raylib.DrawCircleV(Body.Position.WorldToScreen().ToNumericsVector2(),
                           _screenRadius, Color);
    }


    public override void DrawOutline()
    {
        base.DrawOutline();
        DrawCircleOutline(SystemConfig.Instance.Graphics.OutlineThickness);
    }

    public override void Properties(ImGuiUI ui)
    {
        base.Properties(ui);

        float r = Radius;
        SysVec2 limit = SystemConfig.Instance.Limits.CircleSpawnRadiusLimit;
        if (ImGui.SliderFloat("Radius", ref r, limit.X, limit.Y))
        {
            Radius = r;
        }
    }

    private void DrawCircleOutline(int thickness)
    {
        if (thickness <= 0) return;

        SysVec2 center = Body.Position.WorldToScreen().ToNumericsVector2();
        float inner = _screenRadius;
        float outer = _screenRadius + thickness;

        int segments = Math.Clamp((int)(outer * 1.5f), 24, 256);

        Raylib.DrawRing(
            center,
            inner,
            outer,
            0f,
            360f,
            segments,
            SystemConfig.Instance.Graphics.OutLineColor);
    }
}

