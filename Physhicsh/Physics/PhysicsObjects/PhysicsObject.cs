using ImGuiNET;
using Newtonsoft.Json;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Config;
using Physhicsh.Extensions;
using Physhicsh.Json.DTO;
using Physhicsh.UI;
using Physhicsh.Utils;
using Raylib_cs;
using System;

namespace Physhicsh.Physics.PhysicsObjects;

public class PhysicsObject
{
    [JsonIgnore]
    public Body Body = null!;
    public BodySaveDTO BodyData { get; set; } = null!;
    public bool Gay { get; set; }
    public RayColor Color = Color.White;
    private RayColor _ogColorBeforeGay;
    public Fixture? Fixture { get; set; }
    public Texture2D? Texture;
    public bool Locked = false;
    private readonly TextureLoader _texLoader = new();
    private AethVec2 _startPosition = new();
    private BodyType _bodyType;
    private RayColor _randomColor;
    private float _t;
    private const float ChangeInterval = 0.3f;
    private bool _seeded;
    private bool _started;
    public virtual void Start(World world)
    {
        if (_started) return;

        if (BodyData != null)
            Body = BodyData.Create(world);
        else
            Body = world.CreateBody(_startPosition, 0, _bodyType);

        Body.Tag = this;

        //if (Fixture?.Shape is { } shape)
        //{
        //    Fixture f = Body.CreateFixture(shape);
        //    f.Friction = Fixture.Friction;
        //    f.Restitution = Fixture.Restitution;
        //}

        Body.Awake = true;
        Body.Enabled = true;
        Body.ResetDynamics();

        _started = true;
    }


    public virtual void PhysicsUpdate(World world)
    {

    }

    public virtual void Draw()
    {
        if (Gay)
        {
            if (!_seeded)
            {
                _ogColorBeforeGay = Color;
                _randomColor = RandomColorOnBlack();
                Color = _randomColor;
                _seeded = true;
            }

            _t += Raylib.GetFrameTime();
            float interval = SystemConfig.Instance.Graphics.GaySpeed;
            if (_t >= interval)
            {
                _t = 0f;
                _randomColor = RandomColorOnBlack();
            }

            Color = Color.Lerp(Color, _randomColor, 1f - MathF.Pow(0.001f, Raylib.GetFrameTime()));
            Color.A = 255;
        }
        else if (_seeded && !Gay)
        {
            Color = _ogColorBeforeGay;
        }
    }
    public virtual void DrawOutline()
    {

    }
    public virtual void Delete()
    {
        if (Body != null && Fixture != null && Body.FixtureList.Contains(Fixture)) Body.Remove(Fixture);

        Body.Tag = null;
        Body = null!;
    }
    private static Color RandomColorOnBlack()
    {
        RayColor c = Raylib.ColorFromHSV(Random.Shared.NextSingle() * 360f, 0.9f, 0.9f);
        c.A = 255;
        return c;
    }
    public virtual void Properties(ImGuiUI ui)
    {
        SysVec3 color = Color.ToSysVector3();
        ImGuiUtils.DrawColorEdit3("Color",
            new ImColor(color.X, color.Y, color.Z),
            c => Color = new SysVec3(c.X, c.Y, c.Z).ToAetherVector3().ToColor());

        ImGuiUtils.DrawSliderFloat("Mass", Body.Mass, 0f, 100f, v => Body.Mass = v);
        ImGuiUtils.DrawSliderFloat("Rotation", Body.Rotation * (-180f / MathF.PI), -360f, 360f, x => Body.Rotation = -x * (MathF.PI / 180f));

        if (Fixture != null)
        {
            ImGuiUtils.DrawSliderFloat("Boingness", Fixture.Restitution, 0f, 2f, v => Fixture.Restitution = v);
            ImGuiUtils.DrawSliderFloat("Friction", Fixture.Friction, 0f, 2f, v => Fixture.Friction = v);
        }

        ImGuiUtils.DrawCheckbox("Lock", Locked, v => Locked = v);

        ImGui.Separator();
    }

    [JsonConstructor]
    public PhysicsObject(Body body, RayColor color, bool locked, Fixture fixture)
    {
        this.Body = body;
        this.Color = color;
        this.Locked = locked;
        this.Fixture = fixture;

        _startPosition = body.Position;
    }

    public PhysicsObject(float x, float y, RayColor color, BodyType bodyType = BodyType.Dynamic, bool locked = false)
    {
        this.Color = color;
        this.Locked = locked;
        this._startPosition = new(x, y);
        this._bodyType = bodyType;
    }

    public PhysicsObject(PhysicsObject obj)
    {
        this.Color = obj.Color;
        this.Locked = obj.Locked;
        this._startPosition = obj.Body.Position;
        this._bodyType = obj._bodyType;
    }

    public BodySaveDTO CaptureForSave()
    {
        BodyData = BodySaveDTO.ToData(Body);
        return BodyData;
    }
}
