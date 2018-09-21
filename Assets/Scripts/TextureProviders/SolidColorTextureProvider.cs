using UnityEngine;

namespace Tilify.TextureProviders
{
    public class SolidColorTextureProvider : TextureProvider
    {
        private Color color;
        private Vector2Int textureSize;

        public SolidColorTextureProvider(Vector2Int textureSize, Color color, bool cacheTexture = true) : base (cacheTexture)
        {
            textureSize = TextureHelper.Instance.ClampTextureSize (textureSize);

            this.color = color;
            this.textureSize = textureSize;
        }

        protected override RenderTexture Provide_Internal ()
        {
            var renderTexture = Utils.CreateAndAllocateRenderTexture (textureSize);
            renderTexture = new ComputeFillWithColor (renderTexture, color).Execute ();
            return renderTexture;
        }
    }
}
