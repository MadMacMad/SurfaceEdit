using System;
using UnityEngine;

namespace SurfaceEdit.TextureProviders
{
    public abstract class TextureProvider : ObjectChangedNotifier, IDisposable
    {
        public readonly TextureResolution resolution;

        public RenderTexture Texture
        {
            get
            {
                if ( texture == null )
                    texture = ProvideAdjustScale ();
                return texture;
            }
        }
        private RenderTexture texture;

        public TextureProvider (TextureResolution resolution)
        {
            Assert.ArgumentNotNull (resolution, nameof (resolution));

            this.resolution = resolution;
            
            resolution.Changed += (s, e) =>
            {
                texture?.Release ();
                texture = null;
                NotifyChanged ();
            };
        }

        public void Fill (RenderTexture texture, Vector2Int origin, Vector2Int size)
        {
            Assert.ArgumentNotNull (texture, nameof (texture));

            var compute = new ComputeCopy (this.Texture, texture);
            compute.Size = size;
            compute.Origin = origin;
            compute.Execute ();
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
            Texture?.Release ();
            Dispose_Internal ();
        }
        protected virtual void Dispose_Internal () { }
    }
}