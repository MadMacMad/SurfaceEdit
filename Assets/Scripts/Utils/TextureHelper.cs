using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tilify
{
    public class TextureHelper : Singleton<TextureHelper>
    {
        private Vector2Int minTextureSize;
        private Vector2Int maxTextureSize;

        public TextureHelper()
        {
            minTextureSize = Settings.minTextureSize;
            maxTextureSize = Settings.maxTextureSize;
        }

        public TextureHelper(Vector2Int minTextureSize, Vector2Int maxTextureSize)
        {
            Assert.ArgumentTrue (minTextureSize.x < maxTextureSize.x, nameof (minTextureSize) + ".x is less or equals then " + nameof (maxTextureSize) + ".x");
            Assert.ArgumentTrue (minTextureSize.y < maxTextureSize.y, nameof (minTextureSize) + ".y is less or equals then " + nameof (maxTextureSize) + ".y");

            this.minTextureSize = minTextureSize;
            this.maxTextureSize = maxTextureSize;
        }

        public Vector2Int ClampTextureSize (Vector2Int textureSize)
        {
            textureSize.Clamp (minTextureSize, maxTextureSize);
            return textureSize;
        }
    }
}
