using nkast.Aether.Physics2D.Collision.Shapes;
using Physhicsh.Json.DTO;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Physhicsh.Json;

public sealed class ShapeConverter : JsonConverter<Shape>
{
    public override Shape Read(ref Utf8JsonReader reader, Type _, JsonSerializerOptions __)
    {
        ShapeSaveDTO data = JsonSerializer.Deserialize(ref reader, SceneJsonContx.Default.ShapeSaveDTO)!;
        return ShapeSaveDTO.FromData((ShapeSaveDTO)data);
    }

    public override void Write(Utf8JsonWriter writer, Shape value, JsonSerializerOptions _)
    {
        ShapeSaveDTO data = ShapeSaveDTO.ToData(value);
        JsonSerializer.Serialize(writer, data, SceneJsonContx.Default.ShapeSaveDTO);
    }
}
