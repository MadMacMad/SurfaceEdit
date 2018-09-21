using UnityEngine;

namespace Tilify.TextureProviders
{
    public class SolidColorTextureProvider : TextureProvider
    {
        private Color color;
        private TextureResolution textureResolution;

        public SolidColorTextureProvider(TextureResolution textureResolution, Color color, bool cacheTexture = true) : base (cacheTexture)
        {
            Assert.ArgumentNotNull (textureResolution, nameof (textureResolution));

            this.color = color;
            this.textureResolution = textureResolution;
        }

        protected override RenderTexture Provide_Internal ()
        {
            var renderTexture = Utils.CreateAndAllocateRenderTexture (textureResolution.Vector);
            renderTexture = new ComputeFillWithColor (renderTexture, color).Execute ();
            return renderTexture;
        }
    }
}
