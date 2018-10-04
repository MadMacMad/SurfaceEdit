using UnityEngine;

namespace SurfaceEdit.TextureProviders
{
    public class BlankChannelTextureProvider : TextureProvider
    {
        public readonly Channel textureChannel;

        public BlankChannelTextureProvider(TextureResolution resolution, Channel textureChannel, bool cacheTexture = true) : base (resolution, cacheTexture)
        {
            this.textureChannel = textureChannel;
        }

        protected override RenderTexture Provide_Internal ()
        {
            switch (textureChannel)
            {
                case Channel.Albedo:             return ProvideSolidColorRenderTexture (Color.white);
                case Channel.Normal:             return ProvideSolidColorRenderTexture (new Color32 (128, 128, 255, 255));
                case Channel.Metallic:           return ProvideSolidColorRenderTexture (Color.black);
                case Channel.Roughness:          return ProvideSolidColorRenderTexture (Color.white);
                case Channel.Height:             return ProvideSolidColorRenderTexture (new Color(.5f, .5f, .5f, 1));
                case Channel.Mask:               return ProvideSolidColorRenderTexture (Color.black);
                default:                         return ProvideSolidColorRenderTexture (Color.white);
            }
        }
        private RenderTexture ProvideSolidColorRenderTexture(Color color)
        {
            var renderTexture = Utils.CreateRenderTexture (resolution.AsVector);
            return new ComputeFillWithColor (renderTexture, color).Execute();
        }
    }
}
