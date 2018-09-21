using System.Collections.Generic;
using System.Linq;
using Tilify.TextureProviders;
using UnityEngine;

namespace Tilify
{
    public class Surface
    {
        public IReadOnlyDictionary<TextureChannel, RenderTexture> Textures => textures;
        private Dictionary<TextureChannel, RenderTexture> textures = new Dictionary<TextureChannel, RenderTexture> ();

        private Dictionary<TextureChannel, TextureProvider> providers;
        
        public static Surface CreateBlankSurface(TextureResolution textureResolution, TextureChannel channels)
        {
            var surface = new Surface
            {
                providers = new Dictionary<TextureChannel, TextureProvider> ()
            };

            foreach ( TextureChannel channel in channels.GetFlags () )
                surface.providers.Add (channel, new BlankChannelTextureProvider (textureResolution, channel));

            surface.FillTexturesArray ();

            return surface;
        }
        
        private Surface() { }

        public Surface (Dictionary<TextureChannel, TextureProvider> textureProviders)
        {
            Assert.ArgumentNotNull (textureProviders, nameof (textureProviders));
            Assert.ArgumentTrue (textureProviders.Count >= 1, nameof (textureProviders) + ".Count is less then 1");

            int index = 0;
            foreach ( var pair in textureProviders )
            {
                Assert.ArgumentNotNull (pair.Value, $"{nameof (textureProviders)}[{index}]");
                index++;
            }
            
            providers = textureProviders;

            FillTexturesArray ();
        }

        private void FillTexturesArray ()
        {
            foreach ( var pair in providers )
                textures.Add (pair.Key, pair.Value.Provide ());
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