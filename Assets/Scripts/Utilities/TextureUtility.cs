using System.IO;
using UnityEngine;

namespace SurfaceEdit
{
    public static class TextureUtility
    {
        public static RenderTexture CreateRenderTexture (int width, int height)
        {
            RenderTexture renderTexture = new RenderTexture (width, height, 0, RenderTextureFormat.ARGB32)
            {
                enableRandomWrite = true
            };
            renderTexture.Create ();
            return renderTexture;
        }

        public static RenderTexture CreateRenderTexture (int size)
        {
            return CreateRenderTexture (size, size);
        }

        public static RenderTexture CreateRenderTexture (Vector2Int size)
        {
            return CreateRenderTexture (size.x, size.y);
        }

        public static Texture2D LoadTexture2DFromDisk (string path)
        {
            Assert.ArgumentNotNullOrEmptry (path, nameof (path));

            var extension = Path.GetExtension (path);

            Assert.ArgumentTrue (extension == ".png" || extension == ".jpg" || extension == ".jpeg",
                                 $"Unsupported extension: {extension}. Full path: {path}\nSupported Extensions: .png, .jpg, .jpeg");

            Assert.ArgumentTrue (File.Exists (path), $"File ({path}) does not exists.");

            byte[] data = File.ReadAllBytes (path);
            var texture = new Texture2D (2, 2, TextureFormat.ARGB32, false);
            texture.LoadImage (data);

            return texture;
        }
    }
}
