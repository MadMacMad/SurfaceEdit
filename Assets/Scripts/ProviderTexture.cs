using System;
using SurfaceEdit.TextureProviders;
using UnityEngine;

namespace SurfaceEdit
{
    public sealed class ProviderTexture : IDisposable
    {
        public RenderTexture RenderTexture { get; private set; }
        public ProgramContext Context { get; private set; }

        private TextureProvider provider;

        public ProviderTexture (ProgramContext context, TextureProvider provider)
        {
            Assert.ArgumentNotNull (context, nameof (context));
            Assert.ArgumentNotNull (provider, nameof (provider));

            Assert.ArgumentTrue (ReferenceEquals (provider.resolution, context.TextureResolution), $"{nameof (provider)}.resolution is not {nameof (context)}.TextureResolution");

            Context = context;
            this.provider = provider;

            RenderTexture = provider.Provide();

            provider.Changed += (s, e) => RenderTexture = provider.Provide ();
        }

        public void Reset (Vector2Int origin, Vector2Int size)
            => provider.Override (RenderTexture, origin, size);

        public void Dispose ()
        {
            provider.Dispose ();
            RenderTexture.Release ();
        }

        public static implicit operator RenderTexture (ProviderTexture providerTexture)
            => providerTexture.RenderTexture;
    }
}