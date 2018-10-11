using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LZ4;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace SurfaceEdit
{
    public static class TextureUtility
    {
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

        public static bool IsSupportedExtension(string extension)
        {
            if ( string.IsNullOrEmpty (extension) )
                return false;

            extension = extension.Replace (".", "");

            return ( extension == "png" || extension == "jpg" || extension == "jpeg" || extension == "tga" );
        }

        public static bool TryLoadTexture2DFromDisk(string path, out Texture2D texture)
        {
            texture = null;

            try
            {
                texture = LoadTexture2DFromDisk (path);
            }
            catch
            {
                if (texture != null)
                {
                    GameObject.DestroyImmediate (texture);
                    texture = null;
                }
                return false;
            }

            return true;
        }

        public static Texture2D LoadTexture2DFromDisk (string path)
        {
            Assert.ArgumentNotNullOrEmptry (path, nameof (path));

            var extension = Path.GetExtension (path);

            Assert.ArgumentTrue (IsSupportedExtension(extension),
                                 $"Unsupported extension: {extension}. Full path: {path}\nSupported Extensions: .png, .jpg, .jpeg, .tga");

            Assert.ArgumentTrue (File.Exists (path), $"File ({path}) does not exists.");

            byte[] data = File.ReadAllBytes (path);
            var texture = new Texture2D (2, 2, TextureFormat.RGBA32, false);
            texture.LoadImage (data);
            texture.Apply ();

            return texture;
        }

        public static void SaveTexture2DToDisk(string path, Texture2D texture)
        {
            Assert.ArgumentNotNullOrEmptry (path, nameof (path));
            Assert.ArgumentNotNull (texture, nameof (texture));
            Assert.ArgumentTrue (!path.IsDirectory (), $"Path ({path}) is directory");

            var extension = Path.GetExtension (path);

            Assert.ArgumentTrue (IsSupportedExtension (extension),
                                 $"Unsupported extension: {extension}. Full path: {path}\nSupported Extensions: .png, .jpg, .jpeg, .tga");

            var request = AsyncGPUReadback.Request (texture, 0, TextureFormat.RGBA32);
            request.WaitForCompletion ();

            var buffer = request.GetData<Color32> ();
            var newTexture = new Texture2D (texture.width, texture.height, TextureFormat.RGBA32, false);
            newTexture.SetPixels32 (buffer.ToArray());
                    
            byte[] data = null;

            if ( extension == ".tga" )
                data = newTexture.EncodeToTGA ();
            else if ( extension == ".jpeg" || extension == ".jpg" )
                data = newTexture.EncodeToJPG (100);
            else if ( extension == ".png" )
                data = newTexture.EncodeToPNG ();

            File.WriteAllBytes (path, data);
                
            
        }
        public static void SaveTexture2DToDiskAsync (string path, Texture2D texture, Action callback = null)
        {
            Assert.ArgumentNotNullOrEmptry (path, nameof (path));
            Assert.ArgumentNotNull (texture, nameof (texture));
            Assert.ArgumentTrue (!path.IsDirectory (), $"Path ({path}) is directory");

            var extension = Path.GetExtension (path);

            Assert.ArgumentTrue (IsSupportedExtension (extension),
                                 $"Unsupported extension: {extension}. Full path: {path}\nSupported Extensions: .png, .jpg, .jpeg, .tga");

            var request = AsyncGPUReadback.Request (texture, 0, TextureFormat.RGBA32);
            var width = texture.width;
            var height = texture.height;

            UnityCallbackRegistrator.Instance.OnUpdate += CheckRequest;

            void CheckRequest ()
            {
                if ( request.done )
                {
                    UnityCallbackRegistrator.Instance.OnUpdate -= CheckRequest;

                    var buffer = request.GetData<Color32> ();

                    var newTexture = new Texture2D (width, height, TextureFormat.RGBA32, false);
                    newTexture.SetPixels32 (buffer.ToArray ());

                    byte[] data = null;

                    if ( extension == ".tga" )
                        data = newTexture.EncodeToTGA ();
                    else if ( extension == ".jpeg" || extension == ".jpg" )
                        data = newTexture.EncodeToJPG (100);
                    else if ( extension == ".png" )
                        data = newTexture.EncodeToPNG ();

                    File.WriteAllBytes (path, data);

                    callback?.Invoke ();
                }
            }
        }
    }
    public enum TextureExtension
    {
        png,
        jpeg,
        jpg,
        tga
    }
}
