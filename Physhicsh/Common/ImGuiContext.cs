using Physhicsh.UI;

namespace Physhicsh.Common
{
    public class ImGuiContext
    {
        public ImGuiVault ImgVault { get; } = new();
        public ImGuiUI UI { get; set; } = null!;
        public bool ShowDebugWindow { get; set; } = false;

    }
}
