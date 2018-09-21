using UnityEngine;

namespace Tilify.TextureProviders
{
    public class BlankChannelTextureProvider : TextureProvider
    {
        private TextureResolution resolution;
        private TextureChannel textureChannel;

        public BlankChannelTextureProvider(TextureResolution resolution, TextureChannel textureChannel, bool cacheTexture = true) : base (cacheTexture)
        {
            Assert.ArgumentNotNull (resolution, nameof (resolution));
            
            this.resolution = resolution;
            this.textureChannel = textureChannel;

            resolution.PropertyChanged += (s, e) => isNeedReprovide = true;
        }

        protected override RenderTexture Provide_Internal ()
        {
            switch (textureChannel)
            {
                case TextureChannel.Albedo:             return ProvideSolidColorRenderTexture (Color.white);
                case TextureChannel.Normal:             return ProvideSolidColorRenderTexture (new Color32 (128, 128, 255, 255));
                case TextureChannel.Metallic:           return ProvideSolidColorRenderTexture (Color.black);
                case TextureChannel.Roughness:          return ProvideSolidColorRenderTexture (Color.white);
                case TextureChannel.Height:             return ProvideSolidColorRenderTexture (new Color(.5f, .5f, .5f, 1));
                case TextureChannel.Mask:               return ProvideSolidColorRenderTexture (Color.white);
                default:                                return ProvideSolidColorRenderTexture (Color.white);
            }
        }
        private RenderTexture ProvideSolidColorRenderTexture(Color color)
        {
            var renderTexture = Utils.CreateAndAllocateRenderTexture (resolution.Vector);
            return new ComputeFillWithColor (renderTexture, color).Execute();
        }
    }
}
