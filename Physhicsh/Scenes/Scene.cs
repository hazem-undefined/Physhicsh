using Newtonsoft.Json;
using Physhicsh.Physics.PhysicsObjects;
using System.Collections.Generic;

namespace Physhicsh.Scenes;

public class Scene
{
    [JsonProperty(Required = Required.Always)]
    public List<PhysicsObject> Objects { get; set; } = [];
}
