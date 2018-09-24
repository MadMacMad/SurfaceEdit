using System;
using System.Collections.Generic;
using System.Linq;
using SurfaceEdit.TextureProviders;
using UnityEngine;

namespace SurfaceEdit
{
    public sealed class Surface : PropertyChangedNotifier, IDisposable
    {
        public IReadOnlyCollection<TextureChannel> Channels => channels.List;

        public IReadOnlyDictionary<TextureChannel, ChunkTexture> Textures => textures;
        private Dictionary<TextureChannel, ChunkTexture> textures = new Dictionary<TextureChannel, ChunkTexture> ();

        private TextureChannelCollection channels;

        private TextureResolution textureResolution;
        private ImmutableTextureResolution chunkResolution;
        
        public void RecreateBlankSurface(TextureChannelCollection newChannels)
        {
            foreach ( var newChannel in newChannels.List)
            {
                if ( !channels.List.Contains(newChannel) )
                {
                    channels.AddChannel (newChannel);

                    var provider = new BlankChannelTextureProvider (textureResolution, newChannel);
                    var chunkTexture = new ChunkTexture (provider, chunkResolution);
                    textures.Add (newChannel, chunkTexture);
                    chunkTexture.NeedUpdate += (s, e) => NotifyNeedUpdate ();
                }
            }

            foreach(var channel in channels.List)
            {
                if (!newChannels.List.Contains(channel))
                {
                    textures[channel].Dispose ();
                    textures.Remove (channel);
                }
            }
        }

        public Surface (TextureResolution textureResolution, ImmutableTextureResolution chunkResolution, TextureChannelCollection channels)
        {
            Assert.ArgumentNotNull (textureResolution, nameof (textureResolution));
            Assert.ArgumentNotNull (chunkResolution, nameof (chunkResolution));
            Assert.ArgumentNotNull (channels, nameof (channels));

            this.channels = channels;
            this.textureResolution = textureResolution;
            this.chunkResolution = chunkResolution;

            CreateTextures ();
        }

        private void CreateTextures()
        {
            foreach ( var channel in channels.List )
            {
                var provider = new BlankChannelTextureProvider (textureResolution, channel);
                var chunkTexture = new ChunkTexture (provider, chunkResolution);
                textures.Add (channel, chunkTexture);

                chunkTexture.NeedUpdate += (s, e) => NotifyNeedUpdate ();
            }
        }
        
        public Dictionary<TextureChannel, ChunkTexture> SelectTextures (IEnumerable<TextureChannel> selectionList)
        {
            Assert.ArgumentNotNull (selectionList, nameof(selectionList));

            var result = new Dictionary<TextureChannel, ChunkTexture> ();

            foreach(var channel in selectionList)
            {
                if ( result.ContainsKey (channel) )
                    continue;

                if (textures.ContainsKey(channel))
                    result.Add (channel, textures[channel]);
            }

            return result;
        }

        public void Reset (IEnumerable<TextureChannel> selectionList)
        {
            Assert.ArgumentNotNull (selectionList, nameof (selectionList));

            foreach ( var s in selectionList.Distinct ().ToList () )
                textures[s].Reset ();
        }
        public void ResetAll ()
        {
            foreach ( var pair in textures )
                pair.Value.Reset ();
        }
        public void Dispose()
        {
            foreach(var pair in textures)
                pair.Value.Dispose ();
        }
    }
}