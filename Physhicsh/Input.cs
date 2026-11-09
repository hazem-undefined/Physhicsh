using ImGuiNET;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Joints;
using Physhicsh.Config;
using Physhicsh.Extensions;
using Physhicsh.Physics.PhysicsObjects;
using Physhicsh.UI;
using Physhicsh.Utils;
using Raylib_cs;
using System;
using System.Collections.Generic;

namespace Physhicsh;

public class Input(World world, List<PhysicsObject> objs)
{
    private readonly World _world = world;
    private readonly List<PhysicsObject> _physicsObjects = objs;
    public bool ShowDebugWindow { get; set; } = false;
    public AethVec2 MousePosWorld { get; private set; } = new();
    public FixedMouseJoint? MouseJoint { get; private set; }
    private Body? _kinematicDragBody;
    private BodyType _prevKinematicBodyType;
    private AethVec2 _prevKinematicLinearVelocity;
    private bool _isKinematicDragging;
    private bool _angularWasResetOnce = false;
    private PhysicsObject? _pickedObject;
    public void Update(ref PhysicsObject pickedObject)
    {
        MousePosWorld = Raylib.GetMousePosition().ToAetherVector2().ScreenToWorld();
        pickedObject = _pickedObject;
    }

    public void HandleShortcuts(ImGuiUI ui, Action<PhysicsObject> DeletePhysicsObject)
    {
        ImGuiIOPtr io = ImGui.GetIO();
        if (io.WantCaptureKeyboard) return;

        float dt = SystemConfig.Instance.Physics.FixedDt;

        TrySpawnOnKey(KeyboardKey.R, p => Spawn.SpawnRectangle(_world, p, _physicsObjects));
        TrySpawnOnKey(KeyboardKey.C, p => Spawn.SpawnCircle(_world, p, _physicsObjects));
        if (_pickedObject != null)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.D))
            {
                if (_pickedObject is ICloneable r)
                    Spawn.SpawnPhysicsObject(r.Clone() as PhysicsObject, _world, _physicsObjects, MousePosWorld);
            }
        }

        if (Raylib.IsKeyPressed(KeyboardKey.F2)) SystemConfig.Reload();

        if (Raylib.IsKeyPressed(KeyboardKey.Tab))
        {
            AethVec2 mousePos = MousePosWorld;
            ui.ShowPopup("##quick_spawn", Raylib.GetMousePosition(), () => ImGuiStaticPopups.DrawQuickSpawnMenu(_world, mousePos, new(), _physicsObjects));
        }


        bool isRotating = _pickedObject is not null &&
                          (Raylib.IsKeyDown(KeyboardKey.Q) || Raylib.IsKeyDown(KeyboardKey.E));

        if (Raylib.IsKeyPressed(KeyboardKey.V) && _pickedObject is not null && !isRotating)
        {
            BodyType effectiveType = _isKinematicDragging && _kinematicDragBody is not null
                ? _prevKinematicBodyType
                : _pickedObject.Body.BodyType;

            if (effectiveType == BodyType.Static)
            {
                if (_isKinematicDragging && _kinematicDragBody is not null)
                    _prevKinematicBodyType = BodyType.Dynamic;
                else
                    _pickedObject.Body.BodyType = BodyType.Dynamic;
            }
            else if (effectiveType == BodyType.Dynamic)
            {
                if (_isKinematicDragging && _kinematicDragBody is not null)
                    _prevKinematicBodyType = BodyType.Static;
                else
                    _pickedObject.Body.BodyType = BodyType.Static;
            }
        }

        if (Raylib.IsKeyPressed(KeyboardKey.X) && _pickedObject is not null) DeletePhysicsObject(_pickedObject);
        if (Raylib.IsKeyPressed(KeyboardKey.F3)) ShowDebugWindow = !ShowDebugWindow;

        if (_pickedObject is not null)
        {
            float rotationSpeed = Raylib.IsKeyDown(KeyboardKey.LeftShift)
                ? Config.SystemConfig.Instance.Controls.RotationSpeed / 2f
                : Config.SystemConfig.Instance.Controls.RotationSpeed;

            float direction = 0f;

            if (Raylib.IsKeyDown(KeyboardKey.Q)) direction = 1;
            else if (Raylib.IsKeyDown(KeyboardKey.E)) direction = -1;
            else direction = 0f;

            if (Raylib.IsKeyDown(KeyboardKey.Q) || Raylib.IsKeyDown(KeyboardKey.E))
            {
                _angularWasResetOnce = false;
                _pickedObject.Body.IsBullet = true;
                _pickedObject.Body.AngularVelocity = direction * rotationSpeed;
                _pickedObject.Body.Awake = true;
            }
            else
            {
                if (!_angularWasResetOnce)
                {
                    _pickedObject.Body.AngularVelocity = 0f;
                    _angularWasResetOnce = true;
                }
            }
        }

        if (_pickedObject is CirclePhysicsObject circle)
        {
            if (Raylib.IsKeyDown(KeyboardKey.H))
                circle.Radius += 0.01f;
            if (Raylib.IsKeyDown(KeyboardKey.J))
                circle.Radius -= 0.01f;
        }
    }

    public void HandleMouseInput(ImGuiUI ui, World world)
    {
        if (ui.AnyPopupsOpen() || ImGui.GetIO().WantCaptureMouse)
        {
            if (MouseJoint != null || _isKinematicDragging)
                EndDrag(world);

            return;
        }

        if (Raylib.IsMouseButtonPressed(MouseButton.Left) || Raylib.IsMouseButtonPressed(MouseButton.Right))
        {
            AetherUtils.PickFixture(world, MousePosWorld, ref _pickedObject);
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                BeginDrag(world);
        }
        else if ((MouseJoint != null || _isKinematicDragging) && Raylib.IsMouseButtonDown(MouseButton.Left))
        {
            UpdateDrag();
        }
        else if ((MouseJoint != null || _isKinematicDragging) && (Raylib.IsMouseButtonReleased(MouseButton.Left)))
        {
            EndDrag(world);
        }
    }

    public void BeginDrag(World world)
    {
        if (_pickedObject == null) return;
        if (_pickedObject.Locked) return;

        Body body = _pickedObject.Body;
        body.IsBullet = true;

        if (body.BodyType != BodyType.Dynamic)
        {
            _kinematicDragBody = body;
            _prevKinematicBodyType = body.BodyType;
            _prevKinematicLinearVelocity = body.LinearVelocity;
            body.BodyType = BodyType.Kinematic;
            body.Enabled = true;
            body.Awake = true;
            _isKinematicDragging = true;
        }
        else
        {
            MouseJoint = new FixedMouseJoint(body, MousePosWorld)
            {
                MaxForce = 1000f * body.Mass,
                Frequency = 5.0f,
            };

            world.Add(MouseJoint);
        }
    }

    public void UpdateDrag()
    {
        if (_isKinematicDragging && _kinematicDragBody != null)
        {
            const float stiffness = 10.0f;
            const float maxSpeed = 50.0f;

            AethVec2 desiredVelocity = (MousePosWorld - _kinematicDragBody.Position) * stiffness;
            if (desiredVelocity.LengthSquared() > maxSpeed * maxSpeed)
                (desiredVelocity *= maxSpeed).Normalize();

            _kinematicDragBody.LinearVelocity = desiredVelocity;
            _kinematicDragBody.Awake = true;
        }
        else if (MouseJoint != null)
        {
            MouseJoint.WorldAnchorB = MousePosWorld;
        }
    }

    public void EndDrag(World world)
    {
        if (MouseJoint is not null)
        {
            world.Remove(MouseJoint);
            MouseJoint = null;
        }

        if (_isKinematicDragging && _kinematicDragBody != null)
        {
            _kinematicDragBody.LinearVelocity = _prevKinematicLinearVelocity;
            _kinematicDragBody.BodyType = _prevKinematicBodyType;
            _kinematicDragBody = null;
            _isKinematicDragging = false;
        }
    }

    // TODO: Fix clipping
    // TODO: Camera (Long term)
    // TODO: Store bg color in scenes
    // TODO: Add more logs

    private static bool IsKeyPressedOrRepeated(KeyboardKey key) =>
    Raylib.IsKeyPressed(key) || Raylib.IsKeyPressedRepeat(key);

    private void TrySpawnOnKey(KeyboardKey key, Func<AethVec2, PhysicsObject> spawn)
    {
        if (!IsKeyPressedOrRepeated(key)) return;
        if (SystemConfig.Instance.Controls.AutoSelectSpawnedObject) _pickedObject = spawn(MousePosWorld);
        else spawn(MousePosWorld);

    }
}