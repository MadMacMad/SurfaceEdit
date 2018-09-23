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

        public IReadOnlyDictionary<TextureChannel, RenderTexture> Textures => textures;
        private Dictionary<TextureChannel, RenderTexture> textures = new Dictionary<TextureChannel, RenderTexture> ();

        private Dictionary<TextureChannel, TextureProvider> providers = new Dictionary<TextureChannel, TextureProvider>();

        private TextureChannelCollection channels;
        private TextureResolution resolution;
        
        public void RecreateBlankSurface(TextureChannelCollection newChannels)
        {
            foreach ( var newChannel in newChannels.List)
            {
                if ( !channels.List.Contains(newChannel) )
                {
                    channels.AddChannel (newChannel);

                    var provider = new BlankChannelTextureProvider (resolution, newChannel);
                    providers.Add (newChannel, provider);
                    textures.Add (newChannel, provider.Provide());
                }
            }

            foreach(var channel in channels.List)
            {
                if (!newChannels.List.Contains(channel))
                {
                    var provider = providers[channel];
                    provider.Dispose ();
                    providers.Remove (channel);

                    var texture = textures[channel];
                    texture.Release ();
                    textures.Remove (channel);
                }
            }
        }

        public Surface (TextureResolution resolution, TextureChannelCollection channels)
        {
            Assert.ArgumentNotNull (resolution, nameof (resolution));
            Assert.ArgumentNotNull (channels, nameof (channels));

            this.channels = channels;
            this.resolution = resolution;
            
            CreateTextures ();
        }

        private void CreateTextures()
        {
            foreach ( var channel in channels.List )
            {
                var provider = new BlankChannelTextureProvider (resolution, channel);
                provider.NeedUpdate += (s, e) =>
                {
                    textures[provider.textureChannel] = provider.Provide ();
                    NotifyNeedUpdate ();
                };
                providers.Add (channel, provider);
                textures.Add (channel, provider.Provide ());
            }
        }
        
        public Dictionary<TextureChannel, RenderTexture> SelectTextures (IEnumerable<TextureChannel> selectionList)
        {
            Assert.ArgumentNotNull (selectionList, nameof(selectionList));

            var result = new Dictionary<TextureChannel, RenderTexture> ();

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

            foreach ( var s in selectionList.Distinct().ToList() )
                providers[s].Override (textures[s]);
        }
        public void ResetAll ()
        {
            foreach ( var pair in providers )
                pair.Value.Override (textures[pair.Key]);
        }
        public void Dispose()
        {
            foreach(var pair in textures)
                pair.Value.Release ();

            foreach ( var pair in providers )
                pair.Value.Dispose ();
        }
    }
}