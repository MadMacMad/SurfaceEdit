using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilify.TextureProviders;
using UnityEngine;

namespace Tilify
{
    public class Surface
    {
        public Vector2 WorldSize { get; }

        public IReadOnlyDictionary<TextureChannel, RenderTexture> Textures => textures;
        private Dictionary<TextureChannel, RenderTexture> textures = new Dictionary<TextureChannel, RenderTexture> ();

        private Dictionary<TextureChannel, TextureProvider> providers;


        public Surface (TextureChannel textureChannel, TextureProvider textureProvider, Vector2 worldSize)
        {
            Assert.ArgumentNotNull (textureProvider, nameof (textureProvider));

            WorldSize = TextureHelper.Instance.ClampWorldSize (worldSize);

            providers = new Dictionary<TextureChannel, TextureProvider>
            {
                { textureChannel, textureProvider }
            };

            FillTexturesArray ();
        }

        public Surface(Dictionary<TextureChannel, TextureProvider> textureProviders, Vector2 worldSize)
        {
            Assert.ArgumentNotNull (textureProviders, nameof (textureProviders));
            Assert.ArgumentTrue (textureProviders.Count >= 1, nameof (textureProviders) + ".Count is less then 1");

            WorldSize = TextureHelper.Instance.ClampWorldSize (worldSize);

            providers = textureProviders;

            FillTexturesArray ();
        }

        private void FillTexturesArray()
        {
            foreach ( var pair in providers )
                textures.Add (pair.Key, pair.Value.Provide ());
        }
          
        public Dictionary<TextureChannel, RenderTexture> SelectTextures(List<TextureChannel> selectionList)
        {
            return selectionList
                .Where (s => textures.ContainsKey (s))
                .ToDictionary (s => s, s => textures[s]);
        }
        public void Reset(List<TextureChannel> selectionList)
        {
            foreach(var s in selectionList)
                providers[s].Override (textures[s]);
        }
        public void ResetAll()
        {
            foreach ( var pair in textures )
                providers[pair.Key].Override (textures[pair.Key]);
        }
    }
}
