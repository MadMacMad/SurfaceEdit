using SurfaceEdit.TextureProviders;
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

                if (textureResource != null)
                    textureResource.Destroyed -= OnTextureDestroyed;

                value.Destroyed += OnTextureDestroyed;
                textureResource = value;
                NotifyNeedRender (new RenderContext (AffectedChannels.ToImmutable ()));
            }
        }
        private Texture2DResource textureResource;

        public TextureFillAffector (ApplicationContext context, Channels affectedChannels, Texture2DResource textureResource)
            : base (context, affectedChannels)
        {
            Assert.ArgumentNotNull (textureResource, nameof (textureResource));

            this.textureResource = textureResource;
            textureResource.Destroyed += OnTextureDestroyed;
            context.Changed += (s, e) => NotifyNeedRender (new RenderContext(AffectedChannels.ToImmutable()));
        }
        private PartTextureComputeExecutor compute;

        private void OnTextureDestroyed()
        {
            textureResource.Destroyed -= OnTextureDestroyed;
            textureResource = null;
            NotifyNeedRender (new RenderContext (AffectedChannels.ToImmutable ()));
        }

        protected override void PreAffect (ProviderTexture textureToAffect)
        {
            if ( textureResource != null )
                compute = new ComputeCopy (textureResource.Texture, textureToAffect.RenderTexture);
            else
                compute = new ComputeFillWithColor (textureToAffect.RenderTexture, Color.magenta);
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