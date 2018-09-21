using System.IO;
using UnityEngine;

namespace Tilify
{
    public static class TextureIOHelper
    {
        public static Texture2D LoadTexture2DFromDisk(string path)
        {
            Texture2D texture = null;

            if ( File.Exists (path) )
            {
                byte[] data = File.ReadAllBytes (path);
                texture = new Texture2D (2, 2, TextureFormat.ARGB32, false);
                texture.LoadImage (data);
            }

            return texture;
        }
    }
}
