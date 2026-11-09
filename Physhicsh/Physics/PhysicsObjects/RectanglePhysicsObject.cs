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

            if (Texture is Texture2D texture)
            {
                // Texture rect
                Rectangle source = new(0, 0, texture.Width, texture.Height);
                // Drawn rect
                Rectangle dist = new(screenPos.X, screenPos.Y, _widthPx, _heightPx);
                AethVec2 origin = new(_widthPx / 2f, _heightPx / 2f);
                float rotationDegrees = -Body.Rotation * (180f / MathF.PI);

                Raylib.DrawTexturePro(texture, source, dist, origin.ToNumericsVector2(), rotationDegrees, RayColor.White);
            }
            else
            {
                Rectangle rec = new(screenPos.X, screenPos.Y, _widthPx, _heightPx);
                AethVec2 origin = new(_widthPx / 2f, _heightPx / 2f);
                float rotationDegrees = -Body.Rotation * (180f / MathF.PI);

                Raylib.DrawRectanglePro(rec, origin.ToNumericsVector2(), rotationDegrees, Color);
            }
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
