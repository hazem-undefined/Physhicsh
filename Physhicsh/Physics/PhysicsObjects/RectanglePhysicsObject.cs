using ImGuiNET;
using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Config;
using Physhicsh.Extensions;
using Physhicsh.UI;
using Raylib_cs;
using System;

namespace Physhicsh.Physics.PhysicsObjects
{
    public class RectanglePhysicsObject : PhysicsObject, ICloneable
    {
        public AethVec2 Size;
        private float _widthPx;
        private float _heightPx;
        private Rectangle rec;
        private AethVec2 origin;
        private float rotationDegrees;
        private float _outlineThickness = 1;

        public RectanglePhysicsObject(float x, float y, float width, float height, RayColor color, float density = 1f, BodyType bodyType = BodyType.Dynamic)
            : base(x, y, color, bodyType)
        {
            Size = new AethVec2(width, height);
        }


        public override void Start(World world)
        {
            base.Start(world);
            BuildFixture();
        }

        public override void Draw()
        {
            base.Draw();

            (_widthPx, _heightPx) = MeasurePolygon(Fixture.Shape as PolygonShape);
            AethVec2 screenPos = Body.Position.WorldToScreen();

            rec = new(screenPos.X, screenPos.Y, _widthPx, _heightPx);
            origin = new(_widthPx / 2f, _heightPx / 2f);
            rotationDegrees = -Body.Rotation * (180f / MathF.PI);

            Raylib.DrawRectanglePro(rec, origin.ToNumericsVector2(), rotationDegrees, Color);

            //Raylib.DrawRectangleLinesEx(new(rec.Position - origin.ToNumericsVector2(), rec.Size), 2, Color.Red);
            //Raylib.DrawPolyLinesEx(rec.Position, 4, 25, rotationDegrees + 45, 2, Color.Red);
        }

        // Vibe coded
        public override void DrawOutline()
        {
            AethVec2 center = new(rec.X, rec.Y);

            float rad = rotationDegrees * MathF.PI / 180f;
            float cos = MathF.Cos(rad);
            float sin = MathF.Sin(rad);

            float hw = origin.X;
            float hh = origin.Y;

            Span<float> lx = [-hw, hw, hw, -hw];
            Span<float> ly = [-hh, -hh, hh, hh];
            Span<SysVec2> corners = stackalloc SysVec2[4];

            for (int i = 0; i < 4; i++)
            {
                float rx = (lx[i] * cos) - ly[i] * sin;
                float ry = (lx[i] * sin) + ly[i] * cos;
                corners[i] = new SysVec2(center.X + rx, center.Y + ry);
            }

            for (int i = 0; i < 4; i++)
                Raylib.DrawLineEx(corners[i], corners[(i + 1) % 4], _outlineThickness, Color.Red);
        }
        public void Resize(AethVec2 newSize)
        {
            if (newSize == AethVec2.Zero) return;
            if (newSize == Size) return;

            Size = newSize;

            float friction = Fixture.Friction;
            float restit = Fixture.Restitution;
            object tag = Fixture.Tag;

            Body.Remove(Fixture);

            BuildFixture();

            Fixture.Friction = friction;
            Fixture.Restitution = restit;
            Fixture.Tag = tag;

        }

        private void BuildFixture()
        {
            Fixture = Body.CreateRectangle(Size.X, Size.Y, 10f, AethVec2.Zero);
            Fixture.Restitution = 0.3f;
            Fixture.Friction = 0.5f;
            Fixture.Tag = this;
        }

        // Vibe coded
        private static (float width, float height) MeasurePolygon(PolygonShape polygon)
        {
            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;

            foreach (AethVec2 v in polygon.Vertices)
            {
                minX = MathF.Min(minX, v.X);
                maxX = MathF.Max(maxX, v.X);
                minY = MathF.Min(minY, v.Y);
                maxY = MathF.Max(maxY, v.Y);
            }

            return ((maxX - minX) * SystemConfig.Instance.Screen.PPM, (maxY - minY) * Config.SystemConfig.Instance.Screen.PPM);
        }
        public override void Properties(ImGuiUI ui)
        {
            base.Properties(ui);

            SysVec2 sizeRefSlider = Size.ToNumericsVector2();
            SysVec2 limit = SystemConfig.Instance.Limits.RectangleSpawnSizeLimit;
            if (ImGui.SliderFloat2("Size", ref sizeRefSlider, limit.X, limit.Y))
            {
                Resize(sizeRefSlider.ToAetherVector2());
            }
        }

        object ICloneable.Clone()
        {
            return new RectanglePhysicsObject(Body.Position.X, Body.Position.Y, Size.X, Size.Y, Color, bodyType: Body.BodyType);
        }
    }
}
