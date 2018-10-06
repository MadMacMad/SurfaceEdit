using UnityEngine;

namespace SurfaceEdit.TextureProviders
{
    public class SolidColorTextureProvider : TextureProvider
    {
        private Color color;

        public SolidColorTextureProvider(TextureResolution resolution, Color color) : base (resolution)
        {
            this.color = color;
        }

        protected override RenderTexture Provide_Internal ()
        {
            var renderTexture = TextureUtility.CreateRenderTexture (resolution.AsVector);
            renderTexture = new ComputeFillWithColor (renderTexture, color).Execute ();
            return renderTexture;
        }
    }
}
