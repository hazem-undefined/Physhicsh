using System;
using System.IO;

namespace Physhicsh.Config;

public class PathConfig
{
    public PathConfig()
    {
        CreateIfNotExist(SCENES_PATH);
        CreateIfNotExist(TEXTURES_PATH);
    }

    public string SCENES_PATH { get; set; } = "scenes";
    public string TEXTURES_PATH { get; set; } = "textures";

    public static void CreateIfNotExist(string path)
    {
        if (!Directory.Exists(path))
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception)
            {
                throw new($"Error while creating {path}.");
            }
        }
    }
}
