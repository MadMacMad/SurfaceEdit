using System;
using UnityEngine;

namespace Tilify.TextureProviders
{
    public abstract class TextureProvider : IDisposable
    {
        private RenderTexture initialTexture;
        private RenderTexture copiedTexture;

        private bool cacheTexture;

        public TextureProvider(bool cacheTexture)
        {
            this.cacheTexture = initialTexture;
        }

        public void Override(RenderTexture texture)
        {
            Assert.ArgumentNotNull (texture, nameof (texture));

            if ( initialTexture is null )
                initialTexture = Provide_Internal ();

            new ComputeCopy (initialTexture, texture).Execute();
        }
        public RenderTexture Provide ()
        {
            if ( initialTexture is null )
            {
                initialTexture = Provide_Internal ();
                if ( cacheTexture )
                    copiedTexture = initialTexture.Copy ();
            }
            return cacheTexture ? copiedTexture : initialTexture;
        }

        protected abstract RenderTexture Provide_Internal ();

        public void Dispose ()
        {
            initialTexture?.Release ();
            copiedTexture?.Release ();
            Dispose_Internal ();
        }
        protected virtual void Dispose_Internal () { }
    }
}
