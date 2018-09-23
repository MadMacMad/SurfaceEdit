using System;
using UnityEngine;

namespace SurfaceEdit.TextureProviders
{
    public abstract class TextureProvider : PropertyChangedNotifier, IDisposable
    {
        public readonly TextureResolution resolution;

        private RenderTexture initialTexture;
        private RenderTexture copiedTexture;

        private bool isCacheTexture;

        protected bool isNeedReprovide = false;

        public TextureProvider (TextureResolution resolution, bool isCacheTexture)
        {
            Assert.ArgumentNotNull (resolution, nameof (resolution));

            this.resolution = resolution;
            this.isCacheTexture = isCacheTexture;

            resolution.PropertyChanged += (s, e) =>
            {
                isNeedReprovide = true;
                NotifyNeedUpdate ();
            };
        }

        public void Override (RenderTexture texture)
        {
            Assert.ArgumentNotNull (texture, nameof (texture));

            Provide ();

            new ComputeCopy (initialTexture, texture).Execute ();
        }

        public RenderTexture Provide ()
        {
            if ( isNeedReprovide || initialTexture == null )
            {
                initialTexture?.Release ();
                initialTexture = ProvideAdjustScale ();
                isNeedReprovide = false;
                if ( isCacheTexture )
                {
                    copiedTexture?.Release ();
                    copiedTexture = initialTexture.Copy ();
                }
            }
            return isCacheTexture ? copiedTexture : initialTexture;
        }

        private RenderTexture ProvideAdjustScale ()
        {
            var texture = Provide_Internal ();
            if ( texture.width != resolution.Value || texture.height != resolution.Value )
            {
                var result = new ComputeRescaleStupid (texture, resolution.Vector).Execute ();
                texture.Release ();
                return result;
            }
            return texture;
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