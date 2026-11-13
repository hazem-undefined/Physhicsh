using ImGuiNET;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Config;
using Physhicsh.Extensions;
using Physhicsh.UI;
using Raylib_cs;
using System;

namespace Physhicsh.Physics.PhysicsObjects;

public class CirclePhysicsObject(float x, float y, float radius, Color color, float density = 1f, BodyType bodyType = BodyType.Dynamic) : PhysicsObject(x, y, color, bodyType), ICloneable
{
    public float Radius { get { return Fixture.Shape.Radius; } set { Fixture.Shape.Radius = value; } }
    private float _screenRadius => Radius * SystemConfig.Instance.Graphics.PPM;

    public override void Start(World world)
    {
        base.Start(world);

        Fixture = Body.CreateCircle(radius, density);
        Fixture.Restitution = 0.3f;
        Fixture.Friction = 0.5f;

        Fixture.Tag = this;

    }


    public override void Draw()
    {
        base.Draw();

        Raylib.DrawCircleV(Body.Position.WorldToScreen().ToNumericsVector2(), _screenRadius, Color);
    }

    public override void DrawOutline()
    {
        base.DrawOutline();
        DrawCircleOutline(1);
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

    public object Clone()
    {
        return new CirclePhysicsObject(Body.Position.X, Body.Position.Y, Radius, Color, bodyType: Body.BodyType);
    }

    private void DrawCircleOutline(int thickness)
    {
        for (int i = 0; i < thickness; i++)
        {
            Raylib.DrawCircleLinesV(Body.Position.WorldToScreen().ToNumericsVector2(), _screenRadius + i, Color.Red);
        }
    }
}

