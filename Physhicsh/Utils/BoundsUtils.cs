namespace Physhicsh.Utils
{
    public static class BoundsUtils
    {
        public static SysVec2 CenterPositionPivot(float x, float y, float w, float h)
        {
            return new SysVec2(x - (w / 2), y - (h / 2));
        }
        public static SysVec2 CenterPositionPivot(SysVec2 pos, SysVec2 size)
        {
            return CenterPositionPivot(pos.X, pos.Y, size.X, size.Y);
        }
    }
}
