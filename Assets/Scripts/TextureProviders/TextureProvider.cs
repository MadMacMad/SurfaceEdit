using System;
using UnityEngine;

namespace Tilify.TextureProviders
{
    public abstract class TextureProvider : IDisposable
    {
        private RenderTexture texture;

        public void Override(RenderTexture texture)
        {
            new ComputeCopy (Provide (), texture).Execute();
        }
        public RenderTexture Provide ()
        {
            if ( texture is null)
                texture = Provide_Internal ();
            return texture.Copy ();
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
