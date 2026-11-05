using System;
using System.IO;

namespace Physhicsh.Utils;

public static class FormatUtils
{
    public static string FormatPath(string path, string ext, string? combine = null)
    {
        if (Path.IsPathRooted(path) && path.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
            return path;

        if (!path.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
            path += ext;

        if (combine != null) return Path.Combine(combine, path);

        return path;
    }
}
