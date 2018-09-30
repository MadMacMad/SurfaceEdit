using System;
using System.Collections.Generic;
using SurfaceEdit.TextureProviders;
using UnityEngine;

namespace SurfaceEdit
{
    public sealed class Surface : ObjectChangedNotifier, IDisposable
    {
        public ProgramContext Context { get; private set; }

        public IReadOnlyDictionary<Channel, ProviderTexture> Textures => textures;
        private Dictionary<Channel, ProviderTexture> textures = new Dictionary<Channel, ProviderTexture> ();

        public Surface (ProgramContext context)
        {
            Assert.ArgumentNotNull (context, nameof (context));

            Context = context;

            CreateTextures ();
            
            // context.Changed += (s, e) => Update(Recreate?)Textures();
        }
        // TODO: Recteate textures on context change
        private void CreateTextures()
        {
            foreach ( var channel in Context.Channels.List )
            {
                var provider = new BlankChannelTextureProvider (Context.TextureResolution, channel);
                var providerTexture = new ProviderTexture (Context, provider);
                textures.Add (channel, providerTexture);
            }
        }
        
        public Dictionary<Channel, ProviderTexture> SelectTextures (IEnumerable<Channel> selectionList)
        {
            Assert.ArgumentNotNull (selectionList, nameof(selectionList));

            var result = new Dictionary<Channel, ProviderTexture> ();

            foreach(var channel in selectionList)
            {
                if ( result.ContainsKey (channel) )
                    continue;

                if (textures.ContainsKey(channel))
                    result.Add (channel, textures[channel]);
            }

            return result;
        }

        public void Reset (RenderContext renderContext)
        {
            Assert.ArgumentNotNull (renderContext, nameof (renderContext));

            var textureResolution = Context.TextureResolution.AsVector;
            var chunkResolution = Context.ChunkResolution.AsVector;
            foreach ( var channel in renderContext.ChannelsToRender.List )
            {
                if ( renderContext.Covering == RenderCovering.Part )
                {
                    foreach ( var pixelPosition in renderContext.ChunksToRender.PixelPositions )
                        textures[channel].Reset (pixelPosition, chunkResolution);
                }
                else
                    textures[channel].Reset (Vector2Int.zero, textureResolution);
            }
        }
        public void Dispose()
        {
            foreach(var pair in textures)
                pair.Value.Dispose ();
        }
    }
}