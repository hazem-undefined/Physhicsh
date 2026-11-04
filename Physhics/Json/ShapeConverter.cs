using nkast.Aether.Physics2D.Collision.Shapes;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Physhicsh.Json;

public sealed class ShapeConverter : JsonConverter<Shape>
{
    public override Shape Read(ref Utf8JsonReader reader, Type _, JsonSerializerOptions __)
    {
        var data = JsonSerializer.Deserialize(ref reader, SceneJsonContx.Default.ShapeData)!;
        return ShapeData.FromData(data);
    }

    public override void Write(Utf8JsonWriter writer, Shape value, JsonSerializerOptions _)
    {
        ShapeData data = ShapeData.ToData(value);
        JsonSerializer.Serialize(writer, data, SceneJsonContx.Default.ShapeData);
    }
}
