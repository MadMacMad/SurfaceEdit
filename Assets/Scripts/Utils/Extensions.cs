using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SurfaceEdit
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

        public static Vector2 Clamp (this Vector2 vector, Vector2 min, Vector2 max)
        {
            var newVector = vector;

            if ( newVector.x < min.x )
                newVector.x = min.x;
            else if ( newVector.x > max.x )
                newVector.x = max.x;

            if ( newVector.y < min.y )
                newVector.y = min.y;
            else if ( newVector.y > max.y )
                newVector.y = max.y;

            return newVector;
        }

        public static Vector2 Clamp01 (this Vector2 vector)
            => vector.Clamp (Vector2.zero, Vector2.one);

        public static IEnumerable<Enum> GetFlags (this Enum e) 
            => Enum.GetValues (e.GetType ()).Cast<Enum> ().Where (e.HasFlag);

        public static void SetColor(this ComputeShader shader, string name, Color color)
            => shader.SetFloats (name, color.r, color.g, color.b, color.a);

        public static void SetVector (this ComputeShader shader, string name, Vector2 vector)
            => shader.SetFloats (name, vector.x, vector.y);

        public static void SetVector (this ComputeShader shader, string name, Vector3 vector)
            => shader.SetFloats (name, vector.x, vector.y, vector.z);

        public static void SetVector (this ComputeShader shader, string name, Vector2Int vector)
            => shader.SetInts (name, vector.x, vector.y);

        public static void SetVector (this ComputeShader shader, string name, Vector3Int vector)
            => shader.SetInts (name, vector.x, vector.y, vector.z);
    }
}
