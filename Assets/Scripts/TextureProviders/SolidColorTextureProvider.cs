using UnityEngine;

namespace SurfaceEdit.TextureProviders
{
    public class SolidColorTextureProvider : TextureProvider
    {
        private Color color;

        public SolidColorTextureProvider(TextureResolution resolution, Color color, bool cacheTexture = true) : base (resolution, cacheTexture)
        {
            this.color = color;
        }

        protected override RenderTexture Provide_Internal ()
        {
            var renderTexture = Utils.CreateRenderTexture (resolution.AsVector);
            renderTexture = new ComputeFillWithColor (renderTexture, color).Execute ();
            return renderTexture;
        }
    }
}
