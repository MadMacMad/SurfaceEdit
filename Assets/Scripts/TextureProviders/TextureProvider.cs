using System;
using UnityEngine;

namespace SurfaceEdit.TextureProviders
{
    public abstract class TextureProvider : IDisposable
    {
        private RenderTexture initialTexture;
        private RenderTexture copiedTexture;

        private bool isCacheTexture;

        protected bool isNeedReprovide = false;

        public TextureProvider(bool isCacheTexture)
        {
            this.isCacheTexture = isCacheTexture;
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
            if (isNeedReprovide)
            {
                initialTexture?.Release ();
                initialTexture = Provide_Internal ();
                isNeedReprovide = false;
                if ( isCacheTexture )
                    copiedTexture = initialTexture.Copy ();
            }
            else if ( initialTexture is null )
            {
                initialTexture = Provide_Internal ();
                if ( isCacheTexture )
                    copiedTexture = initialTexture.Copy ();
            }

            return isCacheTexture ? copiedTexture : initialTexture;
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
