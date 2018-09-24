using SurfaceEdit.TextureProviders;
using UnityEngine;

namespace SurfaceEdit
{
    public sealed class ChunkTexture
    {
        public TextureResolution TextureResolution { get; private set; }
        public ImmutableTextureResolution ChunkResolution { get; private set; }
        public RenderTexture RenderTexture { get; private set; }
        public Vector2Int ChunksCount { get; private set; }

        private TextureProvider provider;

        public ChunkTexture(TextureProvider provider, ImmutableTextureResolution chunkResolution)
        {
            Assert.ArgumentNotNull (provider, nameof (provider));

            TextureResolution = provider.resolution;

            if ( chunkResolution.AsInt > TextureResolution.AsInt )
                chunkResolution = provider.resolution.ToImmutable ();

            this.provider = provider;
            ChunkResolution = chunkResolution;
            ChunksCount = new Vector2Int (TextureResolution.AsInt / ChunkResolution.AsInt, TextureResolution.AsInt / ChunkResolution.AsInt);
            RenderTexture = provider.Provide ();

            this.provider.NeedUpdate += (s, e) =>
            {
                if ( ChunkResolution.AsInt > TextureResolution.AsInt )
                {
                    ChunkResolution = TextureResolution.ToImmutable();
                    ChunksCount = new Vector2Int (TextureResolution.AsInt / ChunkResolution.AsInt, TextureResolution.AsInt / ChunkResolution.AsInt);
                }

                RenderTexture.Release ();
                RenderTexture = this.provider.Provide ();
            };
        }

        public static implicit operator RenderTexture(ChunkTexture chunkTexture)
            => chunkTexture.RenderTexture;
    }
}
