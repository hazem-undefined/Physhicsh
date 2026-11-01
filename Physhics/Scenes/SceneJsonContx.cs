using Physhicsh.PConfig;
using Physhicsh.PhysicsObjects;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Physhicsh.Scenes;

[JsonSerializable(typeof(Scene))]
[JsonSerializable(typeof(List<PhysicsObject>))]
[JsonSerializable(typeof(Config))]
public partial class SceneJsonContx : JsonSerializerContext;
