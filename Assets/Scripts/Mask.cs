using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
