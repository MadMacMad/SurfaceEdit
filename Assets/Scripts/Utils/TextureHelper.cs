using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tilify
{
    public class TextureHelper
    {
        public static TextureHelper Instance
        {
            get
            {
                if ( instance is null )
                    instance = new TextureHelper (Settings.minTextureSize, Settings.maxTextureSize);
                return instance;
            }
        }
        private static TextureHelper instance;

        private Vector2 minTextureSize;
        private Vector2 maxTextureSize;

        public TextureHelper(Vector2 minTextureSize, Vector2 maxTextureSize)
        {
            Assert.ArgumentTrue (minTextureSize.x < maxTextureSize.x, nameof (minTextureSize) + ".x is less or equals then " + nameof (maxTextureSize) + ".x");
            Assert.ArgumentTrue (minTextureSize.y < maxTextureSize.y, nameof (minTextureSize) + ".y is less or equals then " + nameof (maxTextureSize) + ".y");

            this.minTextureSize = minTextureSize;
            this.maxTextureSize = maxTextureSize;
        }

        public Vector2 ClampTextureSize (Vector2 textureSize)
        {
            return textureSize.Clamp (minTextureSize, maxTextureSize);
        }
    }
}
