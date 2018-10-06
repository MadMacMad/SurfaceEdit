using UnityEngine;

namespace SurfaceEdit.Affectors
{
    public sealed class ColorFillAffector : Affector
    {
        private Color color;

        public ColorFillAffector (ApplicationContext context, Channels affectedChannels, Color color)
            : base (context, affectedChannels)
        {
            this.color = color;
        }

        private ComputeFillWithColor compute;

        protected override void PreAffect (ProviderTexture texture)
        { 
            compute = new ComputeFillWithColor (texture.RenderTexture, color);
        }

        protected override void PostAffect ()
            => compute = null;

        protected override void Affect (ProviderTexture chunkTexture, Vector2Int pixelPosition, Vector2Int pixelSize)
        {
            compute.Origin = pixelPosition;
            compute.Size = pixelSize;
            compute.Execute ();
        }
    }
}