using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Debug;
using Physhicsh.PhysicsObjects;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Physhicsh.Json;

public static class JsonSerializerConfig
{
    private static readonly Dictionary<Type, HashSet<string>> _allowed = new()
    {
        [typeof(PhysicsObject)] = ["Body", "Color", "Locked", "Fixture"],
        [typeof(Body)] = ["Tag", "Position", "Rotation", "BodyType", "IsBullet", "Mass"],
        [typeof(Fixture)] = ["Friction", "Restitution", "Shape"],
    };

    public static readonly JsonSerializerSettings Settings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto, // solves polymorphic types
        ContractResolver = new AllowList(),

        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor, // for internal fucking constructs
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // because fixture references body and references body references fixture, manyaka
        Error = (_, args) =>
        {
            Log.Error(
                $"Deserializing {args.ErrorContext.Path}: {args.ErrorContext.Error.Message}");
            args.ErrorContext.Handled = true;
        }
    };

    private class AllowList : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
            if (!_allowed.TryGetValue(type, out HashSet<string>? smash)) return props;

            return props.Where(p => smash.Contains(p.PropertyName)).ToList();
        }
    }
}
