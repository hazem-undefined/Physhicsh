namespace Physhicsh.Config;

public class GraphicsConfig
{
    public int ScreenWidth { get; set; } = 1280;
    public int ScreenHeight { get; set; } = 640;
    public int MaxFPS { get; set; } = 60;
    public bool UseVsync { get; set; } = true;
    public float PPM { get; set; } = 32f;
}
