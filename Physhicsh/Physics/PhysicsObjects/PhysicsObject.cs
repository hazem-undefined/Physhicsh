using ImGuiNET;
using Newtonsoft.Json;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Extensions;
using Physhicsh.Json.DTO;
using Physhicsh.UI;
using Raylib_cs;

namespace Physhicsh.Physics.PhysicsObjects;

public class PhysicsObject
{
    [JsonIgnore]
    public Body Body = null!;
    public BodySaveDTO BodyData { get; set; } = null!;
    public RayColor Color = Color.White;
    public Fixture? Fixture { get; set; }
    public Texture2D? Texture;
    public bool Locked = false;
    private readonly TextureLoader _texLoader = new();
    private AethVec2 _startPosition = new();
    private BodyType _bodyType;
    public virtual void Start(World world)
    {
        if (BodyData != null)
            Body = BodyData.Create(world);
        else
            Body = world.CreateBody(_startPosition, 0, _bodyType);

        Body.Tag = this;

        if (Fixture?.Shape is { } shape)
        {
            Fixture f = Body.CreateFixture(shape);
            f.Friction = Fixture.Friction;
            f.Restitution = Fixture.Restitution;
        }

        Body.Awake = true;
        Body.Enabled = true;
        Body.ResetDynamics();
    }


    public virtual void PhysicsUpdate(World world)
    {

    }

    public virtual void Draw()
    {

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
