using nkast.Aether.Physics2D.Collision.Shapes;
using System.Collections.Generic;

namespace Physhicsh.Json;

public sealed class ShapeData
{
    public ShapeType Kind { get; set; }
    public float Density { get; set; }
    public float? Radius { get; set; }
    public SysVec2 Position { get; set; }
    public List<AethVec2>? Vertices { get; set; }
    public AethVec2 Vertex1 { get; set; }
    public AethVec2 Vertex2 { get; set; }

    public static ShapeData ToData(Shape s) => s switch
    {
        CircleShape c => new() { Kind = c.ShapeType, Radius = c.Radius, Density = c.Density, Position = new(c.Position.X, c.Position.Y) },
        PolygonShape p => new() { Kind = p.ShapeType, Vertices = p.Vertices, Density = p.Density },
        _ => throw new KeyNotFoundException($"Invalid shape {s.ShapeType}"),
    };

    public static Shape FromData(ShapeData d) => d.Kind switch
    {
        ShapeType.Circle => new CircleShape(d.Radius!.Value, d.Density) { Density = d.Density, Position = new(d.Position.X, d.Position.Y) },
        ShapeType.Polygon => new PolygonShape(new(d.Vertices), d.Density),
    };
}
