using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurfaceEdit.TextureProviders;
using UnityEngine;

namespace SurfaceEdit
{
    public static class SurfaceCombiner
    {
        private static RenderTexture whiteTexture;

        static SurfaceCombiner()
        {
            whiteTexture = new SolidColorTextureProvider (new TextureResolution (TextureResolutionEnum.x4), Color.white, false).Provide();
        }

        public static void CombineSurfaces(Surface bottomSurface, Surface topSurface, LayerBlendType blendType)
        {
            Assert.ArgumentNotNull (bottomSurface, nameof (bottomSurface));
            Assert.ArgumentNotNull (topSurface, nameof (topSurface));
            Assert.ArgumentTrue (bottomSurface.Channels.Except (topSurface.Channels).Count () == 0, "Surfaces channels are not equlas");

            var channels = topSurface.Channels.ToList();

            if (topSurface.Channels.Contains(TextureChannel.Mask))
            {
                var mask = topSurface.Textures[TextureChannel.Mask];
                RenderTexture bottomHeight = null;
                RenderTexture topHeight = null;

                channels.Remove (TextureChannel.Mask);

                if ( blendType == LayerBlendType.HeightBlend )
                {
                    channels.Remove (TextureChannel.Height);
                    channels.Add (TextureChannel.Height);
                    bottomHeight = bottomSurface.Textures[TextureChannel.Height];
                    topHeight = topSurface.Textures[TextureChannel.Height];
                }

                foreach (var channel in channels )
                {
                    var bottomTexture = bottomSurface.Textures[channel];
                    var topTexture = topSurface.Textures[channel];

                    if ( blendType == LayerBlendType.AlphaBlend )
                        AlphaBlend (bottomTexture, topTexture, mask);
                    else
                        HeightBlend (bottomTexture, topTexture, bottomHeight, topHeight, mask);
                }
            }
            else
            {
                foreach ( var channel in channels )
                {
                    var bottomTexture = bottomSurface.Textures[channel];
                    var topTexture = topSurface.Textures[channel];

                    NoBlend (bottomTexture, topTexture);
                }
            }
        }

        private static void AlphaBlend(RenderTexture bottomTexture, RenderTexture topTexture, RenderTexture mask)
            => new ComputeCombineAlphaBlend (bottomTexture, topTexture, mask).Execute ();

        private static void NoBlend (RenderTexture bottomTexture, RenderTexture topTexture)
           => new ComputeCombineAlphaBlend (bottomTexture, topTexture, whiteTexture).Execute ();

        private static void HeightBlend (RenderTexture bottomTexture, RenderTexture topTexture, RenderTexture bottomHeight, RenderTexture topHeight, RenderTexture mask)
           => new ComputeCombineHeightBlend (bottomTexture, topTexture, bottomHeight, topHeight, mask).Execute ();
    }
}
