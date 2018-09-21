using Tilify.TextureProviders;
using UnityEngine;

namespace Tilify
{
    public class Mask
    {
        public RenderTexture Texture { get; }
        private TextureProvider textureProvider;
        public Mask(TextureProvider textureProvider)
        {
            Assert.ArgumentNotNull (textureProvider, nameof (textureProvider));

            this.textureProvider = textureProvider;
            Texture = this.textureProvider.Provide ();
        }
        
        public void Reset ()
        {
            textureProvider.Override (Texture);
        }
    }
}
