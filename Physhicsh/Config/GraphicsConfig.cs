namespace Physhicsh.Config;

public class GraphicsConfig
{
    private int _screenWidth;
    private int _screenHeight;

    public Resolution Resolution
    {
        get => new(_screenWidth, _screenHeight);
        set
        {
            _screenWidth = value?.Width ?? 0;
            _screenHeight = value?.Height ?? 0;
        }
    }

    public int ScreenWidth
    {
        get => _screenWidth;
        set => _screenWidth = value;
    }

    public int ScreenHeight
    {
        get => _screenHeight;
        set => _screenHeight = value;
    }
    public bool UseVsync { get; set; } = true;
    public int MaxFPS { get; set; } = 60;
    public bool ShowOutline { get; set; } = true;
    public bool ShowDragLine { get; set; } = true;
    public RayColor OutLineColor { get; set; } = RayColor.Red;
    public RayColor DragLineColor { get; set; } = RayColor.Green;
    public int DragLineTransparency { get; set; } = 100;
    public int OutlineThickness { get; set; } = 3;
    public int DragLineThickness { get; set; } = 3;
    public bool Gay { get; set; } = false;
    public float GaySpeed { get; set; } = 0.01f;
    public float PPM { get; set; } = 32f;

    private bool _dimensionsLocked = false;

    public void LockDimensions() => _dimensionsLocked = true;
}

public class Resolution(int width, int height)
{
    public int Width { get; set; } = width;
    public int Height { get; set; } = height;
}