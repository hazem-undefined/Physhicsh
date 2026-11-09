using Physhicsh.Config;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.IO;

namespace Physhicsh.Physics.PhysicsObjects
{
    public class TextureLoader : IDisposable
    {
        private readonly List<Image> _loadedImages = [];
        private readonly List<Texture2D> _loadedTextures = [];

        public Texture2D LoadTexture(string path)
        {
            string combinedPath = Path.Combine(Config.SystemConfig.Instance.Paths.TexturesPath, path);
            if (!Directory.Exists(Config.SystemConfig.Instance.Paths.TexturesPath)) Directory.CreateDirectory(Config.SystemConfig.Instance.Paths.TexturesPath);
            if (File.Exists(combinedPath))
            {
                return Raylib.LoadTexture(combinedPath);
            }
            else
            {
                throw new FileNotFoundException(combinedPath);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            foreach (Texture2D item in _loadedTextures)
            {
                Raylib.UnloadTexture(item);
            }

            foreach (Image item in _loadedImages)
            {
                Raylib.UnloadImage(item);
            }
        }
    }
}
