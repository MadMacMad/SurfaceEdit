using LZ4;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tilify
{
    public static class Extensions
    {
        public static Texture2D ConvertToTexture (this RenderTexture renderTexture)
        {
            var newTexture = new Texture2D (renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Graphics.CopyTexture (renderTexture, 0, 0, newTexture, 0, 0);
            return newTexture;
        }
        public static Texture2D ConvertToTextureAndRelease (this RenderTexture renderTexture)
        {
            var newTexture = renderTexture.ConvertToTexture ();
            renderTexture.Release ();
            return newTexture;
        }

        public static RenderTexture ConvertToRenderTexture (this Texture2D texture)
        {
            var renderTexture = Utils.CreateAndAllocateRenderTexture (texture.width, texture.height);
            new ComputeCopy (texture, renderTexture).Execute ();
            return renderTexture;
        }

        public static Texture2D Copy (this Texture2D texture)
        {
            var newTexture = new Texture2D (texture.width, texture.height, texture.format, false);
            Graphics.CopyTexture (texture, 0, 0, newTexture, 0, 0);
            return newTexture;
        }

        public static RenderTexture Copy (this RenderTexture texture)
        {
            var renderTexture = Utils.CreateAndAllocateRenderTexture (texture.width, texture.height);
            new ComputeCopy (texture, renderTexture).Execute();
            return renderTexture;
        }

        public static void Save(this Texture2D texture, string path)
        {
            var bytes = texture.EncodeToJPG (90);
            File.WriteAllBytes (path, bytes);
        }

        //private static byte[] m_SaveRenderTextureBuffer = new byte[8192 * 8192 * 4];

        //public static void SaveAsync (this RenderTexture texture, string directory, string name, Action callback)
        //{
        //    var path = Path.Combine (directory, name + ".binRT");
        //    AsyncTextureReader.RequestTextureData (texture);
            
        //    UpdateChecker.Instance.RegisterConditionChecker (
        //     () => AsyncTextureReader.RetrieveTextureData (texture, m_SaveRenderTextureBuffer) == AsyncTextureReader.Status.Succeeded,
        //     () =>
        //     {
        //         AsyncTextureReader.ReleaseTempResources (texture);
        //         var count = texture.width * texture.height * 4;
        //         Task.Run (() =>
        //          {
        //              using ( var fileStream = new FileStream (path, FileMode.CreateNew) )
        //                  using ( var lz4Stream = new LZ4Stream (fileStream, LZ4StreamMode.Compress) )
        //                    lz4Stream.Write (m_SaveRenderTextureBuffer, 0, m_SaveRenderTextureBuffer.Length);
        //          });

        //         callback ();
        //     });
        //}

        public static Vector2 ClampBoth(this Vector2 vector, float min, float max = float.MaxValue)
        {
            if ( vector.x < min )
                vector.x = min;
            else if ( vector.x > max )
                vector.x = max;

            if ( vector.y < min )
                vector.y = min;
            else if ( vector.y > max )
                vector.y = max;

            return vector;
        }
    }
}
