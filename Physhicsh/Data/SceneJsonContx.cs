using nkast.Aether.Physics2D.Collision.Shapes;
using Physhicsh.Json.DTO;
using Physhicsh.PhysicsObjects;
using Physhicsh.Scenes;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Physhicsh.Json;

[JsonSourceGenerationOptions(ReferenceHandler = JsonKnownReferenceHandler.Preserve)]
[JsonSerializable(typeof(SysVec2))]
[JsonSerializable(typeof(Scene))]
[JsonSerializable(typeof(List<PhysicsObject>))]
[JsonSerializable(typeof(Config.SystemConfig))]
[JsonSerializable(typeof(Shape))]
[JsonSerializable(typeof(CircleShape))]
[JsonSerializable(typeof(PolygonShape))]
[JsonSerializable(typeof(EdgeShape))]
[JsonSerializable(typeof(ChainShape))]
[JsonSerializable(typeof(ShapeSaveDTO))]
public partial class SceneJsonContx : JsonSerializerContext;
