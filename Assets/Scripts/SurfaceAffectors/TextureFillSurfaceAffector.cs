using SurfaceEdit.TextureProviders;
using UnityEngine;

namespace SurfaceEdit.SurfaceAffectors
{
    public class TextureFillSurfaceAffector : SurfaceAffector
    {
        private TextureProvider provider;

        public TextureFillSurfaceAffector (ProgramContext context, Channels affectedChannels, TextureProvider provider)
            : base (context, affectedChannels)
        {
            Assert.ArgumentNotNull (provider, nameof (provider));

            this.provider = provider;
            provider.Changed += (s, e) => NotifyNeedRender (new RenderContext(AffectedChannels.ToImmutable()));
        }
        private ComputeCopy compute;

        protected override void PreAffect (ProviderTexture texture)
        { 
            compute = new ComputeCopy (provider.Provide (), texture.RenderTexture);
            compute.Size = Context.ChunkResolution.AsVector;
        }

        protected override void PostAffect ()
            => compute = null;

        protected override void Affect (ProviderTexture texture, Vector2Int pixelPosition, Vector2Int pixelSize)
        {
            compute.Origin = pixelPosition;
            compute.Size = pixelSize;
            compute.Execute ();
        }
    }
}