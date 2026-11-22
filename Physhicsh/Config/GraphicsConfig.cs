using Raylib_cs;
using Tomlyn.Serialization;

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
            Raylib.SetWindowSize(_screenWidth, _screenHeight);
        }
    }
    [TomlIgnore]
    public int ScreenWidth
    {
        get => _screenWidth;
        set
        {
            _screenWidth = value;
            Raylib.SetWindowSize(_screenWidth, Raylib.GetScreenHeight());
        }
    }
    [TomlIgnore]

    public int ScreenHeight
    {
        get => _screenHeight;
        set
        {
            _screenHeight = value;
            Raylib.SetWindowSize(Raylib.GetScreenWidth(), _screenHeight);
        }
    }
    public bool UseVsync { get; set; } = true;
    public bool MSAA { get; set; } = true;
    public bool Fullscreen
    {
        get { return _fullScreen; }
        set
        {
            _fullScreen = value;
            Raylib.ToggleFullscreen();
        }
    }
    private bool _fullScreen = true;
    public int MaxFPS
    {
        get { return _maxFPS; }
        set
        {
            _maxFPS = value;
            Raylib.SetTargetFPS(value);
        }
    }
    private int _maxFPS = 60;
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