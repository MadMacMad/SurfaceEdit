using System.Collections.Generic;
using System.Linq;
using SurfaceEdit.TextureProviders;
using UnityEngine;

namespace SurfaceEdit
{
    public sealed class Surface
    {
        public IReadOnlyDictionary<TextureChannel, RenderTexture> Textures => textures;
        private Dictionary<TextureChannel, RenderTexture> textures = new Dictionary<TextureChannel, RenderTexture> ();

        private Dictionary<TextureChannel, TextureProvider> providers = new Dictionary<TextureChannel, TextureProvider>();

        private TextureChannelCollection channels;
        private TextureResolution textureResolution;
        
        public void RecreateBlankSurface(TextureChannelCollection newChannels)
        {
            foreach ( var newChannel in newChannels.List)
            {
                if ( !channels.List.Contains(newChannel) )
                {
                    channels.AddChannel (newChannel);

                    var provider = new BlankChannelTextureProvider (textureResolution, newChannel);
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

        private Surface() { }

        public Surface (TextureResolution textureResolution, TextureChannelCollection channels)
        {
            Assert.ArgumentNotNull (textureResolution, nameof (textureResolution));
            Assert.ArgumentNotNull (channels, nameof (channels));

            this.channels = channels;

            foreach(var channel in channels.List)
            {
                var provider = new BlankChannelTextureProvider (textureResolution, channel);
                providers.Add (channel, provider);
                textures.Add (channel, provider.Provide ());
            }
        }
        
        public Dictionary<TextureChannel, RenderTexture> SelectTextures (List<TextureChannel> selectionList)
        {
            Assert.ArgumentNotNull (selectionList, nameof(selectionList));

            return selectionList
                .Where (s => textures.ContainsKey (s))
                .ToDictionary (s => s, s => textures[s]);
        }

        public void Reset (List<TextureChannel> selectionList)
        {
            Assert.ArgumentNotNull (selectionList, nameof (selectionList));

            foreach ( var s in selectionList )
                providers[s].Override (textures[s]);
        }
        public void ResetAll ()
        {
            foreach ( var pair in providers )
                pair.Value.Override (textures[pair.Key]);
        }
    }
}