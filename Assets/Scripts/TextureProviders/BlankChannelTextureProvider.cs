using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tilify.TextureProviders
{
    public class BlankChannelTextureProvider : TextureProvider
    {
        private Vector2Int textureSize;
        private TextureChannel textureChannel;

        public BlankChannelTextureProvider(Vector2Int textureSize, TextureChannel textureChannel, bool cacheTexture = true) : base (cacheTexture)
        {
            textureSize.Clamp (Settings.minTextureSize, Settings.maxTextureSize);
            this.textureSize = textureSize;
            this.textureChannel = textureChannel;
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
                case TextureChannel.Unknown:            return ProvideSolidColorRenderTexture (Color.white);
            }
            return ProvideSolidColorRenderTexture (Color.white);
        }
        private RenderTexture ProvideSolidColorRenderTexture(Color color)
        {
            var renderTexture = Utils.CreateAndAllocateRenderTexture (textureSize);
            return new ComputeFillWithColor (renderTexture, color).Execute();
        }
    }
}
