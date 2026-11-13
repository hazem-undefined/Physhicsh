namespace Physhicsh.Utils
{
    public static class VectorUtils
    {
        public static AethVec2 Px2M(SysVec2 px) =>
    new(px.X / Config.SystemConfig.Instance.Graphics.PPM, px.Y / Config.SystemConfig.Instance.Graphics.PPM);

        public static SysVec2 M2Px(AethVec2 m) =>
     new(m.X * Config.SystemConfig.Instance.Graphics.PPM, m.Y * Config.SystemConfig.Instance.Graphics.PPM);


    }

}
