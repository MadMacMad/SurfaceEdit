using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;

namespace SurfaceEdit
{
    public sealed class Texture2DResource : Resource
    {
        public Texture2D Texture
        {
            get
            {
                Assert.ArgumentTrue (IsLoaded, "Resource is not loaded! Chain it with Chain(object key) method and after that access this property!");
                return texture;
            }
        }
        private Texture2D texture;

        public static Texture2DResource New (ResourceMetadata metadata, Texture2D texture)
        {
            Assert.ArgumentNotNull (metadata, nameof (metadata));
            Assert.ArgumentNotNull (texture, nameof (texture));

            var resource = new Texture2DResource (metadata);
            resource.texture = texture;
            resource.Cache ();
            resource.UnloadResource ();

            return resource;
        }
        public static Texture2DResource Existing (ResourceMetadata metadata)
        {
            Assert.ArgumentNotNull (metadata, nameof (metadata));

            return new Texture2DResource (metadata);
        }

        private Texture2DResource (ResourceMetadata metadata) : base (metadata)
        {
            metadata.Context.TextureResolution.Changed += (s, e) =>
            {
                if ( texture != null )
                    AdjustScale (ref texture);
            };
        }

        private Texture2D LoadTextureFromCache ()
        {
            var texturePath = Path.Combine (Metadata.ResourceCacheDirectory, "texture." + Metadata.TextureExtensionString);
            TextureUtility.TryLoadTexture2DFromDisk (texturePath, out var texture);
            if ( texture == null )
                NotifyFatalError ();
            return texture;
        }
        private void AdjustScale (ref Texture2D texture)
        {
            if ( texture.width > Metadata.Context.TextureResolution.AsInt || texture.height > Metadata.Context.TextureResolution.AsInt )
            {
                var result = new ComputeRescaleStupid (texture, Metadata.Context.TextureResolution.AsVector).Execute ();
                GameObject.DestroyImmediate (texture);
                texture = result.ConvertToTexture2DAndRelease ();
            }
            if  (texture.width < Metadata.Context.TextureResolution.AsInt || texture.height < Metadata.Context.TextureResolution.AsInt )
            {
                ReloadResource ();
            }
        }

        protected override Texture2D GenerateThumbnail ()
        {
            if ( texture != null )
                return new ComputeRescaleStupid (texture, ThumbnailSize).Execute ().ConvertToTexture2DAndRelease ();
            else
            {
                var texture = LoadTextureFromCache ();

                if ( texture == null )
                    return null;

                var thumbnail = new ComputeRescaleStupid (texture, ThumbnailSize).Execute ().ConvertToTexture2DAndRelease ();
                GameObject.DestroyImmediate (texture);
                return thumbnail;
            }
        }
        protected override void CacheResourceSpecific ()
        {
            var texturePath = Path.Combine (Metadata.ResourceCacheDirectory, "texture." + Metadata.TextureExtension);
            TextureUtility.SaveTexture2DToDisk (texturePath, texture);
        }

        protected override void LoadResource ()
        {
            if (texture == null)
            {
                texture = LoadTextureFromCache ();
                if ( texture == null )
                    return;
            }
            AdjustScale (ref texture);
        }
        protected override void UnloadResource ()
        {
            if ( texture != null )
                GameObject.DestroyImmediate (texture);
        }

        private void ReloadResource()
        {
            UnloadResource ();
            LoadResource ();
        }
    }
}