using Physhicsh.PConfig;

namespace Physhicsh.Utils
{
    public static class VectorUtils
    {
        public static AethVec2 Px2M(SysVec2 px) =>
    new(px.X / Config.Instance.Screen.PPM, px.Y / Config.Instance.Screen.PPM);

        public static SysVec2 M2Px(AethVec2 m) =>
     new(m.X * Config.Instance.Screen.PPM, m.Y * Config.Instance.Screen.PPM);
    }
}
