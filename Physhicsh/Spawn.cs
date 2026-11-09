using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using Physhicsh.Extensions;
using Physhicsh.Physics.PhysicsObjects;
using System;
using System.Collections.Generic;

namespace Physhicsh;

public static class Spawn
{
    public static RectanglePhysicsObject SpawnRectangle(World world, Vector2 pos, List<PhysicsObject>? list = null)
    {
        RectanglePhysicsObject newRect = new(
            pos.X, pos.Y, SpawnRules.Instance.RectSize.X, SpawnRules.Instance.RectSize.Y,
            SpawnRules.Instance.UseRandomColors ? Random.Shared.RandomColor() : SpawnRules.Instance.SpawnColor, 1);

        return SpawnPhysicsObject(newRect, world, list);
    }

    public static CirclePhysicsObject SpawnCircle(World world, Vector2 pos, List<PhysicsObject>? list = null)
    {
        CirclePhysicsObject newBall = new(
            pos.X, pos.Y, Random.Shared.NextFloat(SpawnRules.Instance.CirclesRadius.X, SpawnRules.Instance.CirclesRadius.Y),
            SpawnRules.Instance.UseRandomColors ? Random.Shared.RandomColor() : SpawnRules.Instance.SpawnColor, 1);

        return SpawnPhysicsObject(newBall, world, list);
    }

    public static T SpawnPhysicsObject<T>(T obj, World world, List<PhysicsObject>? list = null, AethVec2? customSpawnPosition = null) where T : PhysicsObject
    {

        list?.Add(obj);
        obj.Start(world);

        if (customSpawnPosition != null) obj.Body.Position = customSpawnPosition.Value;

        return obj;
    }
}
