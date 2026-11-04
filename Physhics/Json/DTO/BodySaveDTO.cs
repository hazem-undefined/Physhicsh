using nkast.Aether.Physics2D.Dynamics;

namespace Physhicsh.Json.DTO
{
    public sealed class BodySaveDTO
    {
        public string? Tag { get; set; }
        public AethVec2 Position { get; set; }
        public float Rotation { get; set; }
        public BodyType BodyType { get; set; }
        public bool IsBullet { get; set; }
        public float Mass { get; set; }

        public static BodySaveDTO ToData(Body b) => new()
        {
            Tag = b.Tag as string,
            Position = b.Position,
            Rotation = b.Rotation,
            BodyType = b.BodyType,
            IsBullet = b.IsBullet,
            Mass = b.Mass,
        };

        public Body Create(World world)
        {
            Body body = world.CreateBody(Position, Rotation, BodyType);
            body.Tag = Tag;
            body.IsBullet = IsBullet;
            return body;
        }
    }
}
