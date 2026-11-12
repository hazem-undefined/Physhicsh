using nkast.Aether.Physics2D.Dynamics;
using System;

namespace Physhicsh.Core
{
    public sealed class PhysicsWorld
    {
        public World CurrentWorld { get; private set; } = null!;
        private float _accumulator;
        private SolverIterations _solver = new();

        public void Init(float initGravityScale)
        {
            CurrentWorld = new World(new AethVec2(0, -9.81f * initGravityScale));

            CurrentWorld.ContactManager.VelocityConstraintsMultithreadThreshold = 128;
            CurrentWorld.ContactManager.PositionConstraintsMultithreadThreshold = 128;

            _solver = new()
            {
                VelocityIterations = 8,
                PositionIterations = 16,
            };
        }

        public void StepPhysics(float frameTime)
        {
            frameTime = Math.Min(frameTime, Config.SystemConfig.Instance.Physics.MaxFrameTime);
            _accumulator += frameTime;

            int stepCount = 0;

            while (_accumulator >= Config.SystemConfig.Instance.Physics.FixedDt && stepCount < Config.SystemConfig.Instance.Physics.MaxSubSteps)
            {
                CurrentWorld.Step(Config.SystemConfig.Instance.Physics.FixedDt, ref _solver);
                _accumulator -= Config.SystemConfig.Instance.Physics.FixedDt;
                stepCount++;
            }

            if (_accumulator >= Config.SystemConfig.Instance.Physics.FixedDt)
            {
                _accumulator = 0f;
            }
        }
    }
}
