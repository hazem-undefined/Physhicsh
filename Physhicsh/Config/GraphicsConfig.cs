using Raylib_cs;
using Tomlyn.Serialization;

namespace Physhicsh.Config;

public class GraphicsConfig
{
    private const int DefaultWidth = 1280;
    private const int DefaultHeight = 720;

    private int _screenWidth = DefaultWidth;
    private int _screenHeight = DefaultHeight;

    public Resolution Resolution
    {
        get => new(_screenWidth, _screenHeight);
        set
        {
            _screenWidth = value is { Width: > 0 } ? value.Width : DefaultWidth;
            _screenHeight = value is { Height: > 0 } ? value.Height : DefaultHeight;
            ApplyResolution();
        }
    }

    [TomlIgnore]
    public int ScreenWidth
    {
        get => _screenWidth;
        set { _screenWidth = value > 0 ? value : DefaultWidth; ApplyResolution(); }
    }

    [TomlIgnore]
    public int ScreenHeight
    {
        get => _screenHeight;
        set { _screenHeight = value > 0 ? value : DefaultHeight; ApplyResolution(); }
    }
    public bool UseVsync
    {
        get { return _useVsync; }
        set
        {
            _useVsync = value;
            if (value)
                WindowFlags |= ConfigFlags.VSyncHint;
            else
                WindowFlags &= ~ConfigFlags.VSyncHint;
        }
    }
    private bool _useVsync = true;
    public bool MSAA { get; set; } = true;
    public bool Fullscreen
    {
        get { return _fullScreen; }
        set
        {
            _fullScreen = value;

            if (Raylib.IsWindowFullscreen() != value)
                Raylib.ToggleFullscreen();
        }
    }
    private bool _fullScreen = false;
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

    [TomlIgnore]
    public ConfigFlags WindowFlags { get; set; }

    public void LockDimensions() => _dimensionsLocked = true;

    public void SetResolution(int width, int height)
    {
        _screenWidth = width > 0 ? width : DefaultWidth;
        _screenHeight = height > 0 ? height : DefaultHeight;
        ApplyResolution();
    }

    private void ApplyResolution()
    {
        if (Raylib.IsWindowReady())
            Raylib.SetWindowSize(_screenWidth, _screenHeight);
    }
}

public class Resolution(int width, int height)
{
    public int Width { get; set; } = width;
    public int Height { get; set; } = height;
}