using System;
using System.IO;

namespace Physhicsh.Config;

public class PathConfig
{
    public PathConfig()
    {
        CreateIfNotExist(ScenesPath);
        CreateIfNotExist(TexturesPath);
    }

    public string ScenesPath { get; set; } = "scenes";
    public string TexturesPath { get; set; } = "textures";

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
