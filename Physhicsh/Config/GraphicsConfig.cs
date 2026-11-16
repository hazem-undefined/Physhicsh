namespace Physhicsh.Config;

public class GraphicsConfig
{
    public int ScreenWidth { get; set; } = 1280;
    public int ScreenHeight { get; set; } = 640;
    public bool UseVsync { get; set; } = true;
    public int MaxFPS { get; set; } = 60;
    public bool ShowOutline { get; set; } = true;
    public bool ShowDragLine { get; set; } = true;
    public RayColor OutLineColor { get; set; } = RayColor.Red;
    public RayColor DragLineColor { get; set; } = RayColor.Green;
    public int DragLineTransparency { get; set; } = 100;
    public int OutlineThickness { get; set; } = 3;
    public int DragLineThickness { get; set; } = 3;
    public float PPM { get; set; } = 32f;
}
