namespace Physhicsh.Config;

public class PhysicsConfig
{
    public int MaxSubSteps { get; set; } = 3;
    public float FixedDt { get; set; } = 1f / 60f;
    public float MaxFrameTime { get; set; } = 0.060f;
}
