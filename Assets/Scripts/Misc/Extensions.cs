using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SurfaceEdit
{
    public static class Extensions
    {
        public static Texture2D ConvertToTexture2D (this RenderTexture renderTexture)
        {
            var newTexture = new Texture2D (renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Graphics.CopyTexture (renderTexture, 0, 0, newTexture, 0, 0);
            return newTexture;
        }
        public static Texture2D ConvertToTexture2DAndRelease (this RenderTexture renderTexture)
        {
            var newTexture = renderTexture.ConvertToTexture2D ();
            renderTexture.Release ();
            return newTexture;
        }

        public static RenderTexture ConvertToRenderTexture (this Texture2D texture)
        {
            var renderTexture = TextureUtility.CreateRenderTexture (texture.width, texture.height);
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
            var renderTexture = TextureUtility.CreateRenderTexture (texture.width, texture.height);
            new ComputeCopy (texture, renderTexture).Execute();
            return renderTexture;
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

        public static Vector2 ClampNew (this Vector2 vector, Vector2 min, Vector2 max)
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

        public static Vector2 Clamp01New (this Vector2 vector)
            => vector.ClampNew (Vector2.zero, Vector2.one);

        public static Vector2Int ClampNew (this Vector2Int vector, Vector2Int min, Vector2Int max)
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

        public static IEnumerable<Enum> GetFlags (this Enum e) 
            => Enum.GetValues (e.GetType ()).Cast<Enum> ().Where (e.HasFlag);

        public static void SetColor(this ComputeShader shader, string name, Color color)
            => shader.SetFloats (name, color.r, color.g, color.b, color.a);

        public static bool IsHasEqualSize (this Texture texture, params Texture[] other)
        {
            foreach(var otherTexture in other)
                if ( otherTexture.width != otherTexture.width || otherTexture.height != otherTexture.height )
                    return false;
            return true;
        }

        public static Vector2Int GetVectorSize (this Texture texture)
            => new Vector2Int (texture.width, texture.height);

        public static Dictionary<TKey, TValue> AddRange<TKey, TValue> (this Dictionary<TKey, TValue> dict, IEnumerable<KeyValuePair<TKey, TValue>> range, DictionaryAddRangeMode mode)
        {
            Assert.ArgumentNotNull (range, nameof (range));

            foreach ( var pair in range )
            {
                if (dict.ContainsKey(pair.Key))
                    if ( mode == DictionaryAddRangeMode.OverrideDuplicates )
                    {
                        dict.Remove (pair.Key);
                        dict.Add (pair.Key, pair.Value);
                    }
                else
                    dict.Add (pair.Key, pair.Value);
            }
            return dict;
        }

        public enum DictionaryAddRangeMode
        {
            OverrideDuplicates,
            IgnoreDuplicates
        }

        public static void AddListener (this EventTrigger trigger, EventTriggerType eventType, Action<PointerEventData> listener)
        {
            var entry = new EventTrigger.Entry ();
            entry.eventID = eventType;
            entry.callback.AddListener (data => listener.Invoke ((PointerEventData)data));
            trigger.triggers.Add (entry);
        }

        public static bool IsDirectory (this string path)
        {
            return Path.GetExtension (path) == "";
        }
    }
}
