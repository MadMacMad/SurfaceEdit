using System;
using SurfaceEdit.TextureProviders;
using UnityEngine;

namespace SurfaceEdit
{
    public sealed class ProviderTexture : IDisposable
    {
        public RenderTexture RenderTexture { get; private set; }
        public ApplicationContext Context { get; private set; }

        private TextureProvider provider;

        public ProviderTexture (ApplicationContext context, TextureProvider provider)
        {
            Assert.ArgumentNotNull (context, nameof (context));
            Assert.ArgumentNotNull (provider, nameof (provider));

            Assert.ArgumentTrue (ReferenceEquals (provider.resolution, context.TextureResolution), $"{nameof (provider)}.resolution is not {nameof (context)}.TextureResolution");

            Context = context;
            this.provider = provider;

            RenderTexture = TextureUtility.CreateRenderTexture(context.TextureResolution.AsVector);

            provider.Changed += (s, e) => RenderTexture = provider.Texture;
        }

        public void Reset (Vector2Int origin, Vector2Int size)
            => provider.Fill (RenderTexture, origin, size);

        public void Dispose ()
        {
            provider.Dispose ();
            RenderTexture.Release ();
        }

        public static implicit operator RenderTexture (ProviderTexture providerTexture)
            => providerTexture.RenderTexture;
    }
}