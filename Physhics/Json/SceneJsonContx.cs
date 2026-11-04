using nkast.Aether.Physics2D.Collision.Shapes;
using Physhicsh.PConfig;
using Physhicsh.PhysicsObjects;
using Physhicsh.Scenes;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Physhicsh.Json;

[JsonSourceGenerationOptions(ReferenceHandler = JsonKnownReferenceHandler.Preserve)]
[JsonSerializable(typeof(SysVec2))]
[JsonSerializable(typeof(Scene))]
[JsonSerializable(typeof(List<PhysicsObject>))]
[JsonSerializable(typeof(Config))]
[JsonSerializable(typeof(Shape))]
[JsonSerializable(typeof(CircleShape))]
[JsonSerializable(typeof(PolygonShape))]
[JsonSerializable(typeof(EdgeShape))]
[JsonSerializable(typeof(ChainShape))]
[JsonSerializable(typeof(ShapeData))]
public partial class SceneJsonContx : JsonSerializerContext;
