using System.Collections.Generic;
using System.Linq;
using SurfaceEdit.TextureProviders;
using UnityEngine;

namespace SurfaceEdit
{
    public static class SurfaceCombiner
    {
        private static RenderTexture whiteTexture;

        static SurfaceCombiner()
        {
            whiteTexture = new SolidColorTextureProvider (new TextureResolution (TextureResolutionEnum.x32), Color.white, false).Provide();
        }

        public static void CombineSurfaces(ProgramContext context, RenderContext renderContext, Surface bottomSurface, Surface topSurface, LayerBlendType blendType)
        {
            Assert.ArgumentNotNull (context, nameof (context));
            Assert.ArgumentNotNull (renderContext, nameof (renderContext));
            Assert.ArgumentNotNull (bottomSurface, nameof (bottomSurface));
            Assert.ArgumentNotNull (topSurface, nameof (topSurface));

            var channels = new List<Channel> ();
            channels.AddRange(bottomSurface.Context.Channels.List);

            Assert.ArgumentTrue (channels.Except (topSurface.Context.Channels.List.ToList()).Count () == 0, "Surfaces channels are not equals");

            if ( channels.Contains(Channel.Mask))
            {
                var mask = topSurface.Textures[Channel.Mask];
                RenderTexture bottomHeight = null;
                RenderTexture topHeight = null;

                channels.Remove (Channel.Mask);

                if ( blendType == LayerBlendType.HeightBlend )
                {
                    channels.Remove (Channel.Height);
                    channels.Add (Channel.Height);
                    bottomHeight = bottomSurface.Textures[Channel.Height];
                    topHeight = topSurface.Textures[Channel.Height];
                }

                foreach (var channel in channels )
                {
                    var bottomTexture = bottomSurface.Textures[channel];
                    var topTexture = topSurface.Textures[channel];

                    if ( blendType == LayerBlendType.AlphaBlend )
                    {
                        AlphaBlend (context, renderContext, bottomTexture, topTexture, mask);
                    }
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

        private static void AlphaBlend (ProgramContext context, RenderContext renderContext, RenderTexture bottomTexture, RenderTexture topTexture, RenderTexture mask)
        {
            var compute = new ComputeCombineAlphaBlend (bottomTexture, topTexture, mask);

            if ( renderContext.Covering == RenderCovering.Part )
            {
                compute.Size = context.ChunkResolution.AsVector;
                var chunkResolution = context.ChunkResolution.AsInt;
                foreach ( var pixelPosition  in renderContext.ChunksToRender.PixelPositions )
                {
                    compute.Origin = pixelPosition;
                    compute.Execute ();
                }
            }
            else
            {
                compute.Execute ();
            }
        }

        private static void NoBlend (RenderTexture bottomTexture, RenderTexture topTexture)
           => new ComputeCombineAlphaBlend (bottomTexture, topTexture, whiteTexture).Execute ();

        private static void HeightBlend (RenderTexture bottomTexture, RenderTexture topTexture, RenderTexture bottomHeight, RenderTexture topHeight, RenderTexture mask)
           => new ComputeCombineHeightBlend (bottomTexture, topTexture, bottomHeight, topHeight, mask).Execute ();
    }
}
