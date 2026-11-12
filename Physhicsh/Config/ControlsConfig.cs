namespace Physhicsh.Config;

public class ControlsConfig
{
    public float RotationSpeed { get; set; } = 10f;
    public float KinematicStiffness { get; set; } = 10f;
    public float KinematicMaxSpeed { get; set; } = 50f;
    public float JointMaxForcePerKg { get; set; } = 1000f;
    public float JointFrequency { get; set; } = 5f;
    public bool AutoSelectSpawnedObject { get; set; } = true;
    public bool SelectOnHover { get; set; } = true;
}
