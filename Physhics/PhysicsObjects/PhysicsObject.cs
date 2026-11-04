using ImGuiNET;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Extensions;
using Physhicsh.UI;
using Raylib_cs;
using System.Text.Json.Serialization;

namespace Physhicsh.PhysicsObjects;

public class PhysicsObject
{
    [JsonInclude]
    public Body Body = null!;
    [JsonInclude]
    public RayColor Color = Color.White;
    [JsonInclude]
    public Fixture? Fixture { get; set; }
    [JsonInclude]
    public Texture2D? Texture;
    [JsonInclude]
    public bool Locked = false;
    private readonly TextureLoader _texLoader = new();
    private AethVec2 _startPosition = new();
    private BodyType _bodyType;
    public virtual void Start(World world)
    {
        Body = world.CreateBody(_startPosition, 0, _bodyType);
        Body.Tag = this;
    }


    public virtual void PhysicsUpdate(World world)
    {

    }

    public virtual void Draw()
    {

    }
    public virtual void Delete()
    {
        Body.Remove(Fixture);
        Body.Tag = null;
        Body = null!;
    }

    public virtual void Properties(ImGuiUI ui)
    {
        SysVec3 currentColor = Color.ToSysVector3();

        if (ImGui.ColorEdit3("Color", ref currentColor))
        {
            Color = currentColor.ToAetherVector3().ToColor();
        }

        float massRefSlider = Body.Mass;

        if (ImGui.SliderFloat("Mass", ref massRefSlider, 0f, 100f))
        {
            Body.Mass = massRefSlider;
        }

        float res = Fixture.Restitution;
        if (ImGui.SliderFloat("Boingness", ref res, 0f, 2f))
        {
            Fixture.Restitution = res;
        }

        float slippy = Fixture.Friction;
        if (ImGui.SliderFloat("Slippiness", ref slippy, 0f, 2f))
        {
            Fixture.Friction = slippy;
        }

        bool lockedRef = Locked;
        if (ImGui.Checkbox("Lock", ref lockedRef))
        {
            Locked = lockedRef;
        }

        //if (ImGui.Button("Set texture"))
        //{
        //    byte[] buff = [];
        //    if (ImGui.InputText("Path", buff, 1024))
        //    {

        //    }
        //}
        ImGui.Separator();
        //ImGui.Text($"{Texture}");
    }

    [JsonConstructor]
    public PhysicsObject(Body body, RayColor color, bool locked, Fixture fixture)
    {
        this.Body = body;
        this.Color = color;
        this.Locked = locked;
        this.Fixture = fixture;

        if (fixture?.Shape is { } shape)
        {
            body.CreateFixture(shape);
        }
    }

    public PhysicsObject(float x, float y, RayColor color, BodyType bodyType = BodyType.Dynamic, bool locked = false)
    {
        this.Color = color;
        this.Locked = locked;
        this._startPosition = new(x, y);
        this._bodyType = bodyType;
    }
}
