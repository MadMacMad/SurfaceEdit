using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageMagick;
using LZ4;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace SurfaceEdit
{
    public static class TextureUtility
    {
        private static List<string> magickSupportedFormats = new List<string> ();

        static TextureUtility()
        {
            magickSupportedFormats = Enum.GetNames (typeof (MagickFormat))
                .Select (s => s.ToLowerInvariant())
                .ToList();
        }

        public static RenderTexture CreateRenderTexture (int width, int height, RenderTextureFormat format = RenderTextureFormat.ARGB32)
        {
            RenderTexture renderTexture = new RenderTexture (width, height, 0, format)
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
            var texture = new Texture2D (2, 2, TextureFormat.RGBA32, false);
            texture.LoadImage (data);

            return texture;
        }

        public static void SaveTexture2DToDisk(string path, Texture2D texture, Action callback = null)
        {
            Assert.ArgumentNotNullOrEmptry (path, nameof (path));
            Assert.ArgumentNotNull (texture, nameof (texture));
            Assert.ArgumentTrue (!path.IsDirectory (), $"Path ({path}) is directory");

            var extension = Path.GetExtension (path);

            Assert.ArgumentTrue (magickSupportedFormats.Contains (extension.Substring (1).ToLowerInvariant ()), "Unsupported extension! " + path);

            var request = AsyncGPUReadback.Request (texture);
            var width = texture.width;
            var height = texture.height;

            UnityCallbackRegistrator.Instance.OnUpdate += CheckRequest;

            void CheckRequest()
            {
                if ( request.done )
                {
                    UnityCallbackRegistrator.Instance.OnUpdate -= CheckRequest;

                    var buffer = request.GetData<byte> ().ToArray();

                    using ( var image = new MagickImage (buffer, new PixelStorageSettings (width, height, StorageType.Char, "ARGB")) )
                        image.Write (path);
                    
                    callback?.Invoke ();
                }
            }
        }
    }
}
