using UnityEngine;

namespace SurfaceEdit.Affectors
{
    public class TextureFillAffector : Affector
    {
        public Texture2DResource TextureResource
        {
            get => textureResource;
            set
            {
                Assert.ArgumentNotNull (value, nameof (value));

                textureResource.UnChain (this);
                textureResource = value;
                textureResource.Chain (this);
                NotifyNeedRender (new RenderContext (AffectedChannels.ToImmutable ()));
            }
        }
        private Texture2DResource textureResource;

        private PartTextureComputeExecutor compute;

        public TextureFillAffector (ApplicationContext context, Channels affectedChannels, Texture2DResource textureResource)
            : base (context, affectedChannels)
        {
            Assert.ArgumentNotNull (textureResource, nameof (textureResource));

            this.textureResource = textureResource;
            context.Changed += (s, e) => NotifyNeedRender (new RenderContext(AffectedChannels.ToImmutable()));

            textureResource.Chain (this);
        }

        public override void Dispose()
        {
            base.Dispose ();
            textureResource.UnChain (this);
        }

        private void OnTextureDestroyed()
        {
            textureResource = null;
            NotifyNeedRender (new RenderContext (AffectedChannels.ToImmutable ()));
        }

        protected override void PreAffect (ProviderTexture textureToAffect)
        {
            compute = new ComputeCopy (textureResource.Texture, textureToAffect.RenderTexture);
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