using Physhicsh.Config;

namespace Physhicsh.Utils
{
    public static class VectorUtils
    {
        public static AethVec2 Px2M(SysVec2 px) =>
    new(px.X / Config.SystemConfig.Instance.Screen.PPM, px.Y / Config.SystemConfig.Instance.Screen.PPM);

        public static SysVec2 M2Px(AethVec2 m) =>
     new(m.X * Config.SystemConfig.Instance.Screen.PPM, m.Y * Config.SystemConfig.Instance.Screen.PPM);
    }
}
