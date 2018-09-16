using System;
using UnityEngine;

namespace Tilify.TextureProviders
{
    public abstract class TextureProvider : IDisposable
    {
        private RenderTexture texture;
        private RenderTexture textureCopy;

        public void Override(RenderTexture texture)
        {
            if ( this.texture is null )
                this.texture = Provide_Internal ();

            new ComputeCopy (this.texture, texture).Execute();
        }
        public RenderTexture Provide ()
        {
            if ( texture is null )
            {
                texture = Provide_Internal ();
                textureCopy = texture.Copy ();
            }
            return textureCopy;
        }

        protected abstract RenderTexture Provide_Internal ();

        public void Dispose ()
        {
            texture.Release ();
            Dispose_Internal ();
        }
        protected virtual void Dispose_Internal () { }
    }
}
