namespace Physhicsh.Config;

public class ControlsConfig
{
    public float RotationSpeed { get; set; } = 10f;
    public float KinematicStiffness { get; set; } = 25f;
    public float JointMaxForcePerKg { get; set; } = 1000f;
    public float JointFrequency { get; set; } = 5f;
    public bool AutoSelectSpawnedObject { get; set; } = true;
    public bool SelectOnHover { get; set; } = true;
    public bool DismissOnBlank { get; set; } = false;
    public float MaxPull { get; set; } = 15f;
    public float MaxSpeed { get; set; } = 45f;
    public float MassAdditionOnPick { get; set; } = 50;
}
