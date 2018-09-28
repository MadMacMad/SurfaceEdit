using System;
using UnityEngine;

namespace SurfaceEdit.TextureProviders
{
    public abstract class TextureProvider : ObjectChangedNotifier, IDisposable
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

            resolution.Changed += (s, e) =>
            {
                isNeedReprovide = true;
                NotifyChanged ();
            };
        }

        public void Override (RenderTexture texture, Vector2Int origin, Vector2Int size)
        {
            Assert.ArgumentNotNull (texture, nameof (texture));

            if ( !isCacheTexture )
            {
                Debug.LogWarning ($"{nameof (TextureProvider)}.isCacheTexture is set to false. New texture will be loaded");
                initialTexture?.Release ();
                initialTexture = null;
            }

            Provide ();

            var compute = new ComputeCopy (initialTexture, texture);
            compute.Size = size;
            compute.Origin = origin;
            compute.Execute ();
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
            if ( texture.width != resolution.AsInt || texture.height != resolution.AsInt )
            {
                var result = new ComputeRescaleStupid (texture, resolution.AsVector).Execute ();
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