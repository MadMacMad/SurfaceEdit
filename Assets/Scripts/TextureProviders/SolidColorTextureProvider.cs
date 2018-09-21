using UnityEngine;

namespace Tilify.TextureProviders
{
    public class SolidColorTextureProvider : TextureProvider
    {
        private Color color;
        private TextureResolution resolution;

        public SolidColorTextureProvider(TextureResolution resolution, Color color, bool cacheTexture = true) : base (cacheTexture)
        {
            Assert.ArgumentNotNull (resolution, nameof (resolution));

            this.color = color;
            this.resolution = resolution;
        }

        protected override RenderTexture Provide_Internal ()
        {
            var renderTexture = Utils.CreateAndAllocateRenderTexture (resolution.Vector);
            renderTexture = new ComputeFillWithColor (renderTexture, color).Execute ();
            return renderTexture;
        }
    }
}
